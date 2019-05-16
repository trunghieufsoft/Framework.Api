using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Common.Core.Validations
{
    public class EmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || IsEmptyString(value))
            {
                return ValidationResult.Success;
            }

            Regex emailValid = new Regex(@"^.+@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*(\.[a-zA]{2,})$");
            if (!emailValid.IsMatch((string)value))
            {
                return new ValidationResult("Email is invalid format");
            }

            return ValidationResult.Success;
        }

        private bool IsEmptyString(object value)
        {
            return value is string s && string.IsNullOrEmpty(s);
        }
    }
}