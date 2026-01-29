<x-app-layout>
    <x-slot name="header">
        <h2 class="font-semibold text-xl text-gray-800">
            Room Check-In
        </h2>
    </x-slot>

    <div class="py-6">
        <div class="max-w-xl mx-auto bg-white p-6 rounded shadow">

            @if(session('success'))
                <div class="bg-green-100 text-green-800 p-3 mb-4 rounded">
                    {{ session('success') }}
                </div>
            @endif

            <form method="POST" action="{{ route('bookings.store') }}">
                @csrf

                <div class="mb-4">
                    <label class="block font-medium">Guest Name</label>
                    <input type="text" name="guest_name"
                           class="w-full border rounded p-2" required>
                </div>

                <div class="mb-4">
                    <label class="block font-medium">Passport / NIC Number</label>
                    <input type="text" name="passport_number"
                           class="w-full border rounded p-2">
                </div>

                <div class="mb-4">
                    <label class="block font-medium">Phone Number</label>
                    <input type="text" name="guest_phone"
                           class="w-full border rounded p-2">
                </div>

                <div class="mb-4">
                    <label class="block font-medium">Select Room</label>
                    <select name="room_id" class="w-full border rounded p-2" required>
                        <option value="">-- Select Room --</option>
                        @foreach($rooms as $room)
                            <option value="{{ $room->id }}">
                                Room {{ $room->room_number }} ({{ $room->room_type }})
                            </option>
                        @endforeach
                    </select>
                </div>

                <div class="mb-4">
                    <label class="block font-medium">Check-In Date</label>
                    <input type="date" name="check_in"
                           class="w-full border rounded p-2" required>
                </div>

                <button type="submit"
                        class="bg-blue-600 text-white px-4 py-2 rounded">
                    Check In
                </button>

            </form>
        </div>
    </div>
</x-app-layout>
