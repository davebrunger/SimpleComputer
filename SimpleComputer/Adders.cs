using System.Collections.Immutable;

namespace SimpleComputer;

public class HalfAdder
{
    private readonly Xor sum = new();
    private readonly And carry = new();

    public bool Sum => sum.Value;
    public bool Carry => carry.Value;


    public Task SetOutputSum(Func<bool, Task> output) => sum.SetOutput(output);
    public Task SetOutputCarry(Func<bool, Task> output) => carry.SetOutput(output);

    public Task SetInput1(bool input)
    {
        var sumTask = sum.SetInput1(input);
        var carryTask = carry.SetInput1(input);
        return Task.WhenAll(sumTask, carryTask);
    }

    public Task SetInput2(bool input)
    {
        var sumTask = sum.SetInput2(input);
        var carryTask = carry.SetInput2(input);
        return Task.WhenAll(sumTask, carryTask);
    }
}

public class FullAdder
{
    private readonly HalfAdder one = new();
    private readonly HalfAdder two = new();
    private readonly Or carry = new();

    public bool Sum => two.Sum;
    public bool Carry => carry.Value;

    public FullAdder()
    {
        var oneSumTask = one.SetOutputSum(two.SetInput1);
        var oneCarryTask = one.SetOutputCarry(carry.SetInput1);
        var twoCarryTask = two.SetOutputCarry(carry.SetInput2);
        Task.WaitAll(oneSumTask, oneCarryTask, twoCarryTask);
    }

    public Task SetOutputSum(Func<bool, Task> output) => two.SetOutputSum(output);
    
    public Task SetOutputCarry(Func<bool, Task> output) => carry.SetOutput(output);

    public Task SetInput1(bool input) => one.SetInput1(input);

    public Task SetInput2(bool input) => one.SetInput2(input);

    public Task SetInputCarry(bool input) => two.SetInput2(input);
}

public class EightBitAdder
{
    private readonly ImmutableArray<FullAdder> adders = Enumerable.Range(0, 8)
        .Select(_ => new FullAdder())
        .ToImmutableArray();

    public ImmutableArray<bool> Sum => adders.Select(a => a.Sum).ToImmutableArray();

    public bool Carry => adders[7].Carry;

    public EightBitAdder()
    {
        var setCarryTasks = Enumerable.Range(0, 7)
            .Select(i => adders[i].SetOutputCarry(adders[i + 1].SetInputCarry))
            .ToArray();

        Task.WaitAll(setCarryTasks);
    }

    public Task SetOutputSum(int bit, Func<bool, Task> output) => adders[bit].SetOutputSum(output);

    public Task SetOutputCarry(Func<bool, Task> output) => adders[7].SetOutputCarry(output);


    public Task SetInput1(int bit, bool value) => adders[bit].SetInput1(value);

    public Task SetInput2(int bit, bool value) => adders[bit].SetInput2(value);
    
    public Task SetInputCarry(bool value) => adders[0].SetInputCarry(value);
}


public class SixteenBitAdder
{
    private readonly EightBitAdder low = new EightBitAdder();
    private readonly EightBitAdder high = new EightBitAdder();

    public ImmutableArray<bool> Sum => low.Sum.Concat(high.Sum).ToImmutableArray();

    public bool Carry => high.Carry;

    public SixteenBitAdder()
    {
        var setCarryTask = low.SetOutputCarry(high.SetInputCarry);
        setCarryTask.Wait();
    }

    public Task SetOutputSum(int bit, Func<bool, Task> output) => Set(bit, (a, b) => a.SetOutputSum(b, output));

    public Task SetOutputCarry(Func<bool, Task> output) => high.SetOutputCarry(output);

    public Task SetInput1(int bit, bool value) => Set(bit, (a, b) => a.SetInput1(b, value));

    public Task SetInput2(int bit, bool value) => Set(bit, (a, b) => a.SetInput1(b, value));

    public Task SetInputCarry(bool value) => low.SetInputCarry(value);

    private Task Set(int bit, Func<EightBitAdder, int, Task> task) => task(bit / 8 == 0 ? low : high, bit % 8);
}

