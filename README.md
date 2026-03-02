# Movies REST API - Course Project

## Project Overview

This is a comprehensive REST API course project designed to demonstrate professional-grade API development practices using .NET. The API manages a movies database with full CRUD (Create, Read, Update, Delete) operations, JWT authentication, role-based authorization, and data validation.

The project is built following clean architecture principles with clear separation of concerns, making it maintainable, scalable, and easily extendable.

---

## Table of Contents
- [Architecture Overview](#architecture-overview)
- [Technology Stack](#technology-stack)
- [Design Patterns](#design-patterns)
- [Folder Structure](#folder-structure)
- [Key Features](#key-features)
- [Database Schema](#database-schema)
- [API Endpoints](#api-endpoints)
- [Authentication & Authorization](#authentication--authorization)
- [Validation & Error Handling](#validation--error-handling)
- [Future Database Migration Guide](#future-database-migration-guide)

---

## Architecture Overview

### Layered Architecture (N-Tier Architecture)

The project follows a **Clean Architecture** approach with clear layering:

```
┌─────────────────────────────────────────┐
│      API Layer (Movies.Api)             │
│  (Controllers, Endpoints, Mapping)      │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│   Application Layer (Movies.Application)│
│  (Services, Validators, Repositories)   │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│  Data Layer (Database, Repositories)    │
│  (Dapper ORM, Connection Factory)       │
└─────────────────────────────────────────┘
```

### Layer Responsibilities

1. **API Layer (Movies.Api)**
   - HTTP request/response handling
   - Controller routing and HTTP methods
   - Request/Response contract mapping
   - Authentication & Authorization
   - Error/Validation middleware

2. **Application Layer (Movies.Application)**
   - Business logic implementation
   - Service interfaces & implementations
   - Data validation (FluentValidation)
   - Repository pattern abstraction
   - Database initialization

3. **Contracts Layer (Movies.Contracts)**
   - Shared DTOs (Data Transfer Objects)
   - Request/Response models
   - Decouples API from internal models

4. **Data Layer**
   - Database connection management
   - SQL query execution via Dapper
   - Transaction handling

---

## Technology Stack

### Core Framework
- **.NET 9** - Latest LTS framework
- **ASP.NET Core** - Web API framework
- **C# 13** - Modern language features (records, required keyword, generated regex)

### Authentication & Authorization
- **JWT (JSON Web Tokens)** - Token-based authentication
- **Microsoft.AspNetCore.Authentication.JwtBearer** - JWT validation

### Data Access & ORM
- **Dapper** - Lightweight ORM for SQL execution
- **Npgsql** - PostgreSQL database driver
- **PostgreSQL** - Relational database

### Validation
- **FluentValidation** - Fluent API for data validation rules

### API Documentation
- **OpenAPI/Swagger** - API documentation and exploration
- **Scalar** - Modern API documentation UI

### Testing & Development
- **.http files** - REST client for testing endpoints
- **Rider IDE** - JetBrains IDE for development

---

## Design Patterns

### 1. **Repository Pattern**
   - **Purpose**: Abstracts data access logic
   - **Location**: `Movies.Application/Repositories/`
   - **Interface**: `IMoviesRepository.cs`
   - **Implementation**: `MovieRepository.cs` (uses Dapper)
   - **Benefit**: Easy to swap PostgreSQL with MongoDB, SQL Server, etc. without changing business logic

### 2. **Service Layer Pattern**
   - **Purpose**: Encapsulates business logic
   - **Location**: `Movies.Application/Services/`
   - **Interface**: `IMovieService.cs`
   - **Implementation**: `MovieService.cs`
   - **Responsibility**: 
     - Orchestrates repository calls
     - Applies business rules
     - Validates data before persistence

### 3. **Dependency Injection (DI)**
   - **Container**: Microsoft.Extensions.DependencyInjection
   - **Lifecycle**: Singleton pattern for repositories and services
   - **Configuration**: `ApplicationServiceCollectionExtensions.cs`
   - **Benefit**: Loose coupling, testability, flexibility

### 4. **Data Transfer Object (DTO) Pattern**
   - **Contracts Layer**: `Movies.Contracts/`
   - **Purpose**: Decouples API contracts from internal models
   - **Mapping**: `ContractMapping.cs` - Manual mapping using extension methods
   - **Types**:
     - `CreateMovieRequest` - Request model
     - `UpdateMovieRequest` - Request model
     - `MovieResponse` - Response model
     - `ValidationFailureResponse` - Error response

### 5. **Middleware Pattern**
   - **Purpose**: Cross-cutting concerns
   - **Example**: `ValidationMappingMiddleware.cs`
   - **Function**: Catches `ValidationException` and transforms to standardized response

### 6. **Factory Pattern**
   - **Purpose**: Create database connections
   - **Implementation**: `IDbConnectionFactory` / `NpgsqlConnectionFactory`
   - **Benefit**: Encapsulates connection creation logic

### 7. **Specification Pattern** (Implicit)
   - Validators are used to define specifications for valid entities
   - `MovieValidator.cs` defines all validation rules

### 8. **Async/Await Pattern**
   - All operations are asynchronous
   - `CancellationToken` support throughout
   - Improves scalability and resource management

---

## Folder Structure

```
RestApiCourse/
├── Movies.Api/                          # API Layer
│   ├── Controllers/
│   │   └── MoviesController.cs         # HTTP endpoints
│   ├── Features/
│   │   └── Constants/
│   │       └── AuthConstants.cs        # Authorization constants
│   ├── Mapping/
│   │   ├── ContractMapping.cs          # Manual mapping logic
│   │   └── ValidationMappingMiddleware.cs  # Exception handling
│   ├── ApiEndpoints/
│   │   └── ApiEndpoints.cs             # Centralized endpoint routing
│   ├── Properties/
│   │   └── launchSettings.json         # Launch configuration
│   ├── Program.cs                       # DI configuration & middleware setup
│   ├── appsettings.json                # Configuration
│   ├── appsettings.Development.json    # Development config
│   └── Movies.Api.csproj
│
├── Movies.Application/                  # Business Logic Layer
│   ├── Services/
│   │   ├── IMovieService.cs            # Service contract
│   │   └── MovieService.cs             # Service implementation
│   ├── Repositories/
│   │   ├── IMoviesRepository.cs        # Repository contract
│   │   └── MovieRepository.cs          # Repository implementation (Dapper)
│   ├── Validators/
│   │   └── MovieValidator.cs           # FluentValidation rules
│   ├── Models/
│   │   └── Movie.cs                    # Domain model
│   ├── Database/
│   │   ├── IDbConnectionFactory.cs     # Connection abstraction
│   │   ├── DbConnectionFactory.cs      # PostgreSQL implementation
│   │   └── DbInitializer.cs            # Database schema initialization
│   ├── Extensions/
│   │   └── ApplicationServiceCollectionExtensions.cs  # DI registration
│   ├── AssemblyMarker/
│   │   └── IApplicationMarker.cs       # Marker for validator scanning
│   ├── docker-compose.yml              # PostgreSQL container
│   └── Movies.Application.csproj
│
├── Movies.Contracts/                    # Contracts Layer
│   ├── Requests/
│   │   ├── CreateMovieRequest.cs       # Create request DTO
│   │   └── UpdateMovieRequest.cs       # Update request DTO
│   ├── Responses/
│   │   ├── MovieResponse.cs            # Single movie response
│   │   ├── MoviesResponse.cs           # Multiple movies response
│   │   └── ValidationFailureResponse.cs # Error response
│   └── Movies.Contracts.csproj
│
├── Helper/                              # JWT Token Generation Helper
│   ├── Controllers/
│   │   └── IdentityController.cs       # Token generation endpoint
│   ├── Program.cs
│   └── appsettings.json
│
├── RestApiCourse.sln                    # Solution file
└── README.md                            # This file
```

### Key Directory Purposes

| Directory | Purpose |
|-----------|---------|
| `Controllers/` | HTTP request handlers |
| `Services/` | Business logic execution |
| `Repositories/` | Data access abstraction |
| `Models/` | Domain entities |
| `Validators/` | Data validation rules |
| `Mapping/` | DTO ↔ Model transformations |
| `Database/` | DB connection & initialization |
| `Contracts/` | API request/response schemas |
| `Extensions/` | Dependency injection setup |

---

## Key Features

### 1. **CRUD Operations**
- ✅ Create movies with validation
- ✅ Read movies by ID or slug
- ✅ Read all movies
- ✅ Update movie details
- ✅ Delete movies

### 2. **Authentication & Authorization**
- ✅ JWT token-based authentication
- ✅ Role-based access control (RBAC)
- ✅ Admin policy for delete operations
- ✅ Trusted member policy for create/update

### 3. **Data Validation**
- ✅ Fluent validation framework
- ✅ Automatic validation error responses
- ✅ Property-level error messages
- ✅ Custom async validators

### 4. **Slug Generation**
- ✅ Auto-generated SEO-friendly slugs
- ✅ Format: `{title}-{year}`
- ✅ Regex-based character sanitization

### 5. **Database Transactions**
- ✅ Multi-table transactions (movies + genres)
- ✅ Automatic initialization on startup
- ✅ Cancellation token support

### 6. **API Documentation**
- ✅ OpenAPI/Swagger UI
- ✅ Scalar API reference
- ✅ Endpoint routing constants

---

## Database Schema

### Movies Table
```sql
CREATE TABLE movies (
    id UUID PRIMARY KEY,
    slug VARCHAR(255) UNIQUE NOT NULL,
    title VARCHAR(100) NOT NULL,
    yearofrelease INT NOT NULL
);
```

### Genres Table
```sql
CREATE TABLE genres (
    movieid UUID FOREIGN KEY REFERENCES movies(id),
    name VARCHAR(100) NOT NULL
);
```

### Design Notes
- **One-to-Many**: One movie has multiple genres
- **Composite Key**: (movieId, name) for genres
- **UUID**: Using V7 UUIDs for better database performance
- **Slug Index**: Unique constraint for slug-based lookups

---

## API Endpoints

### Base URL
```
https://localhost:{port}/api/movies
```

### Endpoints

#### 1. Create Movie
```
POST /api/movies
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Inception",
  "yearOfRelease": 2010,
  "genres": ["Sci-Fi", "Thriller", "Adventure"]
}

Response: 201 Created
Location: /api/movies/{id}
```

#### 2. Get Movie by ID or Slug
```
GET /api/movies/{idOrSlug}

Examples:
- GET /api/movies/550e8400-e29b-41d4-a716-446655440000
- GET /api/movies/inception-2010

Response: 200 OK
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Inception",
  "slug": "inception-2010",
  "yearOfRelease": 2010,
  "genres": ["sci-fi", "thriller", "adventure"]
}
```

#### 3. Get All Movies
```
GET /api/movies

Response: 200 OK
{
  "items": [
    { movie object 1 },
    { movie object 2 }
  ]
}
```

#### 4. Update Movie
```
PUT /api/movies/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Inception Remastered",
  "yearOfRelease": 2010,
  "genres": ["Sci-Fi"]
}

Response: 200 OK
```

#### 5. Delete Movie
```
DELETE /api/movies/{id}
Authorization: Bearer {token} (Admin only)

Response: 204 No Content
```

---

## Authentication & Authorization

### JWT Token Structure
```json
{
  "sub": "user-id",
  "admin": "true|false",
  "trusted_member": "true|false",
  "iat": 1234567890,
  "exp": 1234571490
}
```

### Authorization Policies

| Policy | Required Claims | Operations |
|--------|-----------------|------------|
| `None` | - | GET endpoints |
| `Trusted` | `admin: true` OR `trusted_member: true` | POST, PUT |
| `Admin` | `admin: true` | DELETE |

### Configuration
```csharp
// Program.cs
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", p => p.RequireClaim("admin", "true"))
    .AddPolicy("Trusted", p => p.RequireAssertion(/* custom logic */));
```

### Token Generation
Use the **Helper** project to generate JWT tokens for testing:
```
POST /token
{
  "username": "user",
  "password": "password",
  "isAdmin": true
}
```

---

## Validation & Error Handling

### Validation Framework
- **FluentValidation** for rule-based validation
- **AutoRegister** validators via assembly scanning
- **ValidationMappingMiddleware** catches exceptions

### Validation Rules (MovieValidator)
```csharp
RuleFor(m => m.Title)
    .NotEmpty()
    .MaximumLength(100);

RuleFor(m => m.YearOfRelease)
    .LessThanOrEqualTo(DateTime.UtcNow.Year);

RuleForEach(m => m.Genres)
    .NotEmpty();
```

### Error Response Format
```json
HTTP 400 Bad Request
{
  "errors": [
    {
      "propertyName": "Title",
      "message": "Title is required"
    },
    {
      "propertyName": "YearOfRelease",
      "message": "Year cannot be in the future"
    }
  ]
}
```

### Exception Handling
- **ValidationException** → 400 Bad Request
- **Not Found** → 404 Not Found
- **Unauthorized** → 401 Unauthorized
- **Forbidden** → 403 Forbidden

---

## Future Database Migration Guide

### Why This Architecture Supports Easy Migration

The project uses **Repository Pattern** and **Dependency Injection** to decouple business logic from data access. This means you can change the database with **minimal changes**:

### Steps to Change Database

#### 1. **Update Connection Factory** (`Movies.Application/Database/`)

**Before (PostgreSQL):**
```csharp
public class NpgsqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
    {
        var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(token);
        return connection;
    }
}
```

**After (SQL Server):**
```csharp
public class SqlServerConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
    {
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(token);
        return connection;
    }
}
```

#### 2. **Update Repository Implementation** (`Movies.Application/Repositories/MovieRepository.cs`)

Only SQL queries change. The interface `IMoviesRepository` remains identical:

```csharp
// Example: Switch from PostgreSQL to MongoDB
public class MongoMovieRepository : IMoviesRepository
{
    private readonly IMongoCollection<Movie> _moviesCollection;
    
    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        await _moviesCollection.InsertOneAsync(movie, cancellationToken: token);
        return true;
    }
    
    // ... implement other methods
}
```

#### 3. **Update DI Registration** (`Movies.Application/Extensions/ApplicationServiceCollectionExtensions.cs`)

```csharp
// Current (PostgreSQL with Dapper)
services.AddSingleton<IMoviesRepository, MovieRepository>();

// Change to SQL Server with Dapper
services.AddSingleton<IMoviesRepository, MovieRepository>();
services.AddSingleton<IDbConnectionFactory>(_ => 
    new SqlServerConnectionFactory(connectionString));

// Or change to MongoDB
services.AddSingleton<IMoviesRepository, MongoMovieRepository>();
services.AddSingleton<IMongoClient>(_ => 
    new MongoClient(connectionString));
```

#### 4. **Update Database Initialization** (`Movies.Application/Database/DbInitializer.cs`)

Change schema creation logic for the new database:

```csharp
// For SQL Server
public async Task InitializeAsync()
{
    using var connection = await _dbConnectionFactory.CreateConnectionAsync();
    await connection.ExecuteAsync("""
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'movies')
        BEGIN
            CREATE TABLE movies (...)
        END
    """);
}

// For MongoDB
public async Task InitializeAsync()
{
    // MongoDB doesn't require explicit schema creation
    // Collections are created on first insert
}
```

#### 5. **Update Connection String** (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=movies;User Id=sa;Password=YourPassword"
  }
}
```

### Supported Databases

With minimal changes, you can use:
- ✅ **PostgreSQL** (Current)
- ✅ **SQL Server**
- ✅ **MySQL**
- ✅ **SQLite**
- ✅ **MongoDB**
- ✅ **Firebase Firestore**
- ✅ **AWS DynamoDB**

### What Does NOT Change

The following remain **completely unchanged**:

| Component | Reason |
|-----------|--------|
| `IMoviesRepository` interface | Abstraction layer |
| `IMovieService` | Business logic independent of data source |
| `Controllers` | API endpoints unchanged |
| `MovieValidator` | Validation rules unchanged |
| `Contracts` | API schema unchanged |
| `ContractMapping` | Mapping logic unchanged |

### Example: PostgreSQL → MongoDB Migration

**Time to implement:** ~30 minutes

1. Install MongoDB packages
2. Create `MongoMovieRepository` implementing `IMoviesRepository`
3. Update `IDbConnectionFactory` implementation
4. Update DI registration
5. Update `DbInitializer`
6. Test endpoints - no controller changes needed!

---

## Micro-ORM to Full ORM Migration: Dapper → Entity Framework Core

### Architecture Flexibility: Zero Breaking Changes

This project demonstrates **extreme loose coupling** by using the Repository Pattern. You can completely replace **Dapper** (Micro-ORM) with **Entity Framework Core** (Full ORM) by modifying **ONLY TWO DIRECTORIES**:

1. `Movies.Application/Database/` - Connection & DbContext setup
2. `Movies.Application/Repositories/` - CRUD implementation

**Everything else remains completely untouched:**
- ✅ No changes to Controllers
- ✅ No changes to Services
- ✅ No changes to Validators
- ✅ No changes to Mapping
- ✅ No changes to API Layer
- ✅ No changes to Contracts

### Why This Flexibility Exists

```
┌─────────────────────────────────────┐
│    HTTP Requests (Controllers)      │  ← UNCHANGED
├─────────────────────────────────────┤
│  Business Logic (Services)          │  ← UNCHANGED
├─────────────────────────────────────┤
│  IMoviesRepository Interface        │  ← UNCHANGED (Contract)
├─────────────────────────────────────┤
│  MovieRepository Implementation     │  ← CHANGES HERE
│  (Can use Dapper OR EF Core)        │
├─────────────────────────────────────┤
│  Database Access (DbContext/SQL)    │  ← CHANGES HERE
│  (Can use connection string OR EF)  │
├─────────────────────────────────────┤
│    PostgreSQL Database              │  ← UNCHANGED
└─────────────────────────────────────┘
```

The interface acts as a **contract** - the implementation can change without affecting consumers.

### Step-by-Step: Dapper → Entity Framework Core Migration

#### Step 1: Install Entity Framework Core NuGet Packages

```bash
cd Movies.Application

# Add EF Core packages
dotnet add package Microsoft.EntityFrameworkCore --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 9.0.0

# Remove Dapper if migrating completely
dotnet remove package Dapper
```

#### Step 2: Create DbContext (Replace Connection Factory)

**File: `Movies.Application/Database/MoviesDbContext.cs`** (NEW)

```csharp
using Microsoft.EntityFrameworkCore;
using Movies.Application.Models;

namespace Movies.Application.Database;

public class MoviesDbContext(DbContextOptions<MoviesDbContext> options) : DbContext(options)
{
    public required DbSet<Movie> Movies { get; set; }
    public required DbSet<Genre> Genres { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Movie entity
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(e => e.Slug)
                .IsUnique();

            entity.Property(e => e.YearOfRelease)
                .IsRequired();

            // Configure one-to-many relationship
            entity.HasMany(m => m.GenreEntities)
                .WithOne()
                .HasForeignKey(g => g.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Genre entity
        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => new { e.MovieId, e.Name });

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
        });
    }
}

// Genre entity for EF Core (if needed)
public class Genre
{
    public Guid MovieId { get; set; }
    public required string Name { get; set; }
}
```

#### Step 3: Update Database Extension

**File: `Movies.Application/Extensions/ApplicationServiceCollectionExtensions.cs`** (MODIFIED)

```csharp
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.AssemblyMarker;
using Movies.Application.Database;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;

namespace Movies.Application.Extensions;

public static class ApplicationServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IMoviesRepository, MovieRepository>();
        services.AddSingleton<IMovieService, MovieService>();
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);
    }

    // CHANGED: Now registers DbContext instead of connection factory
    public static void AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<MoviesDbContext>(options =>
            options.UseNpgsql(connectionString,
                npgsqlOptions => npgsqlOptions.EnableRetryOnFailure())
        );
        services.AddScoped<DbInitializer>();
    }
}
```

#### Step 4: Rewrite Repository with Entity Framework Core

**File: `Movies.Application/Repositories/MovieRepository.cs`** (REWRITTEN)

```csharp
using Microsoft.EntityFrameworkCore;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository(MoviesDbContext dbContext) : IMoviesRepository
{
    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        // EF Core handles everything - just add and save
        await dbContext.Movies.AddAsync(movie, cancellationToken: token);
        
        // Add genres
        foreach (var genre in movie.Genres)
        {
            await dbContext.Genres.AddAsync(
                new Genre { MovieId = movie.Id, Name = genre },
                cancellationToken: token);
        }
        
        var result = await dbContext.SaveChangesAsync(cancellationToken: token);
        return result > 0;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        var movie = await dbContext.Movies
            .Include(m => m.GenreEntities)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken: token);

        if (movie is null)
            return null;

        // Map genres back to string list
        movie.Genres = movie.GenreEntities
            .Select(g => g.Name)
            .ToList();

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug, CancellationToken token = default)
    {
        var movie = await dbContext.Movies
            .Include(m => m.GenreEntities)
            .FirstOrDefaultAsync(m => m.Slug == slug, cancellationToken: token);

        if (movie is null)
            return null;

        movie.Genres = movie.GenreEntities
            .Select(g => g.Name)
            .ToList();

        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default)
    {
        var movies = await dbContext.Movies
            .Include(m => m.GenreEntities)
            .ToListAsync(cancellationToken: token);

        foreach (var movie in movies)
        {
            movie.Genres = movie.GenreEntities
                .Select(g => g.Name)
                .ToList();
        }

        return movies;
    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        var existingMovie = await dbContext.Movies
            .Include(m => m.GenreEntities)
            .FirstOrDefaultAsync(m => m.Id == movie.Id, cancellationToken: token);

        if (existingMovie is null)
            return false;

        // Update movie properties
        existingMovie.Title = movie.Title;
        existingMovie.YearOfRelease = movie.YearOfRelease;

        // Update genres
        dbContext.Genres.RemoveRange(existingMovie.GenreEntities);
        foreach (var genre in movie.Genres)
        {
            await dbContext.Genres.AddAsync(
                new Genre { MovieId = movie.Id, Name = genre },
                cancellationToken: token);
        }

        dbContext.Movies.Update(existingMovie);
        var result = await dbContext.SaveChangesAsync(cancellationToken: token);
        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        var movie = await dbContext.Movies
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken: token);

        if (movie is null)
            return false;

        dbContext.Movies.Remove(movie);
        var result = await dbContext.SaveChangesAsync(cancellationToken: token);
        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        return await dbContext.Movies
            .AnyAsync(m => m.Id == id, cancellationToken: token);
    }
}
```

#### Step 5: Update DbInitializer

**File: `Movies.Application/Database/DbInitializer.cs`** (MODIFIED)

```csharp
using Microsoft.EntityFrameworkCore;

