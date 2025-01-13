using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.Business.Validations
{
    public class CompareDatesAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public CompareDatesAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var currentValue = (DateTime?)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null)
            {
                return new ValidationResult($"Property '{_comparisonProperty}' not found.");
            }

            var comparisonValue = (DateTime?)property.GetValue(validationContext.ObjectInstance);

            if (currentValue.HasValue && comparisonValue.HasValue && currentValue < comparisonValue)
            {
                return new ValidationResult(ErrorMessage ?? "Bitiş tarihi başlangıç ​​tarihinden büyük veya ona eşit olmalıdır");
            }

            return ValidationResult.Success;
        }
    }
}
