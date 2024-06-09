using Telegram.Bot.Types;
using User = DataAccess.Models.User;

namespace BusinessLogic.Workflow;

public interface IWorkflowAction
{
    Task ProcessAsync(User user, Update update);
}
