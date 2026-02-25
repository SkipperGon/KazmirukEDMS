using System.ComponentModel.DataAnnotations;

namespace KazmirukEDMS.Models
{
    public enum WorkflowStepStatus
    {
        Pending = 0,
        InProgress = 1,
        Completed = 2,
        Skipped = 3
    }

    public class WorkflowStep
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid DocumentId { get; set; }

        // Order in the workflow
        public int StepOrder { get; set; }

        // Role required to approve this step (IdentityRole.Id)
        public string? RoleId { get; set; }

        // Assigned user (optional)
        public string? AssignedUserId { get; set; }

        public WorkflowStepStatus Status { get; set; } = WorkflowStepStatus.Pending;

        public DateTime? AssignedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public string? Comment { get; set; }
    }

}
