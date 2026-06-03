using ETS.Application.Abstraction.Common;
using ETS.Application.Abstraction.Mediation;
using ETS.Application.Users.Queries.Dto;
using ETS.Domain.Common;
using ETS.Domain.Entities;
using ETS.Domain.Enums;
using ETS.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ETS.Application.Users.Queries.GetPagedUsers
{
    public record GetPagedUsersQuery(string? SearchText, 
        DateOnly? StartDateOnly,
        DateOnly? EndDateOnly,
        RoleEnum? Role,
        string? SortField = "CreatedAt",
        bool IsAscending = false) : PaginationQuery, IQuery<PaginatedList<GetUserDto>>;


    public class GetPagedUsersQueryHandler : IQueryHandler<GetPagedUsersQuery, PaginatedList<GetUserDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetPagedUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        private static Expression<Func<User, object>> GetSortProperty(GetPagedUsersQuery request) =>
            request.SortField?.ToLower() switch
            {
                "id" => p => p.Id,
                "name" => p => p.FullName,
                "email" => p => p.EmailAddress,
                _ => p => p.CreatedAt
            };

        private static Expression<Func<User, bool>> GetQueryExpression(GetPagedUsersQuery request) => u =>
            (request.StartDateOnly == null || u.CreatedAt >= request.StartDateOnly.Value.ToDateTime(TimeOnly.MinValue)) &&
            (request.EndDateOnly == null || u.CreatedAt <= request.EndDateOnly.Value.ToDateTime(TimeOnly.MaxValue)) &&
            (string.IsNullOrWhiteSpace(request.SearchText) || 
                EF.Functions.Like(u.FullName, $"%{request.SearchText}%") ||
                EF.Functions.Like(u.PhoneNumber, $"%{request.SearchText}%") ||
                EF.Functions.Like(u.EmailAddress, $"%{request.SearchText}%"));

        private Expression<Func<User, GetUserDto>> Selector()
        {
            return v => new GetUserDto
            {
                Id = v.Id,
                DateCreated = v.CreatedAt,
                Email = v.EmailAddress,
                FullName = v.FullName,
                PhoneNumber = v.PhoneNumber,
                Role = v.Role.ToString()
            };
        }


        public async Task<Result<PaginatedList<GetUserDto>>> Handle(GetPagedUsersQuery request, 
            CancellationToken cancellationToken)
        {
            var filter = GetQueryExpression(request);
            var sort = GetSortProperty(request);

            var users = await _userRepository.GetPaginatedAsync<GetUserDto>(
                filter,
                Selector(),
                request.PageNumber,
                request.PageSize,
                sort,
                request.IsAscending,
                cancellationToken: cancellationToken);

            return users;
        }
    }

}
