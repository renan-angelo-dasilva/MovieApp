# Movie Catalog API - Usage Examples

This document provides practical examples for using the Movie Catalog API.

## Base URL
```
https://localhost:{port}
```

## API Examples

### 1. Get All Movies

**Request:**
```bash
curl -X GET "https://localhost:7xxx/api/movies" -H "accept: application/json"
```

**Response:**
```json
[
  {
    "id": 1,
    "title": "The Shawshank Redemption",
    "description": "Two imprisoned men bond over a number of years...",
    "category": "Drama",
    "releaseYear": 1994,
    "rating": 9.3,
    "minimumAge": 13,
    "director": "Frank Darabont",
    "cast": ["Tim Robbins", "Morgan Freeman"],
    "durationMinutes": 142,
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
]
```

---

### 2. Get Movie by ID

**Request:**
```bash
curl -X GET "https://localhost:7xxx/api/movies/1" -H "accept: application/json"
```

**Response:**
```json
{
  "id": 1,
  "title": "The Shawshank Redemption",
  "category": "Drama",
  "rating": 9.3,
  ...
}
```

---

### 3. Get Movies by Category

**Request:**
```bash
curl -X GET "https://localhost:7xxx/api/movies/category/Action" -H "accept: application/json"
```

**Response:**
```json
[
  {
    "id": 2,
    "title": "The Dark Knight",
    "category": "Action",
    ...
  }
]
```

**Valid Categories:**
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

---

### 4. Stream Movies by Category (Server-Sent Events)

**Request:**
```bash
curl -N -X GET "https://localhost:7xxx/api/movies/category/Action/stream" \
  -H "Accept: text/event-stream"
```

**Response (SSE Stream):**
```
data: {"id":2,"title":"The Dark Knight","category":"Action",...}

data: {"id":6,"title":"Mad Max: Fury Road","category":"Action",...}
```

**JavaScript Example:**
```javascript
const eventSource = new EventSource('https://localhost:7xxx/api/movies/category/Action/stream');

eventSource.onmessage = (event) => {
  const movie = JSON.parse(event.data);
  console.log('Received movie:', movie);
};

eventSource.onerror = (error) => {
  console.error('Error:', error);
  eventSource.close();
};
```

---

### 5. Create a Movie

**Request:**
```bash
curl -X POST "https://localhost:7xxx/api/movies" \
  -H "Content-Type: application/json" \
  -H "accept: application/json" \
  -d '{
    "title": "The Matrix",
    "description": "A computer hacker learns from mysterious rebels about the true nature of his reality.",
    "category": "Sci-Fi",
    "releaseYear": 1999,
    "rating": 8.7,
    "minimumAge": 13,
    "director": "The Wachowskis",
    "cast": ["Keanu Reeves", "Laurence Fishburne", "Carrie-Anne Moss"],
    "durationMinutes": 136
  }'
```

**Response:**
```json
{
  "id": 6,
  "title": "The Matrix",
  "description": "A computer hacker learns from mysterious rebels...",
  "category": "Sci-Fi",
  "releaseYear": 1999,
  "rating": 8.7,
  "minimumAge": 13,
  "director": "The Wachowskis",
  "cast": ["Keanu Reeves", "Laurence Fishburne", "Carrie-Anne Moss"],
  "durationMinutes": 136,
  "createdAt": "2024-01-01T12:00:00Z",
  "updatedAt": "2024-01-01T12:00:00Z"
}
```

---

### 6. Update a Movie

**Request:**
```bash
curl -X PUT "https://localhost:7xxx/api/movies/1" \
  -H "Content-Type: application/json" \
  -H "accept: application/json" \
  -d '{
    "rating": 9.5,
    "description": "Updated description of the movie"
  }'
```

**Note:** All fields are optional. Only provided fields will be updated.

**Response:**
```json
{
  "id": 1,
  "title": "The Shawshank Redemption",
  "description": "Updated description of the movie",
  "rating": 9.5,
  ...
}
```

---

### 7. Delete a Movie

**Request:**
```bash
curl -X DELETE "https://localhost:7xxx/api/movies/1" \
  -H "accept: application/json"
```

**Response:** 204 No Content

---

### 8. Get AI-Powered Movie Recommendations

**Request:**
```bash
curl -X POST "https://localhost:7xxx/api/movies/recommendations" \
  -H "Content-Type: application/json" \
  -H "accept: application/json" \
  -d '{
    "userAge": 15
  }'
```

