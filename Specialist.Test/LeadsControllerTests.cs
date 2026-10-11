using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Specialist.Controllers;
using Specialist.DTOs;
using Specialist.Enums;
using Specialist.Services;

namespace Specialist.Test;

public class LeadsControllerTests
{
    [Fact]
    public async Task CreateLead_ReturnsOk_And_UsesFallback_WhenPrimaryFails()
    {
        // Arrange
        var primary = new EmailService();       // всегда падает
        var fallback = new FallbackEmailService();
        var controller = new LeadsController(
            primary, fallback, NullLogger<LeadsController>.Instance);

        var request = new LeadRequestDto
        {
            Name = "Иван",
            Phone = "+79991234567",
            Email = "ivan@test.com",
            ClientType = ClientType.Corporate
        };

        // Act
        var result = await controller.CreateLead(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<LeadResponseDto>(okResult.Value);
        Assert.True(dto.Success);

        // Проверяем, что письмо ушло на правильный резервный адрес
        Assert.Single(fallback.SentEmails);
        Assert.Equal("specialist.corp@yandex.ru", fallback.SentEmails[0].To);
    }

    [Fact]
    public async Task CreateLead_Individual_UsesCorrectFallbackAddress()
    {
        var primary = new EmailService();
        var fallback = new FallbackEmailService();
        var controller = new LeadsController(
            primary, fallback, NullLogger<LeadsController>.Instance);

        var request = new LeadRequestDto
        {
            Name = "Пётр",
            Phone = "+79990001122",
            Email = "petr@test.com",
            ClientType = ClientType.Individual
        };

        await controller.CreateLead(request, CancellationToken.None);

        Assert.Single(fallback.SentEmails);
        Assert.Equal("specialist.ufl@yandex.ru", fallback.SentEmails[0].To);
    }
}
