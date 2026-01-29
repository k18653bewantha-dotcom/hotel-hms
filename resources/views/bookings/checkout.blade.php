<x-app-layout>
    <x-slot name="header">
        <h2 class="font-semibold text-xl text-gray-800">
            Room Check-Out
        </h2>
    </x-slot>

    <div class="py-6">
        <div class="max-w-xl mx-auto bg-white p-6 rounded shadow">

            <p class="mb-4">
                <strong>Guest:</strong> {{ $booking->guest_name }} <br>
                <strong>Room:</strong> {{ $booking->room->room_number }} <br>
                <strong>Check-In:</strong> {{ $booking->check_in }}
            </p>

            <form method="POST" action="{{ route('bookings.checkout.process', $booking->id) }}">
                @csrf

                <button class="bg-red-600 text-white px-4 py-2 rounded">
                    Confirm Check-Out
                </button>
            </form>

        </div>
    </div>
</x-app-layout>
