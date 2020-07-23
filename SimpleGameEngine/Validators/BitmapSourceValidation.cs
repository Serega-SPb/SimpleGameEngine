using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace SimpleGameEngine.Validators
{
    public class BitmapSourceValidation: ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var strValue = value?.ToString();
            
            if (string.IsNullOrWhiteSpace(strValue))
                return new ValidationResult(true, "Path is null");
            
            return File.Exists(strValue) ? 
                new ValidationResult(true, null) : 
                new ValidationResult(false, $"{strValue} is not file");
        }
    }
}