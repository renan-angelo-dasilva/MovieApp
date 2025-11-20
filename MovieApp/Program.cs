using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MovieApp.Components.Data;
using MovieApp.Components.DTOs;
using MovieApp.Components.Services;
using MovieApp.Components.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Movie Catalog API",
        Version = "v1",
        Description = "Movie Catalog API with multi-agent AI recommendations"
    });
    
    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseInMemoryDatabase("MovieDb"));

// Register KernelFactory (scoped to keep per-request kernel instances)
builder.Services.AddScoped<IKernelFactory, KernelFactory>();

// Movie services now from components project
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IMovieRecommendationService, MovieRecommendationService>();

// Validators (remain same)
builder.Services.AddScoped<IValidator<CreateMovieDto>, CreateMovieDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateMovieDto>, UpdateMovieDtoValidator>();

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie Catalog API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MovieDbContext>();
    await context.Database.EnsureCreatedAsync();
}

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck")
   .WithTags("Health")
   .ExcludeFromDescription();

app.Run();
