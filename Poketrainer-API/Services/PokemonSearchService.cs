using FuzzySharp;

namespace Poketrainer_API.Services;
using System.Text.Json;
using Poketrainer_API.Models;

public interface IPokemonSearchService
{
    Task<List<PokemonSearchResult>> SearchPokemonAsync(string query);
}

public class PokemonSearchService: IPokemonSearchService
{
    private readonly List<PokemonJsonData> _pokemonList;

    public PokemonSearchService()
    {
        try
        {
            var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "pokemon.json");
            var jsonContent = File.ReadAllText(jsonPath);
            var jsonRoot = JsonSerializer.Deserialize<PokemonJsonRoot>(jsonContent);
            _pokemonList = jsonRoot?.Data ?? [];
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Failed to load Pokemon data", e);
        }

    }

    public Task<List<PokemonSearchResult>> SearchPokemonAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Task.FromResult(new List<PokemonSearchResult>());
        }

        var results = _pokemonList.Select(pokemon => new PokemonSearchResult
            {
                Id = pokemon.Id,
                Name = pokemon.Name,
                Score = Fuzz.PartialRatio(query.ToLower(), pokemon.Name.ToLower())
            })
            .Where(result => result.Score > 60) // Check for similarity 60%
            .OrderByDescending(result => result.Score)
            .Take(10)
            .ToList();

        return Task.FromResult(results);
    }
}