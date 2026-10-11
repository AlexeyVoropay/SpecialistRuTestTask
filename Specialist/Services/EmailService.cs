using Specialist.DTOs;

namespace Specialist.Services;

/// <summary>
/// Интерфейс сервиса рассылок почты
/// </summary>
public interface IEmailService
{
    Task SendAsync(LeadRequestDto lead, CancellationToken ct = default);
}

/// <summary>
/// Сервис почты
/// </summary>
public class EmailService : IEmailService
{
    public Task SendAsync(LeadRequestDto lead, CancellationToken ct = default)
    {
        throw new InvalidOperationException("Почтовый сервер недоступен, ведутся технические работы.");
    }
}
