# Movie Catalog API

A .NET 8 Web API for managing movies with AI-powered recommendations using Semantic Kernel and Azure OpenAI.

## Features

- **CRUD Operations**: Create, Read, Update, Delete movies
- **Category Filtering**: Get movies by category
- **Streaming**: Real-time streaming using `System.Threading.Channels` and Server-Sent Events
- **AI Recommendations**: Age-based movie suggestions powered by Semantic Kernel + Azure OpenAI
- **Validation**: Input validation with FluentValidation
- **Swagger**: Interactive API documentation

## Quick Start

```bash
cd MovieApp
dotnet restore
dotnet run
```

Open **https://localhost:7053** for Swagger UI

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/movies` | Get all movies |
| GET | `/api/movies/{id}` | Get movie by ID |
| GET | `/api/movies/category/{category}` | Get movies by category |
| GET | `/api/movies/category/{category}/stream` | Stream movies (SSE) |
| POST | `/api/movies` | Create movie |
| PUT | `/api/movies/{id}` | Update movie |
| DELETE | `/api/movies/{id}` | Delete movie |
| POST | `/api/movies/recommendations` | Get AI recommendations |

## Project Structure

```
MovieApp/
??? Models/           # Movie entity
??? DTOs/             # Request/Response DTOs
??? Validators/       # FluentValidation rules
??? Data/             # EF Core DbContext
??? Services/         # Business logic
??? Endpoints/        # API endpoints
??? Program.cs        # App configuration
```

## Configuration

### Azure OpenAI Setup

Configure your Azure OpenAI credentials using User Secrets (recommended):

```bash
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://your-resource.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:ApiKey" "your-api-key"
dotnet user-secrets set "AzureOpenAI:DeploymentName" "gpt-35-turbo"
```

Or use `appsettings.json` (not recommended for production):

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "your-api-key",
    "DeploymentName": "gpt-35-turbo"
  }
}
```

The API works without Azure OpenAI - it uses fallback logic-based recommendations.

## Technology Stack

- .NET 8
- ASP.NET Core Minimal APIs
- Entity Framework Core (In-Memory)
- Semantic Kernel with Azure OpenAI
- FluentValidation
- Swagger/OpenAPI
- System.Threading.Channels

## Examples

### Create a Movie
```bash
curl -X POST https://localhost:7053/api/movies \
  -H "Content-Type: application/json" \
  -d '{
    "title": "The Matrix",
    "description": "A hacker learns the true nature of reality",
    "category": "Sci-Fi",
    "releaseYear": 1999,
    "rating": 8.7,
    "minimumAge": 13,
    "durationMinutes": 136
  }'
```

### Get AI Recommendations
```bash
curl -X POST https://localhost:7053/api/movies/recommendations \
  -H "Content-Type: application/json" \
  -d '{"userAge": 15}'
```

### Stream Movies (Server-Sent Events)
```bash
curl -N https://localhost:7053/api/movies/category/Action/stream
```

## Key Features

### Channel-Based Streaming
Uses `System.Threading.Channels` for efficient Server-Sent Events streaming.

### AI Recommendations with Azure OpenAI
Semantic Kernel integration provides intelligent movie suggestions based on user age with fallback to logic-based recommendations.

### Performance
- Async/await throughout
- No-tracking queries for reads
- Database indexes on frequently queried columns
- Efficient DTO mapping

## License

MIT
