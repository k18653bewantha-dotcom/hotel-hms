using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Services;

public class RoomAvailabilityService
{
    private readonly HotelDbContext _context;

    public RoomAvailabilityService(HotelDbContext context)
    {
        _context = context;
    }

    public async Task<List<Room>> GetAvailableRoomsAsync(
        DateTime checkIn,
        DateTime checkOut,
        int? excludeBookingId = null)
    {
        if (checkOut <= checkIn)
        {
            return new List<Room>();
        }

        var activeBookingRooms = _context.BookingRooms
            .Where(br =>
                br.Booking != null &&
                br.Booking.Status != BookingStatuses.Cancelled &&
                br.Booking.Status != BookingStatuses.CheckedOut &&
                br.Booking.CheckIn < checkOut &&
                br.Booking.CheckOut > checkIn);

        if (excludeBookingId.HasValue)
        {
            activeBookingRooms = activeBookingRooms
                .Where(br => br.BookingId != excludeBookingId.Value);
        }

        var unavailableRoomIds = activeBookingRooms.Select(br => br.RoomId);

        return await _context.Rooms
            .AsNoTracking()
            .Where(r =>
                r.IsActive &&
                r.Status != RoomStatuses.Maintenance &&
                !unavailableRoomIds.Contains(r.Id))
            .OrderBy(r => r.RoomNumber)
            .ToListAsync();
    }

    public async Task<bool> AreRoomsAvailableAsync(
        IEnumerable<int> roomIds,
        DateTime checkIn,
        DateTime checkOut,
        int? excludeBookingId = null)
    {
        var requestedIds = roomIds.Distinct().ToList();

        if (requestedIds.Count == 0)
        {
            return false;
        }

        var availableIds = await GetAvailableRoomsAsync(
            checkIn,
            checkOut,
            excludeBookingId);

        var set = availableIds.Select(r => r.Id).ToHashSet();
        return requestedIds.All(set.Contains);
    }
}