namespace Movies.Application.Database;

public class DbInitializer(MoviesDbContext dbContext)
{
    public async Task InitializeAsync()
    {
        // Apply all pending migrations automatically
        // or create database if it doesn't exist
        if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
        {
            await dbContext.Database.MigrateAsync();
        }
        else
        {
            // Ensure database exists
            await dbContext.Database.EnsureCreatedAsync();
        }
    }
}
```

#### Step 6: Update Movie Model (Optional Enhancement)

**File: `Movies.Application/Models/Movie.cs`** (ENHANCED FOR EF CORE)

```csharp
using System.Text.RegularExpressions;
using Movies.Application.Database;

namespace Movies.Application.Models;

public partial class Movie
{
    public required Guid Id { get; init; }

    public string Slug => GenerateSlug();
    
    public required string Title { get; set; }

    public required int YearOfRelease { get; set; }

    public required List<string> Genres { get; set; } = [];
    
    // EF Core navigation property (not exposed in API)
    public List<Genre> GenreEntities { get; set; } = [];
    
    private string GenerateSlug()
    {
        var sluggedTitle = SlugRegex().Replace(Title, string.Empty)
            .ToLower()
            .Replace(" ", "-");

        return $"{sluggedTitle}-{YearOfRelease}";
    }

    [GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 5)]
    private static partial Regex SlugRegex();
}
```

#### Step 7: Update Program.cs in Movies.Api

**File: `Movies.Api/Program.cs`** (MINIMAL CHANGES)

```csharp
// ... existing code ...

