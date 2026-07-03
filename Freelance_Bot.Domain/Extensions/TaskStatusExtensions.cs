using Freelance_Bot.Domain.Enum;
using TaskStatus = Freelance_Bot.Domain.Enum.TaskStatus;

namespace Freelance_Bot.Domain.Extensions;

public static class TaskStatusExtensions
{
    public static TaskStatus Next(this TaskStatus status)
    {
        return status switch
        {
            TaskStatus.Todo => TaskStatus.InProgress,
            TaskStatus.InProgress => TaskStatus.Done,
            TaskStatus.Done => TaskStatus.Todo,
            _ => TaskStatus.Todo
        };
    }
}