namespace SimpleComputer;

public abstract class SingleOutputBase
{
    protected readonly object updateLock = new();

    private Func<bool, Task>? output;

    public Task SetOutput(Func<bool, Task> output)
    {
        lock (updateLock)
        {
            this.output = output;
            return SendValueToOutput();
        }
    }

    public bool Value { get; protected set; }

    protected Task SendValueToOutput()
    {
        if (output != null)
        {
            return output(Value);
        }
        return Task.CompletedTask;
    }
}

public abstract class BufferBase : SingleOutputBase
{
    protected abstract bool ProcessInput(bool input);
    
    public Task SetInput(bool input)
    {
        lock (updateLock)
        {
            var newValue = ProcessInput(input);
            if (newValue == Value)
            {
                return Task.CompletedTask;
            }
            Value = newValue;
            return SendValueToOutput();
        }
    }
}

public class Buffer : BufferBase
{
    protected override bool ProcessInput(bool input) => input;
}

public class Not : BufferBase
{
    protected override bool ProcessInput(bool input) => !input;
}