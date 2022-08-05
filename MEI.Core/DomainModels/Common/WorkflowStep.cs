using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using MEI.Core.Helpers;

namespace MEI.Core.DomainModels.Common
{
    public class WorkflowStep
    {
        public WorkflowStep(WorkflowStepEnum @enum)
        {
            Id = (int) @enum;
            Name = @enum.ToString();
            Description = @enum.ToDescription();
        }

        public WorkflowStep()
        {}

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int WorkflowCategoryId { get; set; }

        public virtual WorkflowCategory WorkflowCategory { get; set; }

        [Required]
        public int StepOrder { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }

        public DateTimeOffset? WhenInactivated { get; set; } = null;
    }

    /// <remarks>
    ///     Do not alter the value of existing fields without intentionally meaning to do so because the database uses them as
    ///     the id.
    ///     Add new ones to the end.
    ///     Explicitly define the values if necessary.
    /// </remarks>
    public enum WorkflowStepEnum
    {
        None,
        InvoiceCreated,
        InvoiceDraftSaved,
        InvoiceSubmittedForPayment,
        InvoiceAwaitingReview,
        InvoiceApproved,
        InvoicePaid,
        InvoiceAwaitingSharePointSave,
        InvoiceSavedToSharePoint,
        InvoiceOnHold
    }
}