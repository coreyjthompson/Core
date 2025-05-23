﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MEI.Core.Validation
{
    public class ValidateObjectAttribute
        : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var context = new ValidationContext(value, null, null);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(value, context, results, validateAllProperties: true);

            if (results.Count == 0)
            {
                return ValidationResult.Success;
            }

            return new CompositeValidationResult(
                string.Format("Validation for {0} failed!", validationContext.DisplayName),
                results);
        }
    }
}
