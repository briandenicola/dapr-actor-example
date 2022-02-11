namespace DaprBankActor;

public class BankService
{
    private readonly decimal OverdraftThreshold = -5m;

    public decimal Withdraw(decimal balance, decimal amount)
    {
        var updated = balance - amount;
        if (updated < OverdraftThreshold)
        {
            throw new Exception();
        }

        return updated;
    }

    public decimal Deposit(decimal balance, decimal amount)
    {
        var updated = balance + amount;
        return updated;
    }
}