**Response:**
```json
{
  "recommendedMovies": [
    {
      "id": 3,
      "title": "Toy Story",
      "category": "Animation",
      "rating": 8.3,
      "minimumAge": 0,
      ...
    },
    {
      "id": 1,
      "title": "The Shawshank Redemption",
      "category": "Drama",
      "rating": 9.3,
      "minimumAge": 13,
      ...
    },
    {
      "id": 4,
      "title": "Inception",
      "category": "Sci-Fi",
      "rating": 8.8,
      "minimumAge": 13,
      ...
    }
  ],
  "reasoning": "For a 15-year-old viewer, I've selected a diverse mix of highly-rated films that are age-appropriate. Toy Story offers timeless animation, The Shawshank Redemption provides powerful drama with important life lessons, and Inception delivers mind-bending science fiction that will engage a teenage audience."
}
```

---

## Error Responses

### Validation Error (400 Bad Request)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": ["Title is required"],
    "Rating": ["Rating must be between 0.0 and 10.0"]
  }
}
```

### Not Found (404)
```json
{
  "message": "Movie not found"
}
```

---

## PowerShell Examples

### Get All Movies
```powershell
Invoke-RestMethod -Uri "https://localhost:7xxx/api/movies" -Method Get
```

### Create a Movie
```powershell
$movie = @{
    title = "The Godfather"
    description = "The aging patriarch of an organized crime dynasty transfers control to his reluctant son."
    category = "Drama"
    releaseYear = 1972
    rating = 9.2
    minimumAge = 17
    director = "Francis Ford Coppola"
    cast = @("Marlon Brando", "Al Pacino")
    durationMinutes = 175
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:7xxx/api/movies" `
  -Method Post `
  -ContentType "application/json" `
  -Body $movie
```

### Get Recommendations
```powershell
$request = @{ userAge = 12 } | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:7xxx/api/movies/recommendations" `
  -Method Post `
  -ContentType "application/json" `
  -Body $request
```

---

## Testing with Swagger

1. Navigate to `https://localhost:7xxx` (Swagger UI opens at root)
2. Explore all endpoints with interactive documentation
3. Try out requests directly from the browser
4. View request/response schemas

---

## Configuring OpenAI API Key

### Option 1: User Secrets (Recommended for Development)
```bash
dotnet user-secrets set "OpenAI:ApiKey" "sk-your-api-key-here"
```

### Option 2: Environment Variable
```bash
# Windows
$env:OpenAI__ApiKey = "sk-your-api-key-here"

# Linux/Mac
export OpenAI__ApiKey="sk-your-api-key-here"
```

### Option 3: appsettings.json (Not recommended for production)
```json
{
  "OpenAI": {
    "ApiKey": "sk-your-api-key-here",
    "Model": "gpt-3.5-turbo"
  }
}
```

---

## Health Check

**Request:**
```bash
curl -X GET "https://localhost:7xxx/health"
```

**Response:**
```json
{
  "status": "healthy",
  "timestamp": "2024-01-01T12:00:00Z",
  "service": "Movie Catalog API"
}
```

---

## Rate Limiting & Performance Tips

1. **Caching**: Consider implementing response caching for frequently accessed data
2. **Pagination**: For large datasets, implement pagination (future enhancement)
3. **Batch Operations**: Use bulk operations when creating/updating multiple movies
4. **Streaming**: Use the `/stream` endpoint for real-time data needs
5. **Connection Pooling**: The app uses connection pooling by default

---

## Common HTTP Status Codes

- `200 OK` - Successful GET/PUT request
- `201 Created` - Successful POST request
- `204 No Content` - Successful DELETE request
- `400 Bad Request` - Validation errors
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

---

## Integration Examples

### C# HttpClient
```csharp
using var client = new HttpClient { BaseAddress = new Uri("https://localhost:7xxx") };

// Get all movies
var movies = await client.GetFromJsonAsync<List<MovieResponseDto>>("/api/movies");

// Create a movie
var createDto = new CreateMovieDto("Title", "Description", "Action", 2024, 8.0, 13, "Director", null, 120);
var response = await client.PostAsJsonAsync("/api/movies", createDto);
var newMovie = await response.Content.ReadFromJsonAsync<MovieResponseDto>();
```

### Python
```python
import requests

# Get recommendations
response = requests.post(
    'https://localhost:7xxx/api/movies/recommendations',
    json={'userAge': 16},
    verify=False  # For development with self-signed cert
)
recommendations = response.json()
print(recommendations['reasoning'])
```

---

For more information, see the main [README.md](README.md) file.