// CHANGED: Was AddDatabase, now includes DbContext
builder.Services.AddApplication();
builder.Services.AddDatabase(config["Database:ConnectionString"]!);

// ... rest remains the same ...
```

### Migration Summary

| File | Change | Reason |
|------|--------|--------|
| `DbContext.cs` | CREATE NEW | Replace connection factory pattern |
| `MovieRepository.cs` | REWRITE | EF Core LINQ instead of Dapper SQL |
| `DbInitializer.cs` | MODIFY | Use EF migrations instead of manual SQL |
| `ApplicationServiceCollectionExtensions.cs` | MODIFY | Register DbContext instead of factory |
| `Movie.cs` | ENHANCE | Add navigation properties |
| **IMoviesRepository.cs** | **NO CHANGE** | Interface contract remains identical |
| **Controllers** | **NO CHANGE** | Endpoint logic untouched |
| **Services** | **NO CHANGE** | Business logic untouched |
| **Validators** | **NO CHANGE** | Validation rules untouched |
| **API Layer** | **NO CHANGE** | No modifications needed |

### Key Benefits of This Architecture

1. **Swappable Implementations**
   - Dapper ↔ EF Core ↔ MongoDB ↔ Any ORM
   - Only repository and database folders change

2. **Zero Service/Controller Impact**
   - Business logic never references repository implementation details
   - Services depend only on interfaces

3. **Easy Testing**
   - Mock `IMoviesRepository` in unit tests
   - No need to test database-specific code in service tests

4. **Technology Agnostic**
   - Add features without worrying about ORM lock-in
   - Team can learn new ORMs without affecting whole codebase

### Side-by-Side Comparison

#### With Dapper
- ✅ Lightweight, minimal overhead
- ✅ Full control over SQL
- ✅ Excellent for complex queries
- ❌ Manual object mapping required
- ❌ No automatic relationships

#### With Entity Framework Core
- ✅ Automatic relationship tracking
- ✅ LINQ queries (type-safe)
- ✅ Built-in migrations
- ✅ Change tracking
- ❌ Slightly more overhead
- ❌ Learning curve

### Migration Time Estimate

- **Experienced Developer:** 45 minutes to 1 hour
- **New to EF Core:** 2-3 hours
- **Testing & Validation:** 30 minutes

**Why so fast?**
- Only 2 directories modified
- All interfaces already defined
- No cascading changes required
- No business logic rewrites

This is the power of clean architecture and the Repository Pattern! 🎉

---

## Manual Mapping Pattern

The project uses **manual mapping** instead of AutoMapper for several reasons:

### Mapping Files
- **Location**: `Movies.Api/Mapping/ContractMapping.cs`
- **Method**: Extension methods on request/response objects

### Mapping Operations

```csharp
// Request → Domain Model
public static Movie MapToMovie(this CreateMovieRequest request)
{
    return new Movie
    {
        Id = Guid.CreateVersion7(),
        Title = request.Title,
        YearOfRelease = request.YearOfRelease,
        Genres = request.Genres.ToList()
    };
}

