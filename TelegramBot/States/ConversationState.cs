namespace TelegramBot.States
{
    public enum ConversationStep
    {
        None,
        AwaitingWorkspaceName,     
        AwaitingProjectName,
        AwaitingProjectDescription,
        AwaitingClientName,
        AwaitingClientEmail,
        AwaitingClientPhone,
        AwaitingBudget,
        AwaitingDeadline,
        AwaitingTaskTitle,
        AwaitingTaskDescription,
        AwaitingTaskDeadline,   
        AwaitingConfirmation,
        AwaitingTaskDecision
    }
}