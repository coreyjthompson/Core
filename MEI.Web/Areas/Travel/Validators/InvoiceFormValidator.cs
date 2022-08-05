using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MEI.Web.Areas.Travel.Models;

namespace MEI.Web.Areas.Travel.Validators
{
    public class InvoiceFormValidator : AbstractValidator<InvoiceFormViewModel>
    {
            public InvoiceFormValidator()
            {
                CascadeMode = CascadeMode.StopOnFirstFailure;

                // Validate these properties only on submit
                When(x => x.FormAction.ToLower() == "submit", () =>
                {
                    RuleFor(x => x.ConsultantId).NotEmpty().WithMessage("Required");
                    RuleFor(x => x.EventId).NotEmpty().WithMessage("Required");

                    RuleFor(x => x.LineItems)
                        .NotNull().WithMessage("At least one item is required")
                        .Must((x, list) => list.Count > 0).WithMessage("At least one item is required");
                   
                });
                
                // Validate these properties on both save or submit
                When(x => (x.FormAction.ToLower() == "save" || x.FormAction.ToLower() == "submit"), () =>
                {
                    // Always require AT LEAST the client - otherwise whats the point?
                    RuleFor(x => x.ClientName).NotEmpty().WithMessage("Required");

                    // if there are any line items at all, check and make sure they contain all db required properties
                    RuleForEach(m => m.LineItems).SetValidator(new InvoiceFormLineItemValidator());
                });

                
            }
    }

    public class InvoiceFormLineItemValidator : AbstractValidator<InvoiceFormViewModel.LineItem>
    {
        public InvoiceFormLineItemValidator()
        {
            RuleFor(x => x.Amount).NotEmpty().WithMessage("Required");
            RuleFor(x => x.TravelServiceId).NotEmpty().WithMessage("Required");
        }
    }

}
