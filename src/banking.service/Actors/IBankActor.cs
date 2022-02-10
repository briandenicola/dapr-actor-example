namespace BankingService;
public interface IBankActor : IActor
{
    Task<BankAccount> GetAccountBalance();
    Task<BankAccount> SetupBankAccount(string name);

    Task Withdraw(decimal amount);
    Task Deposit(decimal amount);
}