<?php

use Illuminate\Support\Facades\Route;
use App\Http\Controllers\ProfileController;
use App\Http\Controllers\RoomController;
use App\Http\Controllers\BookingController;
use App\Http\Controllers\MenuController;
use App\Http\Controllers\RestaurantOrderController;



/*
|--------------------------------------------------------------------------
| Web Routes
|--------------------------------------------------------------------------
*/

Route::get('/', function () {
    return view('welcome');
});

/* Dashboard */
Route::get('/dashboard', function () {
    return view('dashboard');
})->middleware(['auth', 'verified'])->name('dashboard');

/* Profile */
Route::middleware('auth')->group(function () {
    Route::get('/profile', [ProfileController::class, 'edit'])->name('profile.edit');
    Route::patch('/profile', [ProfileController::class, 'update'])->name('profile.update');
    Route::delete('/profile', [ProfileController::class, 'destroy'])->name('profile.destroy');
});

/* Rooms */
Route::middleware('auth')->group(function () {
    Route::get('/rooms', [RoomController::class, 'index'])->name('rooms.index');
    Route::get('/rooms/create', [RoomController::class, 'create'])->name('rooms.create');
    Route::post('/rooms', [RoomController::class, 'store'])->name('rooms.store');
    Route::get('/rooms/{room}/edit', [RoomController::class, 'edit'])->name('rooms.edit');
    Route::put('/rooms/{room}', [RoomController::class, 'update'])->name('rooms.update');
    Route::delete('/rooms/{room}', [RoomController::class, 'destroy'])->name('rooms.destroy');
});

/* Check-in / Check-out */
Route::middleware('auth')->group(function () {
    Route::get('/check-in', [BookingController::class, 'create']);
    Route::post('/check-in', [BookingController::class, 'store'])->name('bookings.store');

    Route::get('/check-out/{booking}', [BookingController::class, 'checkoutForm'])
        ->name('bookings.checkout');

    Route::post('/check-out/{booking}', [BookingController::class, 'checkout'])
        ->name('bookings.checkout.process');
});

/* Restaurant Menu */
Route::middleware('auth')->group(function () {
    Route::get('/menu', [MenuController::class, 'index'])->name('menu.index');
    Route::post('/menu/category', [MenuController::class, 'storeCategory']);
    Route::post('/menu/item', [MenuController::class, 'storeItem']);
});

/* KOT */
Route::middleware('auth')->group(function () {
    Route::get('/restaurant/kot/{order}', [RestaurantOrderController::class, 'kot'])
        ->name('restaurant.kot');
});

Route::get('/restaurant/order', [RestaurantOrderController::class, 'create'])
    ->name('restaurant.order');

Route::post('/restaurant/order', [RestaurantOrderController::class, 'store'])
    ->name('restaurant.order.store');

Route::get('/restaurant/kot/{order}', [RestaurantOrderController::class, 'kot'])
    ->name('restaurant.kot');


    use App\Http\Controllers\RestaurantBillController;

Route::get('/restaurant/bill/{order}', [RestaurantBillController::class, 'show'])
    ->name('restaurant.bill');

Route::post('/restaurant/bill/{order}/pay', [RestaurantBillController::class, 'pay'])
    ->name('restaurant.bill.pay');

    


require __DIR__.'/auth.php';
