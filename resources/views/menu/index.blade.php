<x-app-layout>
    <x-slot name="header">
        <h2 class="font-semibold text-xl text-gray-800">
            Restaurant Menu
        </h2>
    </x-slot>

    <div class="p-6">
        <div class="grid grid-cols-2 gap-6">

            <!-- Add Category -->
            <div class="bg-white p-4 rounded shadow">
                <h3 class="font-bold mb-2">Add Category</h3>
                <form method="POST" action="/menu/category">
                    @csrf
                    <input type="text" name="name" placeholder="Category name"
                           class="border p-2 w-full mb-2">
                    <button type="submit" class="bg-blue-600 text-white px-4 py-2 rounded">
    Save
</button>
                </form>
            </div>

            <!-- Add Item -->
            <div class="bg-white p-4 rounded shadow">
                <h3 class="font-bold mb-2">Add Menu Item</h3>
                <form method="POST" action="/menu/item">
                    @csrf
                    <select name="menu_category_id" class="border p-2 w-full mb-2">
                        @foreach($categories as $category)
                            <option value="{{ $category->id }}">{{ $category->name }}</option>
                        @endforeach
                    </select>

                    <input type="text" name="name" placeholder="Item name"
                           class="border p-2 w-full mb-2">

                    <input type="number" name="price" placeholder="Price"
                           class="border p-2 w-full mb-2">

                    <button type="submit" class="bg-blue-600 text-white px-4 py-2 rounded">
    Save
</button>
                </form>
            </div>
        </div>

        <!-- Menu List -->
        <div class="mt-6 bg-white p-4 rounded shadow">
            <h3 class="font-bold mb-2">Menu List</h3>

            @foreach($categories as $category)
                <h4 class="font-semibold mt-3">{{ $category->name }}</h4>
                <ul>
                    @foreach($category->items as $item)
                        <li>{{ $item->name }} - LKR {{ $item->price }}</li>
                    @endforeach
                </ul>
            @endforeach
        </div>
    </div>
</x-app-layout>
