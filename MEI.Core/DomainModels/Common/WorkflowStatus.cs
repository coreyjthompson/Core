using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MEI.Core.DomainModels.Common
{
    public abstract class WorkflowStatus
    {
        [Required]
        public int Id { get; set; }

        public int WorkflowStepId { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTimeOffset WhenCreated { get; set; } = DateTimeOffset.Now;

        public string Notes { get; set; }

        public virtual WorkflowStep WorkflowStep { get; set; }

    }
}