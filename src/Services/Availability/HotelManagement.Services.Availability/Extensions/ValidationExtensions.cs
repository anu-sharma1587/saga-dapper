using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HotelManagement.Services.Availability.Extensions;

public static class ValidationExtensions
{
    public static ActionResult ValidateModelState(this ControllerBase controller)
    {
        if (controller.ModelState.IsValid)
        {
            return null!;
        }

        var errors = GetModelStateErrors(controller.ModelState);
        return controller.BadRequest(new { Errors = errors });
    }

    private static Dictionary<string, string[]> GetModelStateErrors(ModelStateDictionary modelState)
    {
        return modelState.Where(kvp => kvp.Value!.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );
    }

    public static void ValidateDateRange(DateTime startDate, DateTime endDate, ModelStateDictionary modelState)
    {
        if (startDate >= endDate)
        {
            modelState.AddModelError("DateRange", "Start date must be before end date");
        }
    }

    public static void ValidatePositiveNumber(decimal value, string fieldName, ModelStateDictionary modelState)
    {
        if (value < 0)
        {
            modelState.AddModelError(fieldName, $"{fieldName} must be a positive number");
        }
    }

    public static void ValidatePositiveNumber(int value, string fieldName, ModelStateDictionary modelState)
    {
        if (value < 0)
        {
            modelState.AddModelError(fieldName, $"{fieldName} must be a positive number");
        }
    }
}
