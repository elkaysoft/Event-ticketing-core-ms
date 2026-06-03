using ETS.Application.Abstraction.Mediation;
using ETS.Application.Users.Queries.Dto;
using ETS.Domain.Common;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;

namespace ETS.Application.Users.Queries.GetSingleUser
{
    public record GetSingleUserQuery(long UserId) : IQuery<GetUserDto>;

    public class GetSingleUserQueryHandler : IQueryHandler<GetSingleUserQuery, GetUserDto>
    {
        private readonly IUserRepository _userRepository;
        public GetSingleUserQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Result<GetUserDto>> Handle(GetSingleUserQuery request, CancellationToken cancellationToken)
        {
            var userResult = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (userResult is null)
            {
                return Result.Failure<GetUserDto>(UserErrors.NotFound);
            }
            var userDto = new GetUserDto
            {
                Id = userResult.Id,
                FullName = userResult.FullName,
                Email = userResult.EmailAddress,
                PhoneNumber = userResult.PhoneNumber,
                Role = userResult.Role.ToString(),
                DateCreated = userResult.CreatedAt
            };
            return Result.Success(userDto);
        }
    }

 }
