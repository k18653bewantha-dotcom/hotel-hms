using HotelBookingSystem.Models;

namespace HotelBookingSystem.Services;

public class BookingBillingService
{
    public void Recalculate(Booking booking)
    {
        booking.RoomCharge = Math.Max(0, booking.RoomCharge);
        booking.RestaurantBill = Math.Max(0, booking.RestaurantBill);
        booking.BarBill = Math.Max(0, booking.BarBill);
        booking.LaundryBill = Math.Max(0, booking.LaundryBill);
        booking.SeasonalChargePercent = Math.Max(0, booking.SeasonalChargePercent);
        booking.Discount = Math.Max(0, booking.Discount);
        booking.AdvancePayment = Math.Max(0, booking.AdvancePayment);

        booking.SeasonalChargeAmount =
            booking.RoomCharge * booking.SeasonalChargePercent / 100m;

        var total = booking.RoomCharge
                    + booking.SeasonalChargeAmount
                    + booking.RestaurantBill
                    + booking.BarBill
                    + booking.LaundryBill
                    - booking.Discount;

        booking.GrandTotal = Math.Max(0, total);
    }
}
