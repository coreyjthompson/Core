using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using MEI.Core.Helpers;

namespace MEI.Core.DomainModels.Common
{
    public class WorkflowCategory
    {
        public WorkflowCategory(WorkflowCategoryEnum @enum)
        {
            Id = (int) @enum;
            Name = @enum.ToString();
            Description = @enum.ToDescription();
        }

        protected WorkflowCategory()
        {}

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }

        public DateTimeOffset? WhenInactivated { get; set; } = null;
    }

    public enum WorkflowCategoryEnum
    {
        None,
        Contract,
        Invoice,
        Event,
        Consultant,
        Deliverable
    }
}