using System.Text.Json;
using Poketrainer_API.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Poketrainer_API.Services;

public interface IPokemonApiService
{
    Task<PokemonDetails?> GetPokemonByIdAsync(int id);
}

public class PokemonApiService : IPokemonApiService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<PokemonApiService> _logger;
    private const string ApiUrl = "https://pokeapi.co/api/v2/pokemon/";

    public PokemonApiService(HttpClient httpClient, IMemoryCache cache, ILogger<PokemonApiService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<PokemonDetails?> GetPokemonByIdAsync(int id)
    {
        var cacheKey = $"pok_{id}";

        // Check for cache
        if (_cache.TryGetValue(cacheKey, out PokemonDetails? cachedPokemon))
        {
            _logger.LogInformation($"Pokemon {id} cache hit");
            return cachedPokemon;
        }

        var response = await _httpClient.GetAsync($"{ApiUrl}/{id}");

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning($"Pokemon {id} not found");
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<PokemonApiResponse>(content);

        if (apiResponse == null)
        {
            return null;
        }

        var pokemonDetails = new PokemonDetails()
        {
            Id = apiResponse.Id,
            Name = apiResponse.Name,
            Type = apiResponse.Types.Select(t => t.Type.Name).ToList(),
            Experience = apiResponse.BaseExperience,
            SpriteUrl = apiResponse.Sprite.FrontDefault
        };

        var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(24));

        _cache.Set(cacheKey, pokemonDetails, cacheOptions);
        _logger.LogInformation($"Pokemon {id} cached");

        return pokemonDetails;
    }
}