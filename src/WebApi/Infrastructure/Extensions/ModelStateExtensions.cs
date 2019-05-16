using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace WebAPI.Infrastructure.Extensions
{
    public static class ModelStateExtensions
    {
        public static List<string> GetErrors(this ModelStateDictionary modelState)
        {
            List<string> validationErrors = new List<string>();

            foreach (KeyValuePair<string, ModelStateEntry> state in modelState)
            {
                validationErrors.AddRange(state.Value.Errors
                    .Select(error => error.ErrorMessage)
                    .ToList());
            }

            return validationErrors;
        }
    }
}