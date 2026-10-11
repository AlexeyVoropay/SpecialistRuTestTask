using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Specialist.Controllers;
using Specialist.DTOs;
using Specialist.Enums;
using Specialist.Services;

namespace Specialist.Test;

public class LeadsControllerValidationTests
{
    private static LeadsController CreateController()
    {
        return new LeadsController(
            new EmailService(),
            new FallbackEmailService(),
            NullLogger<LeadsController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    /// <summary>
    /// Прогоняет DTO через DataAnnotations и, если есть ошибки,
    /// наполняет ModelState — как это делает ASP.NET Core.
    /// </summary>
    private static void PopulateModelState(ControllerBase controller, object dto)
    {
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(dto, context, results, validateAllProperties: true);

        foreach (var result in results)
        {
            foreach (var member in result.MemberNames)
            {
                controller.ModelState.AddModelError(member, result.ErrorMessage ?? "Invalid");
            }
        }
    }

    [Theory]
    [InlineData("not-an-email", "+79991234567")]
    [InlineData("ivan@test.com", "abc")]
    [InlineData("bad", "123")]
    public async Task CreateLead_Returns400_ForInvalidEmailOrPhone(string email, string phone)
    {
        // Arrange
        var controller = CreateController();
        var dto = new LeadRequestDto
        {
            Name = "Иван",
            Phone = phone,
            Email = email,
            ClientType = ClientType.Individual
        };
        PopulateModelState(controller, dto);

        // Act
        var result = await controller.CreateLead(dto, CancellationToken.None);

        // Assert
        Assert.IsType<ObjectResult>(result); // ValidationProblem → ObjectResult с 400
        var obj = (ObjectResult)result;
        Assert.Equal(StatusCodes.Status400BadRequest, obj.StatusCode);
    }

    [Fact]
    public async Task CreateLead_ReturnsOk_ForValidEmailAndPhone()
    {
        var controller = CreateController();
        var dto = new LeadRequestDto
        {
            Name = "Иван",
            Phone = "+7 (999) 123-45-67",
            Email = "ivan@test.com",
            ClientType = ClientType.Individual
        };
        PopulateModelState(controller, dto);

        var result = await controller.CreateLead(dto, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LeadResponseDto>(ok.Value);
        Assert.True(response.Success);
    }
}