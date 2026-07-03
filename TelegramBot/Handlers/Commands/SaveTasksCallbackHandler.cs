using Freelance_Bot.Infrastruction.DB;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Handlers.Interface;
using TelegramBot.States;

public class SaveTasksCallbackHandler : ICallbackHandler
{
    private readonly FreelancerDbContext _context;
    private readonly IConversationStateStore _stateStore;
    private readonly ITelegramBotClient _botClient;

    public SaveTasksCallbackHandler(FreelancerDbContext context, IConversationStateStore stateStore, ITelegramBotClient botClient)
    {
        _context = context;
        _stateStore = stateStore;
        _botClient = botClient;
    }

    public bool CanHandle(string data) => data.StartsWith("save_tasks_");

    public async Task HandleAsync(CallbackQuery callback)
    {
        var projectId = Guid.Parse(callback.Data!.Split('_').Last());
        var chatId = callback.Message!.Chat.Id;

        // استرجاع المصفوفة الجديدة
        var tasksToSave = _stateStore.Get(chatId).TemporaryAutomationTasks;

        if (tasksToSave == null || !tasksToSave.Any())
        {
            await _botClient.AnswerCallbackQueryAsync(callback.Id, "⚠️ لا توجد مهام للحفظ!");
            return;
        }

        foreach (var dto in tasksToSave)
        {
            var newTask = new Freelance_Bot.Domain.Entity.TaskItem
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Title = dto.Title, // مأخوذة من AutomationTaskDto
                Notes = dto.Notes, // مأخوذة من AutomationTaskDto
                Status = Freelance_Bot.Domain.Enum.TaskStatus.Todo
            };
            _context.tasks.Add(newTask);
        }

        await _context.SaveChangesAsync();

        // تنظيف الـ State
        var ctx = _stateStore.Get(chatId);
        ctx.TemporaryAutomationTasks = null;
        _stateStore.Set(chatId, ctx);

        await _botClient.EditMessageTextAsync(chatId, callback.Message.MessageId, "✅ تم حفظ مهام الأتمتة بنجاح!");
    }
}