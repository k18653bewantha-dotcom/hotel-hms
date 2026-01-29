<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Booking extends Model
{
    use HasFactory;

    protected $fillable = [
        'room_id',
        'guest_name',
        'guest_pasport',
        'guest_phone',
        'check_in',
        'check_out',
        'status'
    ];

    public function room()
    {
        return $this->belongsTo(Room::class);
    }
}
