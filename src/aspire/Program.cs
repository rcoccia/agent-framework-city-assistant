// Simple orchestration without Aspire-specific extensions
// This file is a placeholder for Aspire orchestration
// For now, run projects individually or use docker-compose

Console.WriteLine("Aspire AppHost");
Console.WriteLine("==============");
Console.WriteLine();
Console.WriteLine("To run the City Assistant application:");
Console.WriteLine("1. Start the restaurant agent: cd src/restaurant-agent && dotnet run");
Console.WriteLine("2. Start the orchestrator agent: cd src/orchestrator-agent && dotnet run");
Console.WriteLine("3. Start the frontend: cd src/frontend && npm run dev");
Console.WriteLine();
Console.WriteLine("Make sure to configure the required environment variables:");
Console.WriteLine("- ConnectionStrings__foundry: Azure AI Inference connection string");
Console.WriteLine("- ConnectionStrings__cosmos: Azure Cosmos DB connection string");
Console.WriteLine("- services__restaurant-agent__https__0: Restaurant agent URL (for orchestrator)");
Console.WriteLine();
