using HotelManagement.Services.Reservation.DTOs;
using HotelManagement.Services.Reservation.SpInput;
using HotelManagement.BuildingBlocks.Common.Exceptions;
using DataAccess;

namespace HotelManagement.Services.Reservation.Services;

public class ReservationService : IReservationService
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IDapperDataRepository _dapperRepo;
    private readonly ILogger<ReservationService> _logger;
    private readonly HttpClient _availabilityClient;

    public ReservationService(
        IDbConnectionFactory dbConnectionFactory,
        IDapperDataRepository dapperRepo,
        ILogger<ReservationService> logger,
        HttpClient availabilityClient)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _dapperRepo = dapperRepo;
        _logger = logger;
        _availabilityClient = availabilityClient;
    }

    public async Task<ReservationResponse> CreateReservationAsync(CreateReservationRequest request)
    {
        // Validate availability
        var isAvailable = await CheckAvailabilityAsync(request.HotelId, request.RoomTypeId, 
            request.CheckInDate, request.CheckOutDate, request.NumberOfRooms);
        
        if (!isAvailable)
        {
            throw new BusinessException("Requested rooms are not available for the selected dates.");
        }

        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new CreateReservationParams
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
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                TotalPrice = 0, // This will be calculated by the stored procedure
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<Models.Reservation, CreateReservationParams>(param, db)).FirstOrDefault();
            
            if (result == null)
            {
                throw new Exception("Failed to create reservation");
            }

            return MapToResponse(result);
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
            var param = new GetReservationByIdParams
            {
                Id = id,
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<Models.Reservation, GetReservationByIdParams>(param, db)).FirstOrDefault();
            
            if (result == null)
            {
                throw new NotFoundException($"Reservation with ID {id} not found.");
            }

            return MapToResponse(result);
        }
        catch (NotFoundException)
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
            var param = new GetReservationsByGuestIdParams
            {
                GuestId = guestId,
                p_refcur_1 = null
            };

            var results = await _dapperRepo.ExecuteSpQueryAsync<Models.Reservation, GetReservationsByGuestIdParams>(param, db);
            
            return results.Select(MapToResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reservations for guest with ID {GuestId}", guestId);
            throw;
        }
    }

    public async Task<IEnumerable<ReservationResponse>> GetReservationsByHotelIdAsync(
        Guid hotelId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new GetReservationsByHotelIdParams
            {
                HotelId = hotelId,
                FromDate = fromDate,
                ToDate = toDate,
                p_refcur_1 = null
            };

            var results = await _dapperRepo.ExecuteSpQueryAsync<Models.Reservation, GetReservationsByHotelIdParams>(param, db);
            
            return results.Select(MapToResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reservations for hotel with ID {HotelId}", hotelId);
            throw;
        }
    }

    public async Task<ReservationResponse> UpdateReservationStatusAsync(Guid id, UpdateReservationRequest request)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new UpdateReservationStatusParams
            {
                Id = id,
                Status = request.Status.ToString(),
                CancellationReason = request.CancellationReason,
                ModifiedAt = DateTime.UtcNow,
                CancelledAt = request.Status == ReservationStatus.Cancelled ? DateTime.UtcNow : null,
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<Models.Reservation, UpdateReservationStatusParams>(param, db)).FirstOrDefault();
            
            if (result == null)
            {
                throw new NotFoundException($"Reservation with ID {id} not found.");
            }

            return MapToResponse(result);
        }
        catch (NotFoundException)
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
            // First get the reservation to validate it can be canceled
            var reservation = await GetReservationByIdAsync(id);
            
            if (reservation.Status == ReservationStatus.CheckedIn || 
                reservation.Status == ReservationStatus.CheckedOut)
            {
                throw new BusinessException("Cannot cancel a reservation that is checked in or checked out.");
            }
            
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new CancelReservationParams
            {
                Id = id,
                CancellationReason = reason,
                CancelledAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<Models.Reservation, CancelReservationParams>(param, db)).FirstOrDefault();
            
            return result != null;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling reservation with ID {Id}", id);
            throw;
        }
    }

    public async Task<bool> CheckInAsync(Guid id)
    {
        try
        {
            // First get the reservation to validate its status
            var reservation = await GetReservationByIdAsync(id);
            
            if (reservation.Status != ReservationStatus.Confirmed)
            {
                throw new BusinessException("Only confirmed reservations can be checked in.");
            }

            if (!reservation.IsPaid)
            {
                throw new BusinessException("Cannot check in unpaid reservation.");
            }
            
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new CheckInParams
            {
                Id = id,
                ModifiedAt = DateTime.UtcNow,
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<Models.Reservation, CheckInParams>(param, db)).FirstOrDefault();
            
            return result != null;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking in reservation with ID {Id}", id);
            throw;
        }
    }

    public async Task<bool> CheckOutAsync(Guid id)
    {
        try
        {
            // First get the reservation to validate its status
            var reservation = await GetReservationByIdAsync(id);
            
            if (reservation.Status != ReservationStatus.CheckedIn)
            {
                throw new BusinessException("Only checked-in reservations can be checked out.");
            }
            
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new CheckOutParams
            {
                Id = id,
                ModifiedAt = DateTime.UtcNow,
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<Models.Reservation, CheckOutParams>(param, db)).FirstOrDefault();
            
            return result != null;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking out reservation with ID {Id}", id);
            throw;
        }
    }

    public async Task<bool> MarkAsNoShowAsync(Guid id)
    {
        try
        {
            // First get the reservation to validate its status
            var reservation = await GetReservationByIdAsync(id);
            
            if (reservation.Status != ReservationStatus.Confirmed)
            {
                throw new BusinessException("Only confirmed reservations can be marked as no-show.");
            }
            
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new MarkAsNoShowParams
            {
                Id = id,
                ModifiedAt = DateTime.UtcNow,
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<Models.Reservation, MarkAsNoShowParams>(param, db)).FirstOrDefault();
            
            return result != null;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking reservation with ID {Id} as no-show", id);
            throw;
        }
    }

    public async Task<bool> UpdatePaymentStatusAsync(Guid id, bool isPaid, decimal? depositAmount = null)
    {
        try
        {
            using var db = await _dbConnectionFactory.CreateAsync();
            var param = new UpdatePaymentStatusParams
            {
                Id = id,
                IsPaid = isPaid,
                DepositAmount = depositAmount,
                ModifiedAt = DateTime.UtcNow,
                p_refcur_1 = null
            };

            var result = (await _dapperRepo.ExecuteSpQueryAsync<Models.Reservation, UpdatePaymentStatusParams>(param, db)).FirstOrDefault();
            
            if (result == null)
            {
                throw new NotFoundException($"Reservation with ID {id} not found.");
            }

            return true;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment status for reservation with ID {Id}", id);
            throw;
        }
    }

    private async Task<bool> CheckAvailabilityAsync(
        Guid hotelId, Guid roomTypeId, DateTime checkIn, DateTime checkOut, int numberOfRooms)
    {
        try
        {
            var response = await _availabilityClient.GetAsync(
                $"/api/availability/check?hotelId={hotelId}&roomTypeId={roomTypeId}" +
                $"&checkIn={checkIn:yyyy-MM-dd}&checkOut={checkOut:yyyy-MM-dd}&rooms={numberOfRooms}");

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking availability");
            throw new BusinessException("Unable to verify room availability. Please try again later.");
        }
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
            Status = reservation.Status,
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
