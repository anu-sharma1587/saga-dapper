namespace HotelManagement.Services.Orchestrator.Models;

public enum SagaStepStatus { Pending, Completed, Failed, Compensated }

public class SagaStep
{
    public string Name { get; set; } = null!;
    public SagaStepStatus Status { get; set; }
    public string? Error { get; set; }
}

public class SagaState
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public List<SagaStep> Steps { get; set; } = new();
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsCompensated { get; set; }
}
