
//using TaskStatus = Freelance_Bot.Domain.Enum.TaskStatus;
//using Freelance_Bot.Domain.Enum;

//namespace Freelance_Bot.Domain.Extensions;

//public static class TaskStatusExtensions
//{
//    public static TaskStatus Next(this TaskStatus status)
//    {
//        return status switch
//        {
//            TaskStatus.Todo => TaskStatus.InProgress,
//            TaskStatus.InProgress => TaskStatus.InReview,
//            TaskStatus.InReview => TaskStatus.Done,
//            TaskStatus.Done => TaskStatus.Done,
//            TaskStatus.Cancelled => TaskStatus.Cancelled,
//            _ => TaskStatus.Todo
//        };
//    }
//    }