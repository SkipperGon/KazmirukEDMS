using System.ComponentModel.DataAnnotations;

namespace KazmirukEDMS.Models
{
    public enum ApprovalDecision
    {
        None = 0,
        Approved = 1,
        Rejected = 2,
        NeedsChanges = 3
    }

    public class Approval
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid DocumentVersionId { get; set; }

        public Guid? WorkflowStepId { get; set; }

        public string? ApproverId { get; set; }

        public ApprovalDecision Decision { get; set; } = ApprovalDecision.None;

        public string? Comment { get; set; }

        public DateTime DecisionAt { get; set; } = DateTime.UtcNow;
    }
}