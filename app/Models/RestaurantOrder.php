<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class RestaurantOrder extends Model
{
    use HasFactory;

    protected $fillable = [
        'order_no',
        'table_no',
        'total',
        'service_charge',
        'discount',
        'grand_total',
        'payment_method',
        'is_paid',
    ];

    public function items()
    {
        return $this->hasMany(RestaurantOrderItem::class);
    }
}
