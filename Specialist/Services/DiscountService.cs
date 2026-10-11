using Specialist.Models;

namespace Specialist.Services;

/// <summary>
/// Интерфейс сервиса расчёта скидок
/// </summary>
public interface IDiscountService
{
    /// <summary>
    /// Рассчитывает итоговую стоимость корзины.
    /// </summary>
    /// <param name="courses">Список курсов в корзине.</param>
    /// <param name="isGraduate">Признак профиля пользователя, что он является выпускником.</param>
    /// <param name="date">Дата, на которую производится расчёт.</param>
    decimal CalculateTotal(
        IEnumerable<Course> courses,
        bool isGraduate,
        DateTime? date = null);
}

/// <summary>
/// Сервис расчёта скидок
/// </summary>
public class DiscountService : IDiscountService
{
    /// <summary>
    /// Коофициент скидки для горящих групп
    /// </summary>
    private const decimal HotGroupDiscountRate = 0.20m; // 20%

    /// <summary>
    /// Коофициент скидки для выпускников
    /// </summary>
    private const decimal GraduateDiscountRate = 0.30m; // 30%

    /// <summary>
    /// Порог дней для горящих групп
    /// </summary>
    private const int HotGroupThresholdDays = 5; // 5 дней

    public decimal CalculateTotal(
        IEnumerable<Course> courses,
        bool isGraduate,
        DateTime? date = null)
    {
        if (courses is null) 
            throw new ArgumentNullException(nameof(courses));

        // При расчёте количества дней считаем разницу дат без времени
        var today = (date ?? DateTime.UtcNow).Date;

        decimal total = 0m;

        foreach (var course in courses)
        {
            var daysUntilStart = (course.StartDate.Date - today).TotalDays;

            // Скидка «Горящие группы»
            decimal hotDiscount =
                daysUntilStart < HotGroupThresholdDays
                    ? HotGroupDiscountRate
                    : 0m;

            // Скидка «Выпускник»
            decimal graduateDiscount =
                isGraduate
                    ? GraduateDiscountRate
                    : 0m;

            // Скидки не суммируются, берём максимальную
            decimal appliedDiscount = Math.Max(hotDiscount, graduateDiscount);

            // Итоговая цена курса
            decimal finalPrice = course.Price * (1m - appliedDiscount);
            total += finalPrice;
        }

        return Math.Round(total, 2, MidpointRounding.AwayFromZero);
    }
}
