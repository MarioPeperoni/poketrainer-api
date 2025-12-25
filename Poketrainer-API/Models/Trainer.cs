using System.ComponentModel.DataAnnotations;

namespace Poketrainer_API.Models;

public class TrainerRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Name  must be between 2 and 30 characters")]
    public string TrainerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Age is  required")]
    [Range(16, 99, ErrorMessage = "Age must be between 16 and 99")]
    public string TrainserAge { get; set; } = string.Empty;

    [Required(ErrorMessage = "Pokemon must be selected")]
    public string PokemonName { get; set; } = string.Empty;
}

public class TrainerResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public TrainerRequest? Trainer { get; set; }
}