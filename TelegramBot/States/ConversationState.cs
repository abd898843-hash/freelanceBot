namespace TelegramBot.States
{
    public enum ConversationStep
    {
        None,

        // خطوات إنشاء المشروع والبيانات
        AwaitingWorkspaceName,      // الخطوة التي طلبت إضافتها
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
        // حالة التأكيد وما بعدها
        AwaitingConfirmation,
        AwaitingTaskDecision
    }
}