// Domain Model → Response
public static MovieResponse MapToResponse(this Movie movie)
{
    return new MovieResponse
    {
        Id = movie.Id,
        Title = movie.Title,
        Slug = movie.Slug,
        YearOfRelease = movie.YearOfRelease,
        Genres = movie.Genres
    };
}

// Collection mapping
public static MoviesResponse MapToResponse(this IEnumerable<Movie> movies)
{
    return new MoviesResponse
    {
        Items = movies.Select(MapToResponse)
    };
}
```

### Why Manual Mapping?

1. **Explicit & Transparent** - Exactly what gets mapped is visible
2. **Better Performance** - No reflection overhead
3. **Easy to Debug** - Straightforward code paths
4. **Type Safety** - Compile-time checking
5. **Flexibility** - Complex transformations handled easily
6. **No Magic** - All mapping logic is visible and testable

---

## Industry Standards Implemented

### 1. **RESTful API Design**
- ✅ Proper HTTP methods (GET, POST, PUT, DELETE)
- ✅ Correct status codes (200, 201, 204, 400, 404)
- ✅ Resource-based URLs
- ✅ Stateless operations

### 2. **API Versioning Ready**
- ✅ Centralized endpoint constants
- ✅ Easy to add `/api/v2/movies` routes

### 3. **SOLID Principles**
- ✅ **S**ingle Responsibility - Services, Repositories, Controllers
- ✅ **O**pen/Closed - Extensible via interfaces
- ✅ **L**iskov Substitution - Repository implementations
- ✅ **I**nterface Segregation - Small focused interfaces
- ✅ **D**ependency Inversion - DI throughout

### 4. **Security**
- ✅ JWT Authentication
- ✅ Authorization policies
- ✅ Input validation
- ✅ HTTPS enforcement
- ✅ Claims-based authorization

### 5. **Error Handling**
- ✅ Standardized error responses
- ✅ Meaningful error messages
- ✅ Exception middleware
- ✅ Proper HTTP status codes

### 6. **Async/Concurrency**
- ✅ Async/await throughout
- ✅ Cancellation token support
- ✅ Database transactions
- ✅ Connection pooling

### 7. **Logging & Observability**
- ✅ Structured logging ready
- ✅ Health checks capability
- ✅ API documentation (OpenAPI)

### 8. **Code Organization**
- ✅ Clear layer separation
- ✅ Consistent naming conventions
- ✅ Modular project structure
- ✅ DI configuration centralized

---

## Getting Started

### Prerequisites
- .NET 9 SDK
- PostgreSQL 13+
- Visual Studio / Rider / VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd RestApiCourse
   ```

