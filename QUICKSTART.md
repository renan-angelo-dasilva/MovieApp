# Quick Start Guide - Movie Catalog API

Get up and running with the Movie Catalog API in minutes!

## Prerequisites

- ? .NET 8 SDK installed
- ? Visual Studio 2022 or VS Code (optional)
- ? OpenAI API key (optional, for AI recommendations)

## 5-Minute Quick Start

### Step 1: Clone and Navigate
```bash
cd MovieApp
```

### Step 2: Restore Dependencies
```bash
dotnet restore
```

### Step 3: Run the Application
```bash
dotnet run
```

The API will start and display:
```
Now listening on: https://localhost:7053
Now listening on: http://localhost:5066
```

### Step 4: Open Swagger UI
Open your browser to: **https://localhost:7053**

You'll see the interactive Swagger documentation!

## First API Calls

### 1. Get All Movies (Pre-seeded)
```bash
curl https://localhost:7053/api/movies
```

### 2. Get Movies by Category
```bash
curl https://localhost:7053/api/movies/category/Action
```

### 3. Create Your First Movie
```bash
curl -X POST https://localhost:7053/api/movies \
  -H "Content-Type: application/json" \
  -d '{
    "title": "My Awesome Movie",
    "description": "An incredible adventure",
    "category": "Action",
    "releaseYear": 2024,
    "rating": 8.5,
    "minimumAge": 13,
    "durationMinutes": 120
  }'
```

### 4. Try AI Recommendations (Optional)
First, configure your OpenAI API key:
```bash
dotnet user-secrets set "OpenAI:ApiKey" "sk-your-key-here"
```

Then request recommendations:
```bash
curl -X POST https://localhost:7053/api/movies/recommendations \
  -H "Content-Type: application/json" \
  -d '{"userAge": 15}'
```

## What's Included?

### Pre-seeded Movies
The application comes with 5 sample movies:
- The Shawshank Redemption (Drama) - 9.3?
- The Dark Knight (Action) - 9.0?
- Toy Story (Animation) - 8.3?
- Inception (Sci-Fi) - 8.8?
- The Conjuring (Horror) - 7.5?

### Available Categories
- Action
- Comedy
- Drama
- Horror
- Sci-Fi
- Romance
- Thriller
- Documentary
- Animation
- Adventure

## Key Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/movies` | Get all movies |
| GET | `/api/movies/{id}` | Get movie by ID |
| GET | `/api/movies/category/{category}` | Get movies by category |
| GET | `/api/movies/category/{category}/stream` | Stream movies (SSE) |
| POST | `/api/movies` | Create a movie |
| PUT | `/api/movies/{id}` | Update a movie |
| DELETE | `/api/movies/{id}` | Delete a movie |
| POST | `/api/movies/recommendations` | Get AI recommendations |
| GET | `/health` | Health check |

## Using Swagger UI

1. **Navigate to root**: https://localhost:7053
2. **Expand an endpoint**: Click on any endpoint
3. **Try it out**: Click "Try it out" button
4. **Edit request**: Modify the request body if needed
5. **Execute**: Click "Execute" button
6. **View response**: See the response code and body

## Testing the Streaming Endpoint

The `/stream` endpoint demonstrates **System.Threading.Channels**:

```bash
# Using curl
curl -N https://localhost:7053/api/movies/category/Action/stream
```

You'll see Server-Sent Events streaming in real-time:
```
data: {"id":2,"title":"The Dark Knight",...}

data: {"id":6,"title":"Another Action Movie",...}
```

## Configuration Options

### appsettings.json
```json
{
  "OpenAI": {
    "ApiKey": "",           // Your OpenAI API key
    "Model": "gpt-3.5-turbo" // Or "gpt-4"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### User Secrets (Recommended)
```bash
# Set OpenAI API key
dotnet user-secrets set "OpenAI:ApiKey" "sk-your-key"

# View all secrets
dotnet user-secrets list
```

### Environment Variables
```bash
# Windows
$env:OpenAI__ApiKey = "sk-your-key"

# Linux/Mac
export OpenAI__ApiKey="sk-your-key"
```

## Running in Development Mode

### Using .NET CLI
```bash
dotnet run --environment Development
```

### Using Visual Studio
1. Open `MovieApp.sln`
2. Press F5 to run with debugging
3. Swagger UI opens automatically

### Using VS Code
1. Open folder in VS Code
2. Press F5 (Run and Debug)
3. Select ".NET Core Launch"

## Database Information

The application uses an **in-memory database** for demo purposes:
- ? **Pros**: No setup required, fast, perfect for demos
- ?? **Cons**: Data is lost when app restarts

To persist data, update `Program.cs`:
```csharp
// Replace this:
options.UseInMemoryDatabase("MovieDb")

