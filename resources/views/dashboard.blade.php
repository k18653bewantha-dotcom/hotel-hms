<x-app-layout>
    <x-slot name="header">
        <h2 class="font-semibold text-xl text-black">
            Hotel Management Dashboard
        </h2>
    </x-slot>

    <div class="py-6">
        <div class="max-w-7xl mx-auto sm:px-6 lg:px-8">

            <div class="grid grid-cols-1 md:grid-cols-3 gap-6">

                <!-- Rooms -->
                <a href="{{ route('rooms.index') }}"
                   class="bg-blue-200 hover:bg-blue-300 text-black p-6 rounded shadow text-center">
                    <h3 class="text-xl font-bold">Rooms</h3>
                    <p>Manage rooms</p>
                </a>

                <!-- Check In -->
                <a href="/check-in"
                   class="bg-green-200 hover:bg-green-300 text-black p-6 rounded shadow text-center">
                    <h3 class="text-xl font-bold">Check In</h3>
                    <p>Guest check-in</p>
                </a>

                <!-- Restaurant Menu -->
                <a href="/menu"
                   class="bg-purple-200 hover:bg-purple-300 text-black p-6 rounded shadow text-center">
                    <h3 class="text-xl font-bold">Restaurant Menu</h3>
                    <p>Food items</p>
                </a>

                <!-- Orders -->
                <a href="/restaurant/order"
                   class="bg-orange-200 hover:bg-orange-300 text-black p-6 rounded shadow text-center">
                    <h3 class="text-xl font-bold">Restaurant Orders</h3>
                    <p>Create orders</p>
                </a>

                <!-- KOT -->
                <a href="/restaurant/kot/1"
                   class="bg-red-200 hover:bg-red-300 text-black p-6 rounded shadow text-center">
                    <h3 class="text-xl font-bold">Kitchen (KOT)</h3>
                    <p>Print KOT</p>
                </a>

                <!-- Check Out -->
                <a href="/check-out/1"
                   class="bg-gray-200 hover:bg-gray-300 text-black p-6 rounded shadow text-center">
                    <h3 class="text-xl font-bold">Check Out</h3>
                    <p>Billing & payment</p>
                </a>

            </div>

        </div>
    </div>
</x-app-layout>
