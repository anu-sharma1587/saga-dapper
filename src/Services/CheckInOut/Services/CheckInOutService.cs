using DataAccess.Dapper;
using DataAccess.DbConnectionProvider;
using HotelManagement.Services.CheckInOut.DTOs;
using HotelManagement.Services.CheckInOut.Models;
using HotelManagement.Services.CheckInOut.SpInput;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Services.CheckInOut.Services;

public class CheckInOutService : ICheckInOutService
{
    private readonly IDapperDataRepository _dataRepository;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<CheckInOutService> _logger;

    public CheckInOutService(IDapperDataRepository dataRepository, IDbConnectionFactory connectionFactory, ILogger<CheckInOutService> logger)
    {
        _dataRepository = dataRepository;
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<CheckInOutResponse> CheckInAsync(CheckInRequest request)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            
            // Check if a record already exists
            var existingRecord = await GetRecordByReservationIdAsync(request.ReservationId);
            
            if (existingRecord == null)
            {
                // Create a new check-in record
                var checkInRecord = new CheckInOutRecord
                {
                    Id = Guid.NewGuid(),
                    ReservationId = request.ReservationId,
                    GuestId = request.GuestId,
                    CheckInTime = DateTime.UtcNow,
                    Status = CheckInOutStatus.CheckedIn,
                    Notes = request.Notes
                };
                
                await _dataRepository.AddAsync(checkInRecord, connection);
                return MapToResponse(checkInRecord);
            }
            else
            {
                // Update the existing record
                existingRecord.CheckInTime = DateTime.UtcNow;
                existingRecord.Status = CheckInOutStatus.CheckedIn;
                existingRecord.Notes = request.Notes;
                
                await _dataRepository.UpdateAsync(existingRecord, existingRecord.Id, connection);
                return MapToResponse(existingRecord);
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
            using var connection = await _connectionFactory.CreateAsync();
            
            var existingRecord = await GetRecordByReservationIdAsync(request.ReservationId);
            if (existingRecord == null)
            {
                throw new Exception("Check-in record not found");
            }
            
            existingRecord.CheckOutTime = DateTime.UtcNow;
            existingRecord.Status = CheckInOutStatus.CheckedOut;
            existingRecord.Notes = request.Notes;
            
            await _dataRepository.UpdateAsync(existingRecord, existingRecord.Id, connection);
            return MapToResponse(existingRecord);
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
            var record = await GetRecordByReservationIdAsync(reservationId);
            return record != null ? MapToResponse(record) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving check-in/out record for reservation {ReservationId}", reservationId);
            return null;
        }
    }

    public async Task<bool> CompensateCancelAsync(Guid reservationId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            
            var existingRecord = await GetRecordByReservationIdAsync(reservationId);
            if (existingRecord == null)
            {
                return false;
            }
            
            existingRecord.Status = CheckInOutStatus.Cancelled;
            var result = await _dataRepository.UpdateAsync(existingRecord, existingRecord.Id, connection);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling check-in/out record for reservation {ReservationId}", reservationId);
            return false;
        }
    }

    private async Task<CheckInOutRecord?> GetRecordByReservationIdAsync(Guid reservationId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            
            // For now, we'll create a mock record since the current interface doesn't support this specific query
            // In a real implementation, you would add a method to IDapperDataRepository for this
            _logger.LogWarning("GetRecordByReservationIdAsync using mock implementation - needs proper database query method");
            
            // Mock implementation - replace with actual database query
            var mockRecord = new CheckInOutRecord
            {
                Id = Guid.NewGuid(),
                ReservationId = reservationId,
                GuestId = Guid.NewGuid(), // This should come from the reservation
                CheckInTime = null,
                CheckOutTime = null,
                Status = CheckInOutStatus.Pending,
                Notes = null
            };
            
            return mockRecord;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving check-in/out record for reservation {ReservationId}", reservationId);
            return null;
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
