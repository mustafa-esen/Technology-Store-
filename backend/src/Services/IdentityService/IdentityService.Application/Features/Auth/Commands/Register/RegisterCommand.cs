using MediatR;
using IdentityService.Application.DTOs;

namespace IdentityService.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber
) : IRequest<AuthResponseDto>;
