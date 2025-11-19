# Implementation Notes

## Overview

This implementation creates a multi-agent system using Microsoft Agent Framework (.NET 10) with two agents:
1. **Restaurant Agent** - Specializes in restaurant recommendations
2. **Orchestrator Agent** - Coordinates restaurant agent via A2A protocol

## What Was Implemented

### 1. Service Defaults (`src/service-defaults/`)

Standard Aspire service defaults providing:
- OpenTelemetry configuration
- Health checks
- Service discovery
- HTTP client resilience

### 2. Restaurant Agent (`src/restaurant-agent/`)

A complete agent implementation with:

**Models:**
- `Restaurant.cs` - Data model for restaurant information

**Services:**
- `RestaurantService.cs` - Mock service with 11 hardcoded restaurants
  - 3 Vegetarian restaurants
  - 3 Pizza places
  - 5 Other cuisines (Japanese, Mexican, French, Indian, Steakhouse)
- `CosmosAgentThreadStore.cs` - Conversation persistence
- `CosmosThreadRepository.cs` - Cosmos DB integration
- `CosmosSystemTextJsonSerializer.cs` - Custom JSON serialization

**Tools:**
- `RestaurantTools.cs` - Three AI functions:
  - `GetAllRestaurants()` - Returns all restaurants
  - `GetRestaurantsByCategory(string category)` - Filters by category
  - `SearchRestaurants(string query)` - Searches by keywords

**Endpoints:**
- `/agenta2a/v1/*` - A2A communication endpoint
- `/v1/chat/completions` - OpenAI-compatible endpoint
- `/health` - Health check

**Features:**
- Memory management with Cosmos DB
- A2A protocol support for inter-agent communication
- OpenAI-compatible responses

### 3. Orchestrator Agent (`src/orchestrator-agent/`)

Coordinates the restaurant agent:

**Models:**
- `AIChatRequest.cs` - Request model for frontend API
- `AIChatCompletionDelta.cs` - Streaming response model

**Services:**
- Same Cosmos persistence services as restaurant agent

**Endpoints:**
- `POST /agent/chat/stream` - Custom streaming endpoint for frontend
- `/health` - Health check

**Features:**
- Connects to restaurant agent via A2A
- Uses restaurant agent as a tool
- Streaming responses to frontend
- Memory management with Cosmos DB

### 4. Frontend Updates (`src/frontend/`)

**Changes:**
- Removed agent selection dropdown (now single orchestrator)
- Updated endpoint from multiple agents to single `/agent/chat/stream`
- Changed branding from "AI Agent Hub" to "City Assistant"
- Updated welcome message
- Simplified vite proxy configuration

### 5. Infrastructure

**Solution Structure:**
```
CityAssistant.sln
├── service-defaults
├── restaurant-agent
└── orchestrator-agent

src/aspire/ (not in solution - single-file apphost)
├── apphost.cs
└── apphost.run.json (launchsettings.json format)
```

**Configuration Files:**
- `apphost.cs` - Single-file Aspire orchestration with #:sdk and #:package directives
- `apphost.run.json` - Launch settings for the Aspire host
- `appsettings.json` - Application settings for each agent
- `.gitignore` - Comprehensive ignore rules
- `README.md` - User documentation

## Technical Details

### Agent Framework Patterns Used

1. **Tool Creation:**
   ```csharp
   public IEnumerable<AIFunction> GetFunctions()
   {
       yield return AIFunctionFactory.Create(MethodName);
   }
   ```

2. **Agent Registration:**
   ```csharp
   builder.AddAIAgent("agent-name", (sp, key) =>
   {
       return chatClient.CreateAIAgent(
           instructions: "...",
           name: key,
           tools: [.. toolFunctions]
       );
   }).WithThreadStore(...);
   ```

3. **A2A Connection:**
   ```csharp
   var cardResolver = new A2ACardResolver(
       baseAddress,
       httpClient,
       agentCardPath: "/agenta2a/v1/card"
   );
   var remoteAgent = cardResolver.GetAIAgentAsync().Result;
   ```

4. **Thread Store:**
   - Extends `AgentThreadStore` base class
   - Implements `SaveThreadAsync` and `GetThreadAsync`
   - Uses Cosmos DB with `JsonElement` serialization

### API Flow

```
User → Frontend → Orchestrator Agent → Restaurant Agent → Mock Data
      ← Streaming ← Streaming        ← A2A Response  ← Returns
```

### Cosmos DB Schema

Each thread is stored as:
```json
{
  "id": "agentId:conversationId",
  "conversationId": "agentId:conversationId",
  "serializedThread": "{ ... }",
  "lastUpdated": "2024-01-01T00:00:00Z",
  "ttl": -1
}
```

## Package Versions

All Agent Framework packages use wildcards for preview versions:
- `Microsoft.Agents.AI.*` - Version: `1.0.0-preview.*`
- `Microsoft.Agents.AI.Hosting.*` - Version: `1.0.0-preview.*`
- `Microsoft.Agents.AI.A2A` - Version: `1.0.0-preview.*`
- `Aspire.*` - Version: `9.0.0` or `13.0.0*`

## Known Limitations

1. **Aspire Orchestration:** Uses single-file C# script-based apphost (apphost.cs) with:
   - #:sdk and #:package directives for dependencies
   - No csproj file needed
   - Requires .NET 10 SDK with Aspire support
   - Cosmos DB emulator for local development

2. **Mock Data Only:** Restaurant data is hardcoded in memory
   - No external database
   - Not persisted across restarts
   - Easy to extend with real data source

3. **Development Configuration:** Requires manual configuration of:
   - Azure AI Inference connection string
   - Azure Cosmos DB connection string
   - Agent endpoint URLs

## Testing Checklist

Before deploying, verify:

- [ ] Restaurant agent starts successfully
- [ ] Restaurant agent A2A endpoint responds
- [ ] Orchestrator agent connects to restaurant agent
- [ ] Orchestrator agent streaming endpoint works
- [ ] Frontend connects to orchestrator
- [ ] Conversation history persists in Cosmos
- [ ] All tools are invoked correctly

## Future Enhancements

Potential improvements:
1. Add more specialized agents (events, transportation, etc.)
2. Implement real restaurant data source
3. Add authentication and authorization
4. Implement rate limiting
5. Add logging and monitoring
6. Create Docker containers
7. Add integration tests

## Build Warnings

Non-critical warnings present:
- `NU1902: Package 'OpenTelemetry.Api' 1.10.0 has a known moderate severity vulnerability`
- `NU1902: Package 'KubernetesClient' 15.0.1 has a known moderate severity vulnerability`

These are transitive dependencies from Aspire packages and will be resolved in future package updates.
