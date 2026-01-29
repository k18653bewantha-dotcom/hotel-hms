<!DOCTYPE html>
<html>
<head>
    <title>Final Bill</title>

    <style>
        body { font-family: Arial; text-align: center; }
        .bill-box { width: 350px; margin: auto; }
        table { width: 100%; border-collapse: collapse; }
        td, th { padding: 5px; border-bottom: 1px dashed #000; }
        @media print { button, form { display: none; } }
    </style>
</head>

<body>
<div class="bill-box">
    <h2>KINGSWOOD TEA FACTOTY </h2>
    <h2>RESTAURANT BILL</h2>
    <p>
        Order: {{ $order->order_no }}<br>
        Table: {{ $order->table_no }}
    </p>

    <table>
        @foreach($order->items as $item)
            <tr>
                <td>{{ $item->menuItem->name }} x {{ $item->qty }}</td>
                <td style="text-align:right;">
                    {{ number_format($item->qty * $item->price, 2) }}
                </td>
            </tr>
        @endforeach
    </table>

    <hr>

    <p>Total: {{ number_format($total, 2) }}</p>

    @if(!$order->is_paid)
        <form method="POST" action="{{ route('restaurant.bill.pay', $order->id) }}">
            @csrf

            <input type="number" name="service_charge" placeholder="Service Charge" step="0.01"><br><br>
            <input type="number" name="discount" placeholder="Discount" step="0.01"><br><br>

            <select name="payment_method" required>
                <option value="Cash">Cash</option>
                <option value="Card">Card</option>
            </select><br><br>

            <button type="submit">Pay & Close Bill</button>
        </form>
    @else
        <h3>PAID</h3>
        <p>Grand Total: {{ number_format($order->grand_total, 2) }}</p>
    @endif

    <button onclick="window.print()">Print Bill</button>

</div>
</body>
</html>
