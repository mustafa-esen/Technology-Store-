using MediatR;
using IdentityService.Application.DTOs;

namespace IdentityService.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<AuthResponseDto>;