2. **Start PostgreSQL** (using docker-compose)
   ```bash
   cd Movies.Application
   docker-compose up -d
   ```

3. **Configure connection string** (if needed)
   - Update `appsettings.json` in Movies.Api

4. **Run the API**
   ```bash
   cd Movies.Api
   dotnet run
   ```

5. **Access API Documentation**
   - Swagger: `https://localhost:5001/swagger`
   - Scalar: `https://localhost:5001/scalar`

### Testing Endpoints
- Use `Movies.Api.http` file in Rider/VS Code for quick testing
- Or import Postman collection

---

## Learning Outcomes

This project demonstrates:

✅ Clean Architecture principles  
✅ Repository & Service patterns  
✅ Dependency Injection  
✅ JWT Authentication & Authorization  
✅ Fluent Validation  
✅ Dapper ORM usage  
✅ Async/Await patterns  
✅ Database transactions  
✅ Middleware implementation  
✅ API design best practices  
✅ Error handling strategies  
✅ Manual mapping patterns  

---

## Conclusion

This project is a production-ready template for building scalable REST APIs with .NET. Its architecture ensures that business logic remains independent from data access, making it trivial to swap databases, testing frameworks, or other infrastructure components without touching controller or service code.

The separation of concerns and adherence to SOLID principles make this codebase a solid foundation for enterprise applications.

---

**Last Updated:** March 2, 2026  
**Framework:** .NET 9  
**Database:** PostgreSQL  
**License:** MIT

