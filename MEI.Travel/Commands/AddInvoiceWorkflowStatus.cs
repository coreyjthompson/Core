using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using MEI.Core.Commands;
using MEI.Core.DomainModels.Common;
using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace MEI.Travel.Commands
{
    public class AddInvoiceWorkflowStatus
        : ICommand<int>
    {
        [Required] public WorkflowStepEnum WorkflowStep { get; set; }

        [Required] public string CreatedBy { get; set; }

        public string Notes { get; set; }

        [Required] public int InvoiceId { get; set; }

        public override string ToString()
        {
            return string.Format("InvoiceId={0}, WorkflowStepId={1}, CreatedBy={2}, Notes={3}]", InvoiceId, (int) WorkflowStep, CreatedBy, Notes);
        }
    }

    public class AddInvoiceWorkflowStatusHandler
        : ICommandHandler<AddInvoiceWorkflowStatus, int>
    {
        private readonly CoreContext _db;

        public AddInvoiceWorkflowStatusHandler(CoreContext db)
        {
            _db = db;
        }

        public async Task<int> HandleAsync(AddInvoiceWorkflowStatus command)
        {
            // Validate invoice
            if (command.InvoiceId == 0)
            {
                throw new ArgumentNullException(nameof(command.InvoiceId));
            }

            var invoiceTask = _db.TravelInvoices
                .Include("LineItems")
                .Include("WorkflowSteps")
                .FirstOrDefaultAsync(x => x.Id == command.InvoiceId);

            // Validate workflowstep
            var workflowStepTask = _db.WorkflowSteps.FirstOrDefaultAsync(i => i.Id == (int) command.WorkflowStep);

            // Now, await them so that they all must be completed before any code after this can run which is dependent on them
            await Task.WhenAll(invoiceTask, workflowStepTask);

            // If that invoice does not exist, throw error
            if (invoiceTask.Result == null)
            {
                throw new ArgumentException(string.Format("Invalid Invoice Id. {0}", command.InvoiceId));
            }

            // If that workflowstep does not exist, throw error
            if (workflowStepTask.Result == null)
            {
                throw new ArgumentException(string.Format("Invalid Workflow Step Id. {0}", (int) command.WorkflowStep));
            }

            // Create the new status
            var newStatus = new InvoiceWorkflowStatus {CreatedBy = command.CreatedBy, InvoiceId = command.InvoiceId, Notes = command.Notes, WorkflowStepId = (int) command.WorkflowStep};

            await _db.TravelInvoiceWorkflowStatuses.AddAsync(newStatus);

            await _db.SaveChangesAsync();

            return newStatus.Id;
        }
    }
}