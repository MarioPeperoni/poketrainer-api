using Microsoft.AspNetCore.Mvc;

using Poketrainer_API.Services;
using Poketrainer_API.Models;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<INtpService, NtpService>();
builder.Services.AddSingleton<IPokemonSearchService, PokemonSearchService>();
builder.Services.AddHttpClient<IPokemonApiService, PokemonApiService>();
builder.Services.AddScoped<ITrainerService, TrainerService>();
builder.Services.AddOpenApi();

var app = builder.Build();
app.MapOpenApi();


app.MapScalarApiReference(options =>
{
    options.WithTitle("PokeTrainer API").WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.MapGet("/api/time", async (INtpService ntpService) =>
    {
        try
        {
            var time = await ntpService.GetNetworkTimeAsync();
            return Results.Ok(new
            {
                time,
                formatted = time.ToString("dddd, dd.MM.yyyy"),
                timezone = "UTC"
            });
        }
        catch (Exception e)
        {
            return Results.Problem(detail: e.Message, statusCode: 500, title: "Failed to receive NTP time.");
        }
    }).WithName("GetNtpTime")
    .WithTags("Time")
    .WithSummary("Get current UTC time from NTP server")
    .WithDescription("Retrieves synchronized time from Google's NTP server (time.google.com)")
    .Produces(200)
    .Produces(500);
;

app.MapGet("/api/search", async ([FromQuery(Name = "q")] string? query,
        IPokemonSearchService pokemonSearchService) =>
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Results.BadRequest(new { error = "Parameter required" });
        }

        var results = await pokemonSearchService.SearchPokemonAsync(query);
        return Results.Ok(results);
    }).WithName("SearchPokemon")
    .WithTags("Pokemon")
    .WithSummary("Search Pokemon by name using fuzzy matching")
    .WithDescription("Returns up to 10 Pokemon with similarity score above 60%. Supports partial matches and typos.")
    .Produces<List<PokemonSearchResult>>(200)
    .Produces(400);

app.MapGet("/api/pokemon", async ([FromQueryAttribute(Name = "id")] int? id, IPokemonApiService pokemonService) =>
    {
        if (!id.HasValue || id.Value <= 0)
        {
            return Results.BadRequest(new { error = "Valid Pokemon ID is required" });
        }

        try
        {
            var pokemon = await pokemonService.GetPokemonByIdAsync(id.Value);

            if (pokemon == null)
            {
                return Results.NotFound(new { error = "Pokemon with given ID does not exist" });
            }

            return Results.Ok(pokemon);
        }
        catch (Exception e)
        {
            return Results.Problem(detail: e.Message, statusCode: 500, title: "Failed to receive Pokemon ID.");
        }
    }).WithName("GetPokemonById")
    .WithTags("Pokemon")
    .WithSummary("Get detailed Pokemon information by ID")
    .WithDescription(
        "Fetches Pokemon data from PokeAPI including types, base experience, and sprite. Results are cached for 2 hours.")
    .Produces<PokemonDetails>(200)
    .Produces(400)
    .Produces(404)
    .Produces(500);

app.MapPost("/api/trainer", ([FromBody] TrainerRequest request, ITrainerService trainerService) =>
    {
        var response = trainerService.ValidateTrainer(request);

        if (!response.Success)
        {
            return Results.BadRequest(response);
        }

        return Results.Created($"/api/trainer", response);
    }).WithName("CreateTrainer")
    .WithTags("Trainer")
    .WithSummary("Create and validate a new Pokemon trainer")
    .WithDescription(
        "Validates trainer data: name (2-30 chars), age (16-99), and Pokemon selection. Returns validation errors if any requirements are not met.")
    .Accepts<TrainerRequest>("application/json")
    .Produces<TrainerResponse>(201)
    .Produces<TrainerResponse>(400);


app.UseHttpsRedirection();

app.Run();