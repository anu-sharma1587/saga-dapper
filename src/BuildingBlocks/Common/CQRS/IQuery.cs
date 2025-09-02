using MediatR;

namespace HotelManagement.BuildingBlocks.Common.CQRS;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
