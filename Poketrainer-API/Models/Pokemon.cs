using System.Text.Json.Serialization;

namespace Poketrainer_API.Models;

public class PokemonJsonData
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class PokemonJsonRoot
{
    [JsonPropertyName("data")]
    public List<PokemonJsonData> Data { get; set; } = [];
}

public class PokemonSearchResult : PokemonJsonData
{
    public int Score { get; set; }
}

public class PokemonDetails
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Type { get; set; } = [];
    public int Experience { get; set; }
    public string SpriteUrl { get; set; } = string.Empty;
}

public class PokemonApiResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("base_experience")]
    public int BaseExperience { get; set; }

    [JsonPropertyName("sprites")]
    public PokemonSpriteApiReponse Sprite { get; set; } = new();

    [JsonPropertyName("types")]
    public List<PokemonTypeSlot> Types { get; set; } = new();
}
public class PokemonSpriteApiReponse
{
    [JsonPropertyName("front_default")]
    public string FrontDefault { get; set; } = string.Empty;
}

public class PokemonTypeApiResponse
{
    [JsonPropertyName("types")]
    public List<PokemonTypeSlot> Types { get; set; } = new();
}

public class PokemonTypeSlot
{
    [JsonPropertyName("type")]
    public PokemonTypeInfo Type { get; set; } = new();
}

public class PokemonTypeInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}