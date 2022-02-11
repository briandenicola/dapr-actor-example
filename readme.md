First attempt at a Virtual Actor Pattern application 

# Server
cd src/BankActor
dapr init
dapr run --dapr-http-port 3500 --app-id bank_actor --app-port 5010 dotnet run 

# Client
cd src/Client
dotnet run