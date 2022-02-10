namespace BankingService.Models;

public record BankAccount {
    public string AccountId { get; init; } = default!;
    public string AccountName { get; init; } = default!;
    public decimal Balance { get; set; }

    public decimal Deposit(decimal amount) {
        Balance += amount;
        return Balance;
    }

    public decimal Withdraw(decimal amount) {
        Balance -= amount;
        return Balance;
    }
}

public record ReminderParams {
    public string AccountName { get; init; } = string.Empty;
    public TransferType ReminderType { get; init; } = TransferType.Deposit;
    public decimal Amount { get; set; }
}

public enum TransferType {
    Withdraw,
    Deposit,
};
