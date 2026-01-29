
<x-app-layout>
    <x-slot name="header">
        <h2 class="font-semibold text-xl text-gray-800">
            Add Room
        </h2>
    </x-slot>

    <div class="py-6">
        <div class="max-w-xl mx-auto bg-white p-6 rounded shadow">

            <form method="POST" action="{{ route('rooms.store') }}">
                @csrf

                <div class="mb-4">
                    <label class="block">Room Number</label>
                    <input type="text" name="room_number" class="w-full border rounded p-2" required>
                </div>

                <div class="mb-4">
                    <label class="block">Room Type</label>
                    <input type="text" name="room_type" class="w-full border rounded p-2" required>
                </div>

                <div class="mb-4">
                    <label class="block">Price (LKR)</label>
                    <input type="number" name="price" class="w-full border rounded p-2" required>
                </div>

                <button class="bg-blue-600 text-white px-4 py-2 rounded">
                    Save Room
                </button>

            </form>

        </div>
    </div>
</x-app-layout>
