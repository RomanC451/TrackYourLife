cd ./src/
dotnet ef migrations remove --startup-project App --project Persistence --context ApplicationWriteDbContext
cd ..