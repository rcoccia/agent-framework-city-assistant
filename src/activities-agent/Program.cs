using A2A;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Hosting.A2A;
using Microsoft.Extensions.AI;
using ActivitiesAgent.Services;
using ActivitiesAgent.Tools;
using SharedServices;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Configure Azure chat client
builder.AddAzureChatCompletionsClient(connectionName: "foundry",
    configureSettings: settings =>
    {
        settings.TokenCredential = new DefaultAzureCredential();
        settings.EnableSensitiveTelemetryData = true;
    })
    .AddChatClient("gpt-4.1");

// Register services
builder.Services.AddSingleton<ActivitiesService>();
builder.Services.AddSingleton<ActivitiesTools>();

// Register OpenAI endpoints
builder.Services.AddOpenAIResponses();
builder.Services.AddOpenAIConversations();

// Register Cosmos for conversation storage
builder.AddKeyedAzureCosmosContainer("conversations",
    configureClientOptions: (option) => option.Serializer = new CosmosSystemTextJsonSerializer());
builder.Services.AddSingleton<ICosmosThreadRepository, CosmosThreadRepository>();
builder.Services.AddSingleton<CosmosAgentThreadStore>();

// Register the activities agent
builder.AddAIAgent("activities-agent", (sp, key) =>
{
    var chatClient = sp.GetRequiredService<IChatClient>();
    var activitiesTools = sp.GetRequiredService<ActivitiesTools>().GetFunctions();

    var agent = chatClient.CreateAIAgent(
        instructions: @"You are a helpful activities assistant. You help users discover and plan activities during their trip.
You can search for museums, theaters, cultural events, and attractions.
Each activity includes detailed information about hours, dates, pricing, restrictions, accessibility, location, and user reviews.
Always be friendly and provide comprehensive information to help users plan their visit.
When users ask about activities, use the available tools to retrieve the information.",
        description: "A friendly activities assistant that helps discover museums, theaters, cultural events, and attractions",
        name: key,
        tools: [.. activitiesTools]
    );

    return agent;
}).WithThreadStore((sp, key) => sp.GetRequiredService<CosmosAgentThreadStore>());

var app = builder.Build();

// Map A2A endpoint
app.MapA2A("activities-agent", "/agenta2a", new AgentCard
{
    Name = "activities-agent",
    Url = app.Configuration["ASPNETCORE_URLS"]?.Split(';')[0] + "/agenta2a" ?? "http://localhost:5198/agenta2a",
    Description = "An activities assistant that helps users discover and plan activities including museums, theaters, cultural events, and attractions",
    Version = "1.0",
    DefaultInputModes = ["text"],
    DefaultOutputModes = ["text"],
    Capabilities = new AgentCapabilities
    {
        Streaming = true,
        PushNotifications = false
    },
    Skills = [
        new AgentSkill
        {
            Name = "Activity Search",
            Description = "Find museums, theaters, cultural events, and attractions",
            Examples = [
                "Find me museums to visit",
                "What theaters are available?",
                "Show me cultural events",
                "What attractions do you recommend?"
            ]
        }
    ]
});

// Map OpenAI-compatible endpoints
app.MapOpenAIResponses();
app.MapOpenAIConversations();

app.MapDefaultEndpoints();
app.Run();
