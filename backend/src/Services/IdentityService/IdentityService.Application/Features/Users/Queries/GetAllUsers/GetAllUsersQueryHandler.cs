using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using MediatR;

namespace IdentityService.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync();

        return users
            .Select(u => new UserDto(
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                u.PhoneNumber,
                u.EmailConfirmed,
                u.IsActive,
                u.UserRoles.Select(ur => ur.Role.Name)))
            .ToList();
    }
}
