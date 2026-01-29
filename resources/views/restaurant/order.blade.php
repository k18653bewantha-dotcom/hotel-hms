<x-app-layout>
    <x-slot name="header">
        <h2 class="font-semibold text-xl text-gray-800">
            Create Restaurant Order
        </h2>
    </x-slot>

    <div class="py-6">
        <div class="max-w-6xl mx-auto sm:px-6 lg:px-8">
            <div class="bg-white p-6 rounded shadow">

                <form method="POST" action="{{ route('restaurant.order.store') }}">
                    @csrf

                    <!-- Table Number -->
                    <div class="mb-4">
                        <label class="block text-sm font-medium">Table No</label>
                        <input type="text" name="table_no"
                               class="w-full border rounded p-2"
                               placeholder="Table 01">
                    </div>

                    <!-- Menu Items -->
                    <table class="w-full border">
                        <thead class="bg-gray-100">
                            <tr>
                                <th class="border p-2 text-left">Item</th>
                                <th class="border p-2 w-24">Price</th>
                                <th class="border p-2 w-24">Qty</th>
                            </tr>
                        </thead>
                        <tbody>
                            @foreach($items as $item)
                                <tr>
                                    <td class="border p-2">
                                        {{ $item->name }}
                                    </td>
                                    <td class="border p-2">
                                        {{ number_format($item->price, 2) }}
                                        <input type="hidden"
                                               name="items[{{ $item->id }}][price]"
                                               value="{{ $item->price }}">
                                    </td>
                                    <td class="border p-2">
                                        <input type="number"
                                               min="0"
                                               name="items[{{ $item->id }}][qty]"
                                               class="w-full border rounded p-1"
                                               value="0">
                                    </td>
                                </tr>
                            @endforeach
                        </tbody>
                    </table>

                    <div class="mt-4">
                        <button class="bg-blue-600 text-white px-4 py-2 rounded">
                            Place Order
                        </button>
                    </div>

                </form>

            </div>
        </div>
    </div>
</x-app-layout>
