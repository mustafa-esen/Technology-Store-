using IdentityService.Application.DTOs;
using MediatR;

namespace IdentityService.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<List<UserDto>>
{
}
