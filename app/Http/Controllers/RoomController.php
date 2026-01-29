<?php

namespace App\Http\Controllers;

use App\Models\Room;
use Illuminate\Http\Request;

class RoomController extends Controller
{
    public function index()
    {
        $rooms = Room::all();
        return view('rooms.index', compact('rooms'));
    }

    public function create()
    {
        return view('rooms.create');
    }

    public function store(Request $request)
    {
        $request->validate([
            'room_number' => 'required|unique:rooms',
            'room_type' => 'required',
            'price' => 'required|numeric',
        ]);

        Room::create([
            'room_number' => $request->room_number,
            'room_type' => $request->room_type,
            'price' => $request->price,
            'status' => 'available',
        ]);

        return redirect()->route('rooms.index')->with('success', 'Room added successfully');
    }
    public function edit(Room $room)
{
    return view('rooms.edit', compact('room'));
}

public function update(Request $request, Room $room)
{
    $request->validate([
        'room_number' => 'required|unique:rooms,room_number,' . $room->id,
        'room_type' => 'required',
        'price' => 'required|numeric',
        'status' => 'required',
    ]);

    $room->update($request->all());

    return redirect()->route('rooms.index')->with('success', 'Room updated successfully');
}

public function destroy(Room $room)
{
    $room->delete();
    return redirect()->route('rooms.index')->with('success', 'Room deleted successfully');
}

}
