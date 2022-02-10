namespace BankingService; 

public class BankActor : Actor, IBankActor, IRemindable
{
    private const string StateStoreName = "statestore";
    private readonly BankAccount account;

    public BankActor(ActorHost host, String name)
        : base(host)
    {
        this.account = SetupBankAccount(name).GetAwaiter().GetResult();
    }

    public async Task<BankAccount> SetupBankAccount(string name) 
    {
        var starting = new BankAccount()
        {
            AccountId = this.Id.GetId(),
            AccountName = name,
            Balance = 1000m,
        };
        
        var account = await this.StateManager.GetOrAddStateAsync<BankAccount>(StateStoreName, starting);
        return account; 
    }

    public async Task<BankAccount> GetAccountBalance()
    {
        var balance = await this.StateManager.GetOrAddStateAsync<BankAccount>(StateStoreName, account);
        return balance;
    }

    public async Task Deposit(decimal amount)
    {
        var balance = await this.StateManager.GetOrAddStateAsync<BankAccount>(StateStoreName, account);
        var updated = this.account.Deposit(amount);
        await this.StateManager.SetStateAsync("balance", balance);
    }

    public async Task Withdraw(decimal amount)
    {
        var balance = await this.StateManager.GetOrAddStateAsync<BankAccount>(StateStoreName, account);
        var updated = this.account.Withdraw(amount);
        await this.StateManager.SetStateAsync("balance", balance);
    }

    public Task SetupReoccuringWithdrawl(decimal amount) 
    {
        return this.RegisterReminder(TransferType.Withdraw, amount);
    }

    public async Task RegisterReminder(TransferType type, decimal amount, int seconds = 10)
    {
        var reminderName = $"{this.account.AccountName}-reoccuring-${type}";
        int period = 60;

        var reminderParams = new ReminderParams(){
            AccountName = this.account.AccountName,
            ReminderType = type,
            Amount = amount,
        };

        var serializedParams = JsonSerializer.SerializeToUtf8Bytes(reminderParams);
        await this.RegisterReminderAsync(reminderName, serializedParams, TimeSpan.FromSeconds(seconds), TimeSpan.FromSeconds(period));
    }

    public Task UnregisterReminder(string reminderName)
    {
        return this.UnregisterReminderAsync(reminderName);
    }

    public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        var actorState = await this.StateManager.GetStateAsync<BankAccount>(StateStoreName);
        var reminder = JsonSerializer.Deserialize<ReminderParams>(state);

        switch (reminder.ReminderType)  
        { 
            case TransferType.Deposit:
                await this.Deposit(reminder.Amount);
                break;
            case TransferType.Withdraw:
                if( actorState.Balance - reminder.Amount <= 0 ) {
                    await this.UnregisterReminder(reminderName);
                } 
                else {
                    await this.Withdraw(reminder.Amount);
                }
                break;
            default:
                throw new ArgumentException("Invalid value for reminder type", nameof(reminder.ReminderType));
        };

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