namespace CollaborativeCodeEditor.Domain.Executions;

public enum ExecutionStatus
{
    Requested = 1,
    Queued = 2,
    Running = 3,
    Completed = 4,
    Failed = 5,
    TimedOut = 6
}