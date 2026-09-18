# Test checklist

After `dotnet restore`, `dotnet build` and `dotnet run`, test in this order.

## Login and permissions

1. First launch redirects to `/Account/Setup`.
2. Create the first administrator; verify no default password is requested.
3. Open **User Accounts** and create:
   - one Reception account;
   - one Restaurant account;
   - one Housekeeping account.
4. Sign in to each account and verify:
   - Reception cannot open Restaurant, Housekeeping or User Accounts;
   - Restaurant can open only Restaurant pages;
   - Housekeeping can open only Housekeeping;
   - Admin can open all pages.
5. Verify a new account is forced to change its temporary password.
6. Deactivate a test account and verify it cannot sign in again.

## Hotel booking

1. Rooms page shows the 16 seeded rooms (401–416).
2. Create a customer.
3. Create a booking:
   - select dates;
   - verify only rooms free for the period appear;
   - select two or more rooms;
   - enter the total room charge;
   - save.
4. Try an overlapping booking; occupied dates must not appear available.
5. Check in; every selected room becomes `Occupied`.

## Restaurant bill to room bill

1. Sign in with the Restaurant account.
2. POS: select **Charge to Room** and select the checked-in booking.
3. Add items and send the order.
4. Kitchen Live: move Pending → Preparing → Ready → Served.
5. Verify the success message states the order was added to the booking.
6. Sign in as Reception and open Bookings.
7. Verify the checkout restaurant amount equals the served room-order total and is read-only.
8. Create another pending room order and try checkout; checkout must be blocked.
9. Complete or cancel the pending order, then checkout.
10. Verify the invoice shows restaurant order details and the grand total includes them.
11. Verify all rooms become `Ready` after checkout.

## Other modules

1. Print the standard invoice.
2. Open Letterhead Invoice and adjust millimetre positions if necessary.
3. Open Calendar and confirm each room has its own event.
4. Open Reports and verify hotel and restaurant totals.

## Build verification

```powershell
dotnet restore
dotnet build
dotnet run
```
