using HotelManagement.Services.Reservation.DTOs;
using HotelManagement.BuildingBlocks.Common.Exceptions;
using DataAccess.Dapper;
using DataAccess.DbConnectionProvider;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Services.Reservation.Services;

public class ReservationService : IReservationService
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IDapperDataRepository _dapperRepo;
    private readonly ILogger<ReservationService> _logger;

    public ReservationService(
        IDbConnectionFactory dbConnectionFactory,
        IDapperDataRepository dapperRepo,
        ILogger<ReservationService> logger)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _dapperRepo = dapperRepo;
        _logger = logger;
    }

    public async Task<ReservationResponse> CreateReservationAsync(CreateReservationRequest request)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            
            var reservation = new Models.Reservation
            {
                Id = Guid.NewGuid(),
                HotelId = request.HotelId,
                RoomTypeId = request.RoomTypeId,
                GuestId = request.GuestId,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                NumberOfRooms = request.NumberOfRooms,
                NumberOfGuests = request.NumberOfGuests,
                SpecialRequests = request.SpecialRequests,
                Status = Models.ReservationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                TotalPrice = 0, // This would need to be calculated based on business logic
                IsPaid = false
            };

            await _dapperRepo.AddAsync(reservation, db);
            
            return MapToResponse(reservation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating reservation");
            throw;
        }
    }

    public async Task<ReservationResponse> GetReservationByIdAsync(Guid id)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var result = await _dapperRepo.FindByIDAsync<Models.Reservation>(id, db);
            
            if (result == null)
            {
                throw new KeyNotFoundException($"Reservation with ID {id} not found.");
            }

            return MapToResponse(result);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reservation with ID {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<ReservationResponse>> GetReservationsByGuestIdAsync(Guid guestId)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            
            // Note: This would need a custom method or we'd need to implement a more specific query method
            // For now, returning empty as the current interface doesn't support this specific query
            _logger.LogWarning("GetReservationsByGuestIdAsync not fully implemented with current interface");
            return Enumerable.Empty<ReservationResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reservations for guest {GuestId}", guestId);
            throw;
        }
    }

    public async Task<IEnumerable<ReservationResponse>> GetReservationsByHotelIdAsync(Guid hotelId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            
            // Note: This would need a custom method or we'd need to implement a more specific query method
            // For now, returning empty as the current interface doesn't support this specific query
            _logger.LogWarning("GetReservationsByHotelIdAsync not fully implemented with current interface");
            return Enumerable.Empty<ReservationResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reservations for hotel {HotelId}", hotelId);
            throw;
        }
    }

    public async Task<ReservationResponse> UpdateReservationStatusAsync(Guid id, UpdateReservationRequest request)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            
            var reservation = await _dapperRepo.FindByIDAsync<Models.Reservation>(id, db);
            if (reservation == null)
            {
                throw new KeyNotFoundException($"Reservation with ID {id} not found.");
            }

            // Update status and related fields
            reservation.Status = ConvertToModelStatus(request.Status);
            if (request.Status == DTOs.ReservationStatus.Cancelled)
            {
                reservation.CancelledAt = DateTime.UtcNow;
                reservation.CancellationReason = request.CancellationReason;
            }
            
            reservation.ModifiedAt = DateTime.UtcNow;

            await _dapperRepo.UpdateAsync(reservation, reservation.Id, db);
            
            return MapToResponse(reservation);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for reservation with ID {Id}", id);
            throw;
        }
    }

    public async Task<bool> CancelReservationAsync(Guid id, string reason)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            
            var reservation = await _dapperRepo.FindByIDAsync<Models.Reservation>(id, db);
            if (reservation == null)
            {
                return false;
            }

            reservation.Status = Models.ReservationStatus.Cancelled;
            reservation.CancellationReason = reason;
            reservation.CancelledAt = DateTime.UtcNow;
            reservation.ModifiedAt = DateTime.UtcNow;

            var result = await _dapperRepo.UpdateAsync(reservation, reservation.Id, db);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling reservation with ID {Id}", id);
            return false;
        }
    }

    public async Task<bool> CheckInAsync(Guid id)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            
            var reservation = await _dapperRepo.FindByIDAsync<Models.Reservation>(id, db);
            if (reservation == null)
            {
                return false;
            }

            reservation.Status = Models.ReservationStatus.CheckedIn;
            reservation.ModifiedAt = DateTime.UtcNow;

            var result = await _dapperRepo.UpdateAsync(reservation, reservation.Id, db);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking in reservation with ID {Id}", id);
            return false;
        }
    }

    public async Task<bool> CheckOutAsync(Guid id)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            
            var reservation = await _dapperRepo.FindByIDAsync<Models.Reservation>(id, db);
            if (reservation == null)
            {
                return false;
            }

            reservation.Status = Models.ReservationStatus.CheckedOut;
            reservation.ModifiedAt = DateTime.UtcNow;

            var result = await _dapperRepo.UpdateAsync(reservation, reservation.Id, db);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking out reservation with ID {Id}", id);
            return false;
        }
    }

    public async Task<bool> MarkAsNoShowAsync(Guid id)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            
            var reservation = await _dapperRepo.FindByIDAsync<Models.Reservation>(id, db);
            if (reservation == null)
            {
                return false;
            }

            reservation.Status = Models.ReservationStatus.NoShow;
            reservation.ModifiedAt = DateTime.UtcNow;

            var result = await _dapperRepo.UpdateAsync(reservation, reservation.Id, db);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking reservation with ID {Id} as no-show", id);
            return false;
        }
    }

    public async Task<bool> UpdatePaymentStatusAsync(Guid id, bool isPaid, decimal? depositAmount = null)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            
            var reservation = await _dapperRepo.FindByIDAsync<Models.Reservation>(id, db);
            if (reservation == null)
            {
                throw new KeyNotFoundException($"Reservation with ID {id} not found.");
            }

            reservation.IsPaid = isPaid;
            if (depositAmount.HasValue)
                reservation.DepositAmount = depositAmount.Value;
            reservation.ModifiedAt = DateTime.UtcNow;

            var result = await _dapperRepo.UpdateAsync(reservation, reservation.Id, db);
            return result > 0;
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment status for reservation with ID {Id}", id);
            throw;
        }
    }

    private static Models.ReservationStatus ConvertToModelStatus(DTOs.ReservationStatus dtoStatus)
    {
        return dtoStatus switch
        {
            DTOs.ReservationStatus.Pending => Models.ReservationStatus.Pending,
            DTOs.ReservationStatus.Confirmed => Models.ReservationStatus.Confirmed,
            DTOs.ReservationStatus.Cancelled => Models.ReservationStatus.Cancelled,
            DTOs.ReservationStatus.CheckedIn => Models.ReservationStatus.CheckedIn,
            DTOs.ReservationStatus.CheckedOut => Models.ReservationStatus.CheckedOut,
            DTOs.ReservationStatus.NoShow => Models.ReservationStatus.NoShow,
            _ => Models.ReservationStatus.Pending
        };
    }

    private static DTOs.ReservationStatus ConvertToDtoStatus(Models.ReservationStatus modelStatus)
    {
        return modelStatus switch
        {
            Models.ReservationStatus.Pending => DTOs.ReservationStatus.Pending,
            Models.ReservationStatus.Confirmed => DTOs.ReservationStatus.Confirmed,
            Models.ReservationStatus.Cancelled => DTOs.ReservationStatus.Cancelled,
            Models.ReservationStatus.CheckedIn => DTOs.ReservationStatus.CheckedIn,
            Models.ReservationStatus.CheckedOut => DTOs.ReservationStatus.CheckedOut,
            Models.ReservationStatus.NoShow => DTOs.ReservationStatus.NoShow,
            _ => DTOs.ReservationStatus.Pending
        };
    }

    private static ReservationResponse MapToResponse(Models.Reservation reservation)
    {
        return new ReservationResponse
        {
            Id = reservation.Id,
            HotelId = reservation.HotelId,
            RoomTypeId = reservation.RoomTypeId,
            GuestId = reservation.GuestId,
            CheckInDate = reservation.CheckInDate,
            CheckOutDate = reservation.CheckOutDate,
            NumberOfRooms = reservation.NumberOfRooms,
            NumberOfGuests = reservation.NumberOfGuests,
            TotalPrice = reservation.TotalPrice,
            Status = ConvertToDtoStatus(reservation.Status),
            SpecialRequests = reservation.SpecialRequests,
            CreatedAt = reservation.CreatedAt,
            ModifiedAt = reservation.ModifiedAt,
            CancellationReason = reservation.CancellationReason,
            CancelledAt = reservation.CancelledAt,
            IsPaid = reservation.IsPaid,
            DepositAmount = reservation.DepositAmount
        };
    }
}