// With this (SQL Server example):
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
```

## Troubleshooting

### Port Already in Use
Change the port in `Properties/launchSettings.json`:
```json
"applicationUrl": "https://localhost:7053;http://localhost:5066"
```

### SSL Certificate Issues
Trust the development certificate:
```bash
dotnet dev-certs https --trust
```

### AI Recommendations Not Working
The app works without OpenAI! It will use fallback logic-based recommendations if no API key is configured.

To enable AI:
1. Get API key from https://platform.openai.com/api-keys
2. Configure using User Secrets (recommended)
3. Restart the application

### Build Errors
Ensure .NET 8 SDK is installed:
```bash
dotnet --version  # Should be 8.0.x or higher
```

Clean and rebuild:
```bash
dotnet clean
dotnet restore
dotnet build
```

## Next Steps

1. **Explore the API**: Try all endpoints in Swagger UI
2. **Read Documentation**: Check `README.md` for detailed info
3. **View Examples**: See `API_EXAMPLES.md` for more examples
4. **Architecture**: Read `ARCHITECTURE.md` to understand the design
5. **Customize**: Add your own movies and categories

## Performance Features Showcased

? **Async/Await**: All operations are asynchronous
? **Channels**: Real-time streaming with `System.Threading.Channels`
? **No-Tracking Queries**: Optimized read operations
? **Validation**: Input validation with FluentValidation
? **AI Integration**: Semantic Kernel for intelligent recommendations
? **Best Practices**: Clean architecture, SOLID principles

## Common Use Cases

### 1. Movie Discovery App
Use category filtering and recommendations

### 2. Admin Dashboard
Full CRUD operations for catalog management

### 3. Streaming Service
Real-time updates with SSE streaming

### 4. Recommendation Engine
AI-powered suggestions based on user profile

## Development Tips

### Watch Mode
Auto-reload on file changes:
```bash
dotnet watch run
```

### Hot Reload
Enabled by default in .NET 8 - just save your changes!

### Logging
Check console output for detailed logs:
```
info: MovieApp.Services.MovieService[0]
      Created movie 6: My Awesome Movie
```

## API Testing Tools

### Swagger UI (Built-in)
? Interactive documentation
? Try endpoints directly
? View request/response schemas

### Postman
1. Import OpenAPI spec from `/swagger/v1/swagger.json`
2. Create requests
3. Save collections

### curl (Command Line)
See `API_EXAMPLES.md` for complete curl examples

### Thunder Client (VS Code)
1. Install Thunder Client extension
2. Import API collection
3. Test endpoints

## Getting Help

- ?? **README.md**: Comprehensive documentation
- ?? **API_EXAMPLES.md**: Request/response examples
- ??? **ARCHITECTURE.md**: System design details
- ?? **GitHub Issues**: Report bugs or request features

## What Makes This Special?

### 1. Channels Implementation ?
Demonstrates **System.Threading.Channels** for high-performance streaming

### 2. Semantic Kernel Integration ?
AI-powered recommendations using Microsoft's AI orchestration framework

### 3. Best Practices ?
- Clean Architecture
- SOLID Principles
- Async/Await throughout
- Proper error handling
- Input validation
- Swagger documentation

### 4. Performance Optimized ?
- No-tracking queries
- Connection pooling
- Efficient data mapping
- Minimal API overhead

### 5. Production-Ready Structure ?
- Layered architecture
- Dependency injection
- Configuration management
- Health checks
- Logging

## Production Checklist

Before deploying to production:

- [ ] Replace in-memory database with persistent storage
- [ ] Add authentication and authorization
- [ ] Implement rate limiting
- [ ] Add caching layer (Redis)
- [ ] Configure production logging (App Insights)
- [ ] Set up monitoring and alerts
- [ ] Implement backup strategy
- [ ] Configure HTTPS properly
- [ ] Review and update CORS policy
- [ ] Add comprehensive tests
- [ ] Document API versioning strategy
- [ ] Set up CI/CD pipeline

---

**You're all set! ??**

Start exploring the API with Swagger UI at: **https://localhost:7053**

Enjoy building with the Movie Catalog API!
