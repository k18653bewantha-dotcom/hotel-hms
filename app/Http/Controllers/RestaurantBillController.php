<?php

namespace App\Http\Controllers;

use App\Models\RestaurantOrder;
use Illuminate\Http\Request;

class RestaurantBillController extends Controller
{
    public function show(RestaurantOrder $order)
    {
        $total = $order->items->sum(function ($item) {
            return $item->qty * $item->price;
        });

        return view('restaurant.bill', compact('order', 'total'));
    }

    public function pay(Request $request, RestaurantOrder $order)
    {
        $total = $order->items->sum(fn($i) => $i->qty * $i->price);

        $service = $request->service_charge ?? 0;
        $discount = $request->discount ?? 0;

        $grand = $total + $service - $discount;

        $order->update([
            'total' => $total,
            'service_charge' => $service,
            'discount' => $discount,
            'grand_total' => $grand,
            'payment_method' => $request->payment_method,
            'is_paid' => true,
        ]);

        return redirect()->route('restaurant.bill', $order->id);
    }
}
