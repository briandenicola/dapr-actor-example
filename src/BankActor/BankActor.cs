namespace DaprBankActor;

using System;
using System.Text.Json;
using System.Threading.Tasks;
using Dapr.Actors.Runtime;
using IBankActorInterface;

public class BankActor : Actor, IBankActor, IRemindable
{
    private const string StateName = "statestore";

    private readonly BankService bank;
    private string AccountId;

    public BankActor(ActorHost host, BankService bank)
        : base(host)
    {
        this.bank = bank;
        this.AccountId = this.Id.GetId();
    }

    public async Task<AccountBalance> SetupNewAccount(decimal startingDeposit) 
    {
        var starting = new AccountBalance()
        {
            AccountId = AccountId,
            Balance = startingDeposit, 
        };

        var balance = await this.StateManager.GetOrAddStateAsync<AccountBalance>(AccountId, starting);
        return balance;
    }

    public Task UnRegisterReoccuring(TransferType type) 
    {
        return type switch 
        {
            TransferType.Deposit => this.UnregisterReminderAsync("deposit"),
            TransferType.Withdraw => this.UnregisterReminderAsync("withdraw"),
            _ => Task.CompletedTask
        };
    }

    public Task RegisterReoccuring(TransferType type, decimal amount)
    {
        var serializedParams = JsonSerializer.SerializeToUtf8Bytes(amount);

        return type switch 
        {
            TransferType.Deposit => this.RegisterReminderAsync("deposit", serializedParams, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5)),
            TransferType.Withdraw => this.RegisterReminderAsync("withdraw", serializedParams, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5)),
            _ => Task.CompletedTask,
        };
    }

    public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        var request = JsonSerializer.Deserialize<decimal>(state);

        return reminderName switch
        {
            "withdraw" => this.Withdraw(new WithdrawRequest(){ Amount = request}),
            "deposit" => this.Desposit(new DepositRequest(){ Amount = request}),
            _ => Task.CompletedTask 
        };
    }

    public async Task<AccountBalance> GetAccountBalance()
    {
        var balance = await this.StateManager.GetStateAsync<AccountBalance>(AccountId);
        return balance;
    }

    public async Task Desposit(DepositRequest deposit)
    {
        var balance = await this.StateManager.GetStateAsync<AccountBalance>(AccountId);
        var updated = this.bank.Deposit(balance.Balance, deposit.Amount);
        balance.Balance = updated;
        await this.StateManager.SetStateAsync(AccountId, balance);
    }
    public async Task Withdraw(WithdrawRequest withdraw)
    {
        var balance = await this.StateManager.GetStateAsync<AccountBalance>(AccountId);

        var updated = this.bank.Withdraw(balance.Balance, withdraw.Amount);
        balance.Balance = updated;
        await this.StateManager.SetStateAsync(AccountId, balance);
    }

    protected override Task OnActivateAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task OnDeactivateAsync()
    {
        return Task.CompletedTask;
    }
}

