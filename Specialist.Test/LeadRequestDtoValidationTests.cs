using System.ComponentModel.DataAnnotations;
using Specialist.DTOs;
using Specialist.Enums;

namespace Specialist.Test;

public class LeadRequestDtoValidationTests
{
    private static (bool IsValid, List<ValidationResult> Errors) Validate(LeadRequestDto dto)
    {
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        bool ok = Validator.TryValidateObject(dto, context, results, validateAllProperties: true);
        return (ok, results);
    }

    private static LeadRequestDto ValidDto() => new()
    {
        Name = "Иван",
        Phone = "+79991234567",
        Email = "ivan@test.com",
        ClientType = ClientType.Individual
    };

    // 01.Правильная почта
    [Theory]
    [InlineData("ivan@test.com")]
    [InlineData("i.ivanov@corp.company.ru")]
    [InlineData("user+tag@sub.domain.io")]
    public void Email_Valid_Passes(string email)
    {
        var dto = ValidDto();
        dto.Email = email;

        var (isValid, _) = Validate(dto);

        Assert.True(isValid, $"Email '{email}' должен считаться валидным");
    }

    // 02. Неправильная почта
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-an-email")]
    [InlineData("user@")]
    [InlineData("@domain.com")]
    [InlineData("user@domain")]
    [InlineData("user@domain.")]
    public void Email_Invalid_Fails(string email)
    {
        var dto = ValidDto();
        dto.Email = email;

        var (isValid, errors) = Validate(dto);

        Assert.False(isValid);
        Assert.Contains(errors, e =>
            e.MemberNames.Contains(nameof(LeadRequestDto.Email)));
    }

    // 03. Правильный телефон
    [Theory]
    [InlineData("+79991234567")]
    [InlineData("89991234567")]
    [InlineData("+7 999 123-45-67")]
    [InlineData("(999) 123-45-67")]
    [InlineData("+7(999)1234567")]
    public void Phone_Valid_Passes(string phone)
    {
        var dto = ValidDto();
        dto.Phone = phone;

        var (isValid, _) = Validate(dto);

        Assert.True(isValid, $"Телефон '{phone}' должен считаться валидным");
    }
    // 04. Не правильный телефон
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123")]              // слишком короткий
    [InlineData("abcdefg")]          // буквы
    [InlineData("+7 999 123 45 67 89 00 11")] // слишком длинный
    [InlineData("++79991234567")]    // двойной плюс
    public void Phone_Invalid_Fails(string phone)
    {
        var dto = ValidDto();
        dto.Phone = phone;

        var (isValid, errors) = Validate(dto);

        Assert.False(isValid);
        Assert.Contains(errors, e =>
            e.MemberNames.Contains(nameof(LeadRequestDto.Phone)));
    }

    // 05. Комплексный сценарий 1
    [Fact]
    public void Dto_WithInvalidEmailAndPhone_ReturnsBothErrors()
    {
        var dto = ValidDto();
        dto.Email = "bad-email";
        dto.Phone = "abc";

        var (isValid, errors) = Validate(dto);

        Assert.False(isValid);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(LeadRequestDto.Email)));
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(LeadRequestDto.Phone)));
    }

    // 06. Комплексный сценарий 2
    [Fact]
    public void ValidDto_PassesAllValidation()
    {
        var (isValid, errors) = Validate(ValidDto());

        Assert.True(isValid);
        Assert.Empty(errors);
    }
}