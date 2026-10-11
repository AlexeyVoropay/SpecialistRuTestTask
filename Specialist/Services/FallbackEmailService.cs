using Specialist.DTOs;
using Specialist.Enums;

namespace Specialist.Services;

public interface IFallbackEmailService : IEmailService
{

}

public class FallbackEmailService : IFallbackEmailService
{
    public List<(string To, LeadRequestDto Lead)> SentEmails { get; } = [];

    public Task SendAsync(LeadRequestDto lead, CancellationToken ct = default)
    {
        string recipient = lead.ClientType switch
        {
            ClientType.Corporate => "specialist.corp@yandex.ru",
            ClientType.Individual => "specialist.ufl@yandex.ru",
            _ => throw new ArgumentOutOfRangeException()
        };

        // Имитация реальной отправки
        SentEmails.Add((recipient, lead));
        return Task.CompletedTask;
    }
}
