cd ./Src/
dotnet ef migrations remove --startup-project App --project Persistence --context %1
cd ..