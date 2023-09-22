namespace SimpleComputer;

public abstract class GateBase : SingleOutputBase
{
    private bool input1;
    private bool input2;

    protected abstract bool ProcessInput(bool input1, bool input2);

    public Task SetInput1(bool input)
    {
        lock (updateLock)
        {
            if (input1 == input)
            {
                return Task.CompletedTask;
            }
            input1 = input;
            return UpdateValue();
        }
    }
    public Task SetInput2(bool input)
    {
        lock (updateLock)
        {
            if (input2 == input)
            {
                return Task.CompletedTask;
            }
            input2 = input;
            return UpdateValue();
        }
    }

    private Task UpdateValue()
    {
        var newValue = ProcessInput(input1, input2);
        if (newValue == Value)
        {
            return Task.CompletedTask;
        }
        Value = newValue;
        return SendValueToOutput();
    }
}

public class And : GateBase
{
    protected override bool ProcessInput(bool input1, bool input2) => input1 && input2;
}

public class Or : GateBase
{
    protected override bool ProcessInput(bool input1, bool input2) => input1 || input2;
}

public class Xor : GateBase
{
    protected override bool ProcessInput(bool input1, bool input2) => (input1 || input2) && !(input1 && input2);
}

public class Nand : GateBase
{
    protected override bool ProcessInput(bool input1, bool input2) => !(input1 && input2);
}

public class Nor : GateBase
{
    protected override bool ProcessInput(bool input1, bool input2) => !(input1 || input2);
}
