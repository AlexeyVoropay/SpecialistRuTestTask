using Specialist.Models;
using Specialist.Services;

namespace Specialist.Test;

public class DiscountServiceTests
{
    private readonly DiscountService _discountService = new();
    private readonly DateTime _date = new(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    private static Course MakeCourse(decimal price, DateTime start) =>
        new() { Id = 1, Title = "Test", Price = price, StartDate = start };

    // 01. Горящая группа: < 5 дней => −20%
    [Fact]
    public void HotGroup_Discount20_WhenStartInLessThan5Days()
    {
        var course = MakeCourse(1000m, _date.AddDays(3));
        var isGraduate = false;

        var total = _discountService.CalculateTotal(new[] { course }, isGraduate, _date);

        Assert.Equal(800m, total);
    }

    // 02. Не горящая: ровно 5 дней => скидки нет
    [Fact]
    public void HotGroup_NoDiscount_WhenStartInExactly5Days()
    {
        var course = MakeCourse(1000m, _date.AddDays(5));
        var isGraduate = false;

        var total = _discountService.CalculateTotal([course], isGraduate, _date);

        Assert.Equal(1000m, total);
    }

    // 03. Выпускник: −30%
    [Fact]
    public void Graduate_Discount30_WhenCourseNotHot()
    {
        var course = MakeCourse(1000m, _date.AddDays(30));
        var isGraduate = true;

        var total = _discountService.CalculateTotal([course], isGraduate, _date);

        Assert.Equal(700m, total);
    }

    // 04. Правило «не суммировать»: горящий + выпускник => −30 %, не −50 %
    [Fact]
    public void Discounts_NotSummed_MaxIsApplied()
    {
        var course = MakeCourse(1000m, _date.AddDays(2));
        var isGraduate = true;

        var total = _discountService.CalculateTotal([course], isGraduate, _date);

        // 30% (выпускник) > 20% (горящая) => применяется 30 %
        Assert.Equal(700m, total);
    }

    // 05. Корзина из нескольких курсов, разные скидки
    [Fact]
    public void MultipleCourses_EachGetsItsOwnMaxDiscount()
    {
        var hot = MakeCourse(1000m, _date.AddDays(1)); // −20% => 800
        var far = MakeCourse(2000m, _date.AddDays(60)); // без скидки => 2000
        var isGraduate = false;

        var total = _discountService.CalculateTotal([hot, far], isGraduate, _date);

        Assert.Equal(2800m, total);
    }

    // 06. Пустая корзина
    [Fact]
    public void EmptyCart_ReturnsZero()
    {
        var isGraduate = false;
        var total = _discountService.CalculateTotal([], isGraduate, _date);

        Assert.Equal(0m, total);
    }
}