using ETS.Domain.Common;
using MediatR;

namespace ETS.Application.Abstraction.Mediation
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
