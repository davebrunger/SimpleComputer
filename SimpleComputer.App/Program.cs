// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

bool? sum = null;
bool? carry = null;

var halfAdder = new SimpleComputer.FullAdder();
await halfAdder.SetSumOutput(o => { sum = o; Console.WriteLine($"{sum}, {carry}"); return Task.CompletedTask; });
await halfAdder.SetCarryOutput(o => { carry = o; Console.WriteLine($"{sum}, {carry}"); return Task.CompletedTask; });
Console.WriteLine($"Props: {halfAdder.Sum}, {halfAdder.Carry}");
Console.WriteLine();
await halfAdder.SetInput1(true);
Console.WriteLine($"Props: {halfAdder.Sum}, {halfAdder.Carry}");
Console.WriteLine();
await halfAdder.SetInput2(true);
Console.WriteLine($"Props: {halfAdder.Sum}, {halfAdder.Carry}");
Console.WriteLine();
await halfAdder.SetCarryIn(true);
Console.WriteLine($"Props: {halfAdder.Sum}, {halfAdder.Carry}");
Console.WriteLine();
await halfAdder.SetInput1(false);
Console.WriteLine($"Props: {halfAdder.Sum}, {halfAdder.Carry}");
Console.WriteLine();
await halfAdder.SetInput2(false);
Console.WriteLine($"Props: {halfAdder.Sum}, {halfAdder.Carry}");
Console.WriteLine();
await halfAdder.SetCarryIn(false);
Console.WriteLine($"Props: {halfAdder.Sum}, {halfAdder.Carry}");
