<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\RestaurantOrder;
use App\Models\MenuItem;

class RestaurantOrderController extends Controller
{
    /**
     * Show order creation page
     */
    public function create()
    {
        $items = MenuItem::all();
        return view('restaurant.order', compact('items'));
    }

    /**
     * Store new restaurant order
     */
    public function store(Request $request)
    {
        $request->validate([
            'table_no' => 'required',
            'items' => 'required|array',
        ]);

        // Create order
        $order = RestaurantOrder::create([
            'order_no' => 'ORD-' . time(),
            'table_no' => $request->table_no,
        ]);

        // Save order items
        foreach ($request->items as $itemId => $data) {
            if (isset($data['qty']) && $data['qty'] > 0) {
                $order->items()->create([
                    'menu_item_id' => $itemId,
                    'qty'          => $data['qty'],
                    'price'        => $data['price'],
                ]);
            }
        }

        return redirect()->route('restaurant.kot', $order->id);
    }

    /**
     * Show KOT (Kitchen Order Ticket)
     */
    public function kot($id)
    {
        $order = RestaurantOrder::with(['items.menuItem'])->findOrFail($id);

        return view('restaurant.kot', compact('order'));
    }
}
