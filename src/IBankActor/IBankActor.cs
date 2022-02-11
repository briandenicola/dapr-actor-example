namespace IBankActorInterface;

using System.Threading.Tasks;
using Dapr.Actors;

public class AccountBalance
{
    public string? AccountId { get; set; }

    public decimal Balance { get; set; }
}

public class WithdrawRequest
{
    public decimal Amount { get; set; }
}
public class DepositRequest
{
    public decimal Amount { get; set; }
}

public enum TransferType {
    Withdraw,
    Deposit,
};

public interface IBankActor : IActor
{
    Task<AccountBalance> SetupNewAccount(decimal startingDeposit);
    Task<AccountBalance> GetAccountBalance();
    Task Withdraw(WithdrawRequest withdraw);
    Task Desposit(DepositRequest deposit);   

    Task UnRegisterReoccuring(TransferType type);
    Task RegisterReoccuring(TransferType type, decimal amount);

}