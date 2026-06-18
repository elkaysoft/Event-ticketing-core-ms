using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Errors;
using ETS.Domain.Models;
using ETS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ETS.Application.Authentication.Queries.GetProfileDetails
{
    public record GetProfileDetailsQuery() : IQuery<TokenUser>;

    public class GetProfileDetailsQueryHandler : IQueryHandler<GetProfileDetailsQuery, TokenUser>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserContext _userContext;
        private readonly ILogger<GetProfileDetailsQueryHandler> _logger;

        public GetProfileDetailsQueryHandler(IUserRepository userRepository,
            IUserContext userContext,
            ILogger<GetProfileDetailsQueryHandler> logger)
        {
            _userRepository = userRepository;
            _userContext = userContext;
            _logger = logger;
        }

        public async Task<Result<TokenUser>> Handle(GetProfileDetailsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetSingleAsync(x => x.EmailAddress == _userContext.UserEmail, cancellationToken);
                if (user == null) 
                {
                    return Result.Failure<TokenUser>(UserErrors.NotFound);
                }

                return new TokenUser
                {
                    Email = user.EmailAddress,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role.ToString(),
                    UserId = user.Id
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occured in {nameof(GetProfileDetailsQueryHandler)}");
                return Result.Failure<TokenUser>(UserErrors.SomethingWentWrong);
            }
        }
    }

}
