<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>Rooms | Hotel Management System</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    {{-- Tailwind CSS --}}
    <script src="https://cdn.tailwindcss.com"></script>
</head>
<body class="bg-gray-100">

<div class="max-w-6xl mx-auto mt-10 bg-white p-6 rounded shadow">

    {{-- Page Header --}}
    <div class="flex justify-between items-center mb-6">
        <h1 class="text-2xl font-bold text-black">Room List</h1>

        <a href="{{ route('rooms.create') }}"
           class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded">
            Add Room
        </a>
    </div>

    {{-- Success Message --}}
    @if(session('success'))
        <div class="bg-green-100 text-green-800 p-3 rounded mb-4">
            {{ session('success') }}
        </div>
    @endif

    {{-- Rooms Table --}}
    <div class="overflow-x-auto">
        <table class="w-full border-collapse border border-gray-300">
            <thead>
            <tr class="bg-gray-200 text-black">
                <th class="border p-2">Room No</th>
                <th class="border p-2">Type</th>
                <th class="border p-2">Price</th>
                <th class="border p-2">Status</th>
                <th class="border p-2">Actions</th>
            </tr>
            </thead>

            <tbody>
            @forelse($rooms as $room)
                <tr class="text-center">
                    <td class="border p-2">{{ $room->room_number }}</td>
                    <td class="border p-2">{{ $room->room_type }}</td>
                    <td class="border p-2">LKR {{ number_format($room->price, 2) }}</td>
                    <td class="border p-2 capitalize">
                        {{ $room->status }}
                    </td>

                    <td class="border p-2">
                        <a href="{{ route('rooms.edit', $room->id) }}"
                           class="text-blue-600 hover:underline font-medium">
                            Edit
                        </a>

                        <form action="{{ route('rooms.destroy', $room->id) }}"
                              method="POST"
                              class="inline">
                            @csrf
                            @method('DELETE')

                            <button type="submit"
                                    onclick="return confirm('Are you sure you want to delete this room?')"
                                    class="text-red-600 hover:underline font-medium ml-2">
                                Delete
                            </button>
                        </form>
                    </td>
                </tr>
            @empty
                <tr>
                    <td colspan="5" class="text-center p-4 text-gray-500">
                        No rooms found.
                    </td>
                </tr>
            @endforelse
            </tbody>
        </table>
    </div>

</div>

</body>
</html>
