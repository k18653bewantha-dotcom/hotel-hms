
<x-app-layout>
    <x-slot name="header">
        <h2 class="font-semibold text-xl text-gray-800">
            Edit Room
        </h2>
    </x-slot>

    <div class="py-6">
        <div class="max-w-xl mx-auto bg-white p-6 rounded shadow">

            <form method="POST" action="{{ route('rooms.update', $room->id) }}">
                @csrf
                @method('PUT')

                <div class="mb-4">
                    <label>Room Number</label>
                    <input type="text" name="room_number"
                           value="{{ $room->room_number }}"
                           class="w-full border p-2 rounded">
                </div>

                <div class="mb-4">
                    <label>Room Type</label>
                    <input type="text" name="room_type"
                           value="{{ $room->room_type }}"
                           class="w-full border p-2 rounded">
                </div>

                <div class="mb-4">
                    <label>Price</label>
                    <input type="number" name="price"
                           value="{{ $room->price }}"
                           class="w-full border p-2 rounded">
                </div>

                <div class="mb-4">
                    <label>Status</label>
                    <select name="status" class="w-full border p-2 rounded">
                        <option value="available" {{ $room->status=='available'?'selected':'' }}>Available</option>
                        <option value="occupied" {{ $room->status=='occupied'?'selected':'' }}>Occupied</option>
                        <option value="maintenance" {{ $room->status=='maintenance'?'selected':'' }}>Maintenance</option>
                    </select>
                </div>

                <button class="bg-blue-600 text-white px-4 py-2 rounded">
                    Update Room
                </button>

            </form>
        </div>
    </div>
</x-app-layout>
