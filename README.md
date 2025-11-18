# City Assistant - Agent Framework Demo

A multi-agent application built with Microsoft Agent Framework, featuring a restaurant recommendation system with orchestrated agents.

## Architecture

The application consists of three main components:

1. **Restaurant Agent** - A specialized agent that can search and recommend restaurants by category or keywords
2. **Orchestrator Agent** - An orchestrator that uses the restaurant agent as a tool via A2A (Agent-to-Agent) communication
3. **Frontend** - A React-based chat interface for interacting with the orchestrator

## Prerequisites

- .NET 10 SDK
- Node.js 18+ and npm
- Azure AI Inference (Foundry) connection
- Azure Cosmos DB instance

## Configuration

### Environment Variables

Create appsettings.Development.json files or set environment variables for:

**Restaurant Agent**:
- `ConnectionStrings__foundry`: Your Azure AI Inference connection string
- `ConnectionStrings__cosmos`: Your Azure Cosmos DB connection string

**Orchestrator Agent**:
- `ConnectionStrings__foundry`: Your Azure AI Inference connection string
- `ConnectionStrings__cosmos`: Your Azure Cosmos DB connection string  
- `services__restaurant-agent__https__0`: URL where restaurant agent is running (e.g., `https://localhost:5196`)

**Frontend**:
- Automatically proxies requests to orchestrator agent (configured in vite.config.ts)

## Running the Application

### Option 1: Run Each Component Separately

1. **Start the Restaurant Agent**:
   ```bash
   cd src/restaurant-agent
   dotnet run
   ```
   The restaurant agent will start on https://localhost:5196 (or the port specified in launchSettings.json)

2. **Start the Orchestrator Agent**:
   ```bash
   cd src/orchestrator-agent
   dotnet run
   ```
   The orchestrator agent will start on https://localhost:5197 (or the port specified in launchSettings.json)

3. **Start the Frontend**:
   ```bash
   cd src/frontend
   npm install  # First time only
   npm run dev
   ```
   The frontend will start on http://localhost:5173

4. Open your browser to http://localhost:5173

### Option 2: Build All Projects

```bash
dotnet build CityAssistant.sln
```

## Features

### Restaurant Agent
- Search restaurants by category (vegetarian, pizza, japanese, mexican, french, indian, steakhouse)
- Search restaurants by keywords
- Get all available restaurants
- A2A endpoint at `/agenta2a`
- OpenAI-compatible endpoints for testing

### Orchestrator Agent
- Orchestrates calls to the restaurant agent
- Maintains conversation history via Cosmos DB
- Custom streaming API endpoint at `/agent/chat/stream`
- Integrates with restaurant agent via A2A protocol

### Frontend
- Clean, modern chat interface
- Streaming responses
- Theme support (light/dark/system)
- Session management with conversation history

## Mock Data

The restaurant agent includes mock data for 11 restaurants across various categories:
- 3 Vegetarian restaurants
- 3 Pizza places
- 5 Other cuisines (Japanese, Mexican, French, Indian, Steakhouse)

All restaurant data is hardcoded in `RestaurantService.cs` and doesn't require external data sources.

## API Endpoints

### Restaurant Agent
- `POST /agenta2a/v1/run` - A2A endpoint for agent communication
- `POST /v1/chat/completions` - OpenAI-compatible chat endpoint
- `GET /health` - Health check endpoint

### Orchestrator Agent
- `POST /agent/chat/stream` - Streaming chat endpoint for frontend
- `GET /health` - Health check endpoint

## Development

### Project Structure

```
src/
├── service-defaults/          # Shared Aspire service configuration
├── restaurant-agent/          # Restaurant recommendation agent
│   ├── Models/               # Data models
│   ├── Services/             # Business logic and storage
│   └── Tools/                # Agent tools/functions
├── orchestrator-agent/       # Orchestrator agent
│   ├── Models/               # API models
│   └── Services/             # Storage services
├── frontend/                 # React frontend
│   └── src/
│       ├── Chat.tsx         # Main chat component
│       └── ...
└── aspire/                   # Aspire orchestration (placeholder)
```

### Building

```bash
# Build all projects
dotnet build

# Build specific project
cd src/restaurant-agent && dotnet build
cd src/orchestrator-agent && dotnet build
```

### Testing the Restaurant Agent Directly

You can test the restaurant agent's A2A endpoint:

```bash
curl -X POST https://localhost:5196/agenta2a/v1/run \
  -H "Content-Type: application/json" \
  -d '{
    "messages": [
      {
        "role": "user",
        "content": "Find me a vegetarian restaurant"
      }
    ]
  }'
```

## Troubleshooting

### Restaurant Agent not connecting
- Ensure the restaurant agent is running and accessible
- Check that `services__restaurant-agent__https__0` environment variable is set correctly in orchestrator
- Verify SSL certificate if using HTTPS in development

### Cosmos DB connection issues
- Verify your Cosmos DB connection string is valid
- Ensure the `conversations` container exists or can be created
- Check that your Azure Cosmos DB firewall rules allow your IP

### Frontend not connecting
- Verify the proxy configuration in `vite.config.ts`
- Check that orchestrator agent is running on the expected port
- Look for CORS issues in browser console

## License

MIT
