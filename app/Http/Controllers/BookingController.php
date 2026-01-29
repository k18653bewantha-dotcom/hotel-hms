<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Room;
use App\Models\Booking;

class BookingController extends Controller
{
    public function create()
    {
        $rooms = Room::where('status', 'available')->get();
        return view('bookings.create', compact('rooms'));
    }

    public function store(Request $request)
    {
        Booking::create($request->all());

        // change room status to occupied
        Room::where('id', $request->room_id)
            ->update(['status' => 'occupied']);

        return redirect()->back()->with('success', 'Room booked successfully');
    }

    public function checkoutForm(Booking $booking)
{
    return view('bookings.checkout', compact('booking'));
}

public function checkout(Booking $booking)
{
    $booking->update([
        'check_out' => now(),
        'status' => 'checked_out'
    ]);

    // Make room available again
    $booking->room->update([
        'status' => 'available'
    ]);

    return redirect('/rooms')->with('success', 'Room checked out successfully');
}

}
