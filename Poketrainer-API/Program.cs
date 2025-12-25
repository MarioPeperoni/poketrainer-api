using Microsoft.AspNetCore.Mvc;
using Poketrainer_API.Services;
using Poketrainer_API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<INtpService, NtpService>();
builder.Services.AddSingleton<IPokemonSearchService, PokemonSearchService>();
builder.Services.AddHttpClient<IPokemonApiService, PokemonApiService>();
builder.Services.AddScoped<ITrainerService, TrainerService>();

var app = builder.Build();

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
}).WithName("GetNtpTime").WithTags("Time");

app.MapGet("/api/search", async ([FromQuery(Name = "q")] string? query,
    IPokemonSearchService pokemonSearchService) =>
{
    if (string.IsNullOrWhiteSpace(query))
    {
        return Results.BadRequest(new {error = "Parameter required"});
    }

    var results = await pokemonSearchService.SearchPokemonAsync(query);
    return Results.Ok(results);

}).WithName("SearchPokemon").WithTags("Pokemon");

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
}).WithName("GetPokemonById").WithTags("Pokemon");

app.MapPost("/api/trainer", ([FromBody] TrainerRequest request, ITrainerService trainerService) =>
{
    var response = trainerService.ValidateTrainer(request);

    if (!response.Success)
    {
        return Results.BadRequest(response);
    }

    return Results.Created($"/api/trainer", response); 
}).WithName("ValidateTrainer").WithTags("Trainer");


app.UseHttpsRedirection();

app.Run();