using MediatR;

namespace HotelManagement.BuildingBlocks.Common.Domain;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
    Guid Id { get; }
}
