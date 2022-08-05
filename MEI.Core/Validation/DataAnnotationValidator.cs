using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace MEI.Core.Validation
{
    public class DataAnnotationValidator
        : IValidator
    {
        [DebuggerStepThrough]
        void IValidator.ValidateObject(object instance)
        {
            var context = new ValidationContext(instance, null, null);

            Validator.ValidateObject(instance, context, validateAllProperties: true);
        }
    }
}
