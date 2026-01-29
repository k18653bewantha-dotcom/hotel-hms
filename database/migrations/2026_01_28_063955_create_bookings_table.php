<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    /**
     * Run the migrations.
     */
    public function up()
{
    Schema::create('bookings', function (Blueprint $table) {
        $table->id();
        $table->unsignedBigInteger('room_id');
        $table->string('guest_name');
        $table->string('guest_phone')->nullable();
        $table->date('check_in');
        $table->date('check_out')->nullable();
        $table->string('status')->default('checked_in'); // checked_in / checked_out
        $table->timestamps();

        $table->foreign('room_id')->references('id')->on('rooms')->onDelete('cascade');
    });
}


    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('bookings');
    }
};
