using ETS.Domain.Common;
using MediatR;

namespace ETS.Application.Abstraction.Mediation
{
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
      where TQuery : IQuery<TResponse>
    {
    }
}
