using Microsoft.AspNetCore.Mvc;
using Specialist.DTOs;
using Specialist.Services;

namespace Specialist.Controllers;

/// <summary>
/// Контроллер лидов
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LeadsController(
    IEmailService emailService,
    IFallbackEmailService fallbackEmailService,
    ILogger<LeadsController> logger)
    : ControllerBase
{
    /// <summary>
    /// POST /api/leads — заявка «Обратный звонок».
    /// При сбое основной почты заявка уходит на резервный адрес,
    /// клиент всегда получает 200 OK.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(LeadResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateLead(
        [FromBody] LeadRequestDto request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return new ObjectResult(new ValidationProblemDetails(ModelState))
            {
                StatusCode = StatusCodes.Status400BadRequest,
                ContentTypes = { "application/problem+json" }
            };
        }

        try
        {
            await emailService.SendAsync(request, ct);
            logger.LogInformation("Заявка успешно отправлена.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Почтовый сервис недоступен. Переключаемся на резервный.");

            try
            {
                await fallbackEmailService.SendAsync(request, ct);
                logger.LogInformation("Заявка отправлена через резервный сервис.");
            }
            catch (Exception fallbackEx)
            {
                logger.LogError(fallbackEx, "Резервный сервис недоступен.");
                // TODO: Cохранить заявку в БД для повторной отправки.
            }
        }

        return Ok(new LeadResponseDto
        {
            Success = true,
            Message = "Спасибо! Мы свяжемся с вами в ближайшее время."
        });
    }
}
