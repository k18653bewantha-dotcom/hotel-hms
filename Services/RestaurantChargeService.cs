using HotelBookingSystem.Models;

namespace HotelBookingSystem.Services;

public class RestaurantChargeService
{
    private readonly BookingBillingService _billing;

    public RestaurantChargeService(BookingBillingService billing)
    {
        _billing = billing;
    }

    public void ApplyStatusChange(Order order, string newStatus)
    {
        if (order.OrderType == OrderTypes.RoomCharge && order.Booking != null)
        {
            if (newStatus == OrderStatuses.Served && !order.PostedToRoom)
            {
                order.Booking.RestaurantBill += order.TotalAmount;
                order.PostedToRoom = true;
                _billing.Recalculate(order.Booking);
            }
            else if (newStatus == OrderStatuses.Cancelled && order.PostedToRoom)
            {
                order.Booking.RestaurantBill = Math.Max(
                    0,
                    order.Booking.RestaurantBill - order.TotalAmount);

                order.PostedToRoom = false;
                _billing.Recalculate(order.Booking);
            }
        }

        order.Status = newStatus;
    }
}
