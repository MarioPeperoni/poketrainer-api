using System.ComponentModel.DataAnnotations;
using Poketrainer_API.Models;

namespace Poketrainer_API.Services;

public interface ITrainerService
{
    TrainerResponse ValidateTrainer(TrainerRequest request);
}

public class TrainerService: ITrainerService
{
    public TrainerResponse ValidateTrainer(TrainerRequest request)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(request);
        var isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

        if (isValid)
            return new TrainerResponse()
            {
                Success = true,
                Trainer = request
            };
        
        var errors = validationResults.Select(result => result.ErrorMessage).ToList();
        return new TrainerResponse()
        {
            Success = false,
            Message = $"Validation failed: {string.Join(",", errors)}",
            Trainer = null
        };

    }
}