using System.ComponentModel.DataAnnotations;
using Specialist.Enums;

namespace Specialist.DTOs;

public class LeadRequestDto
{
    [Required(ErrorMessage = "Имя обязательно")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Телефон обязателен")]
    [RegularExpression(@"^\+?[0-9\s\-\(\)]{7,20}$",
        ErrorMessage = "Некорректный формат телефона")]
    //[RegularExpression(@"^(\+7|8)[\s\-\(\)0-9]{9,15}$",
    //  ErrorMessage = "Телефон должен быть в российском формате")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email обязателен")]
    //[EmailAddress(ErrorMessage = "Некорректный Email")]
    [RegularExpression(@"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}$",
        ErrorMessage = "Некорректный Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [EnumDataType(typeof(ClientType))]
    public ClientType ClientType { get; set; }
}
