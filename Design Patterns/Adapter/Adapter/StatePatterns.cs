using System;

// State interface
public interface IVendingMachineState
{
    void InsertCoin();
    void SelectProduct();
    void Dispense();
}

// Context class
public class VendingMachine
{
    private IVendingMachineState _state;

    public VendingMachine()
    {
        _state = new NoCoinState(this); // Initial state
    }

    public void SetState(IVendingMachineState state)
    {
        _state = state;
    }

    public void InsertCoin()
    {
        _state.InsertCoin();
    }

    public void SelectProduct()
    {
        _state.SelectProduct();
    }

    public void Dispense()
    {
        _state.Dispense();
    }
}

// Concrete state: NoCoin
public class NoCoinState : IVendingMachineState
{
    private readonly VendingMachine _machine;

    public NoCoinState(VendingMachine machine)
    {
        _machine = machine;
    }

    public void InsertCoin()
    {
        Console.WriteLine("Coin inserted.");
        _machine.SetState(new HasCoinState(_machine)); // Transition to HasCoin
    }

    public void SelectProduct()
    {
        Console.WriteLine("Insert a coin first.");
    }

    public void Dispense()
    {
        Console.WriteLine("Insert a coin and select a product first.");
    }
}

// Concrete state: HasCoin
public class HasCoinState : IVendingMachineState
{
    private readonly VendingMachine _machine;

    public HasCoinState(VendingMachine machine)
    {
        _machine = machine;
    }

    public void InsertCoin()
    {
        Console.WriteLine("Coin already inserted.");
    }

    public void SelectProduct()
    {
        Console.WriteLine("Product selected.");
        _machine.SetState(new DispensingState(_machine)); // Transition to Dispensing
    }

    public void Dispense()
    {
        Console.WriteLine("Select a product first.");
    }
}

// Concrete state: Dispensing
public class DispensingState : IVendingMachineState
{
    private readonly VendingMachine _machine;

    public DispensingState(VendingMachine machine)
    {
        _machine = machine;
    }

    public void InsertCoin()
    {
        Console.WriteLine("Dispensing in progress, please wait.");
    }

    public void SelectProduct()
    {
        Console.WriteLine("Dispensing in progress, please wait.");
    }

    public void Dispense()
    {
        Console.WriteLine("Product dispensed.");
        _machine.SetState(new NoCoinState(_machine)); // Transition back to NoCoin
    }
}

// Client code
public class Program
{
    public static void Main()
    {
        VendingMachine machine = new VendingMachine();

        // Simulate user actions
        machine.SelectProduct(); // Output: Insert a coin first.
        machine.InsertCoin();    // Output: Coin inserted.
        machine.SelectProduct(); // Output: Product selected.
        machine.Dispense();      // Output: Product dispensed.
        machine.InsertCoin();    // Output: Coin inserted.
    }
}