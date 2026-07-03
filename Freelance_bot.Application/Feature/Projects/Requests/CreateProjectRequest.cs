namespace Freelance_bot.Application.Feature.Projects.Requests
{
    public record CreateProjectRequest(
        string Title,
        string? Description,
        Guid? ClientId,
     
        string? ClientName,
        string? ClientEmail,
        string? ClientPhone,
        decimal? Budget,
        string Currency,
        DateTime? StartDate,
        DateTime? Deadline
    );
}