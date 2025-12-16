using FluentValidation;
using OrderService.Domain.Enums;

namespace OrderService.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(BeValidStatus).WithMessage($"Invalid status value. Valid values: {GetValidStatusValues()}");
    }

    private bool BeValidStatus(string status)
    {
        return Enum.TryParse<OrderStatus>(status, true, out _);
    }

    private static string GetValidStatusValues()
    {
        return string.Join(", ", Enum.GetNames<OrderStatus>());
    }
}
