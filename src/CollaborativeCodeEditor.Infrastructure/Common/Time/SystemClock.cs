using CollaborativeCodeEditor.Application.Common.Time;

namespace CollaborativeCodeEditor.Infrastructure.Common.Time;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}