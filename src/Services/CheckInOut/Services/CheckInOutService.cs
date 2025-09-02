using DataAccess;
using HotelManagement.Services.CheckInOut.DTOs;
using HotelManagement.Services.CheckInOut.Models;
using HotelManagement.Services.CheckInOut.SpInput;

namespace HotelManagement.Services.CheckInOut.Services;

public class CheckInOutService : ICheckInOutService
{
    private readonly IDataRepository _dataRepository;
    private readonly ILogger<CheckInOutService> _logger;

    public CheckInOutService(IDataRepository dataRepository, ILogger<CheckInOutService> logger)
    {
        _dataRepository = dataRepository;
        _logger = logger;
    }

    public async Task<CheckInOutResponse> CheckInAsync(CheckInRequest request)
    {
        try
        {
            // Check if a record already exists
            var existingRecord = await GetByReservationIdAsync(request.ReservationId);
            
            if (existingRecord == null)
            {
                // Create a new check-in record
                var parameters = new CheckInParams
                {
                    Id = Guid.NewGuid(),
                    ReservationId = request.ReservationId,
                    GuestId = request.GuestId,
                    CheckInTime = DateTime.UtcNow,
                    Status = CheckInOutStatus.CheckedIn,
                    Notes = request.Notes
                };
                
                var record = await _dataRepository.QueryFirstOrDefaultAsync<CheckInOutRecord>(parameters);
                return MapToResponse(record);
            }
            else
            {
                // Update the existing record
                var parameters = new CheckInParams
                {
                    Id = existingRecord.Id,
                    ReservationId = request.ReservationId,
                    GuestId = request.GuestId,
                    CheckInTime = DateTime.UtcNow,
                    Status = CheckInOutStatus.CheckedIn,
                    Notes = request.Notes
                };
                
                var record = await _dataRepository.QueryFirstOrDefaultAsync<CheckInOutRecord>(parameters);
                return MapToResponse(record);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during check-in for reservation {ReservationId}", request.ReservationId);
            throw;
        }
    }

    public async Task<CheckInOutResponse> CheckOutAsync(CheckOutRequest request)
    {
        try
        {
            var parameters = new CheckOutParams
            {
                ReservationId = request.ReservationId,
                CheckOutTime = DateTime.UtcNow,
                Status = CheckInOutStatus.CheckedOut,
                Notes = request.Notes
            };
            
            var record = await _dataRepository.QueryFirstOrDefaultAsync<CheckInOutRecord>(parameters);
            
            if (record == null)
            {
                throw new Exception("Check-in record not found");
            }
            
            return MapToResponse(record);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during check-out for reservation {ReservationId}", request.ReservationId);
            throw;
        }
    }

    public async Task<CheckInOutResponse?> GetByReservationIdAsync(Guid reservationId)
    {
        try
        {
            var parameters = new GetCheckInOutByReservationIdParams
            {
                ReservationId = reservationId
            };
            
            var record = await _dataRepository.QueryFirstOrDefaultAsync<CheckInOutRecord>(parameters);
            return record == null ? null : MapToResponse(record);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving check-in/out record for reservation {ReservationId}", reservationId);
            throw;
        }
    }

    public async Task<bool> CompensateCancelAsync(Guid reservationId)
    {
        try
        {
            var parameters = new CancelCheckInOutParams
            {
                ReservationId = reservationId,
                Status = CheckInOutStatus.Cancelled
            };
            
            var result = await _dataRepository.ExecuteAsync(parameters);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling check-in/out record for reservation {ReservationId}", reservationId);
            throw;
        }
    }

    private static CheckInOutResponse MapToResponse(CheckInOutRecord record) => new()
    {
        Id = record.Id,
        ReservationId = record.ReservationId,
        GuestId = record.GuestId,
        CheckInTime = record.CheckInTime,
        CheckOutTime = record.CheckOutTime,
        Status = record.Status.ToString(),
        Notes = record.Notes
    };
}
