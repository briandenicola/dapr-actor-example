namespace ActorClient;
using System;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;

using IBankActorInterface;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Creating a Bank Actor");
        var bankUser = ActorProxy.Create<IBankActor>(ActorId.CreateRandom(), "BankActor");
        await bankUser.SetupNewAccount(1000m);

        var balance = await bankUser.GetAccountBalance();
        Console.WriteLine($"Balance for account '{balance.AccountId}' is '{balance.Balance:c}'.");

        Console.WriteLine($"Setting up re-occurring withdrawal");
        await bankUser.RegisterReoccuring(TransferType.Withdraw, 50m);

        Console.WriteLine($"Sleeping 30 seconds");
        Thread.Sleep(TimeSpan.FromSeconds(10));
        
        balance = await bankUser.GetAccountBalance();
        Console.WriteLine($"Balance for account '{balance.AccountId}' is '{balance.Balance:c}'.");

        await bankUser.UnRegisterReoccuring(TransferType.Withdraw);
    }
}