using Microsoft.AspNetCore.Mvc;
using Specialist.DTOs;
using Specialist.Services;

namespace Specialist.Controllers;

/// <summary>
/// Контроллер корзины курсов
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CartController(IDiscountService discountService) 
    : ControllerBase
{
    [HttpPost("calculate")]
    public IActionResult Calculate([FromBody] CartRequest request)
    {
        var total = discountService.CalculateTotal(request.Courses, request.IsGraduate);
        return Ok(new { Total = total });
    }
}