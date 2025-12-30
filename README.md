# PokéTrainer API

ASP.NET Core backend API for the PokéTrainer application.

## How to set up and run the project

### Prerequisites
- .NET 9.0 SDK

### Development

```bash
# Restore dependencies
dotnet restore

# Run the application
dotnet run --project Poketrainer-API/Poketrainer-API.csproj
```

API will be available at:
- HTTP: http://localhost:5017
- API Documentation: http://localhost:5017/scalar/v1

## 📁 Project Structure

```
Poketrainer-API/
├── Data/
│   └── pokemon.json       # Pokemon dataset
├── Models/               # Data models
│   ├── Pokemon.cs
│   └── Trainer.cs
├── Services/             # Business logic
│   ├── NtpService.cs
│   ├── PokemonApiService.cs
│   ├── PokemonSearchService.cs
│   └── TrainerService.cs
├── Program.cs            # Application entry point
└── appsettings.json      # Configuration
```

## 📦 Dependencies

- **Scalar.AspNetCore** - API documentation
- **FuzzySharp** - Fuzzy string matching
- **Yort.Ntp.Portable** - NTP client
- **Microsoft.Extensions.Caching.Memory** - Caching


## Available Commands

```bash
dotnet restore                    # Restore dependencies
dotnet build                      # Build project
dotnet run                        # Run application
dotnet watch                      # Run with hot reload
```

## Docker Support

Build and run with Docker:

```bash
docker build -t poketrainer-api .
docker run -p 5017:5017 poketrainer-api
```
