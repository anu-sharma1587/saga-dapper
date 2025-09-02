using MediatR;

namespace HotelManagement.BuildingBlocks.Common.CQRS;

public interface ICommand : IRequest
{
}

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
