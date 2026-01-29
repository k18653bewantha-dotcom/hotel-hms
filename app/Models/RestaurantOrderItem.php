<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class RestaurantOrderItem extends Model
{
    use HasFactory;

    protected $table = 'restaurant_order_items';

    protected $fillable = [
        'restaurant_order_id',
        'menu_item_id',
        'qty',
        'price',
    ];

    /**
     * Parent order
     */
    public function order()
    {
        return $this->belongsTo(RestaurantOrder::class, 'restaurant_order_id');
    }

    /**
     * Menu item (for KOT & billing)
     */
    public function menuItem()
    {
        return $this->belongsTo(MenuItem::class, 'menu_item_id');
    }
}
