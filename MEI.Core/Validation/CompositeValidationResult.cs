using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MEI.Core.Validation
{
    public class CompositeValidationResult
        : ValidationResult, IEnumerable<ValidationResult>
    {
        public CompositeValidationResult(string errorMessage, IEnumerable<ValidationResult> results)
            : base(errorMessage)
        {
            Results = results;
        }

        public IEnumerable<ValidationResult> Results { get; }

        public IEnumerator<ValidationResult> GetEnumerator() => Results.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
