using MediatR;
using IdentityService.Application.DTOs;

namespace IdentityService.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<AuthResponseDto>;
