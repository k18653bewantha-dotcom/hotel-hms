<!DOCTYPE html>
<html>
<head>
    <title>KOT</title>

    <style>
        body {
            font-family: Arial, Helvetica, sans-serif;
            text-align: center;
        }

        h2, h3 {
            margin: 5px 0;
        }

        .kot-box {
            width: 300px;
            margin: auto;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 10px;
        }

        th, td {
            border-bottom: 1px dashed #000;
            padding: 5px;
            text-align: left;
            font-size: 14px;
        }

        th {
            text-align: left;
        }

        /* 🔥 HIDE BUTTON WHEN PRINTING */
        @media print {
            button {
                display: none;
            }
        }
    </style>
</head>

<body>

    <div class="kot-box">
        <h2>KITCHEN ORDER TICKET</h2>
        <h3>{{ $order->order_no }}</h3>

        <p>
            <strong>Table:</strong> {{ $order->table_no }} <br>
            <strong>Date:</strong> {{ $order->created_at->format('Y-m-d H:i') }}
        </p>

        <hr>

        <table>
            <thead>
                <tr>
                    <th>Item</th>
                    <th style="text-align:right;">Qty</th>
                </tr>
            </thead>
            <tbody>
                @foreach($order->items as $item)
                    <tr>
                        <td>{{ $item->menuItem->name }}</td>
                        <td style="text-align:right;">{{ $item->qty }}</td>
                    </tr>
                @endforeach
            </tbody>
        </table>

        <br>

        <!-- Print button (hidden when printing) -->
        <button onclick="window.print()">Print KOT</button>
    </div>

    <!-- 🔥 AUTO PRINT WHEN PAGE LOADS -->
    <script>
        window.onload = function () {
            window.print();
        }
    </script>

</body>
</html>
