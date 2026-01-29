<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    /**
     * Run the migrations.
     */
    public function up(): void
{
    Schema::create('restaurant_orders', function (Blueprint $table) {
        $table->id();
        $table->string('order_no')->unique();
        $table->string('table_no')->nullable();
        $table->enum('status', ['pending', 'preparing', 'served', 'cancelled'])
              ->default('pending');
        $table->timestamps();
        $table->decimal('total', 10, 2)->default(0);
$table->decimal('service_charge', 10, 2)->default(0);
$table->decimal('discount', 10, 2)->default(0);
$table->decimal('grand_total', 10, 2)->default(0);
$table->string('payment_method')->nullable(); // cash, card
$table->boolean('is_paid')->default(false);

    });
}


    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('restaurant_orders');
    }
};
