# Kingswood Hotel PMS v3 update

## Added

- Admin-only pre-checkout booking/invoice edit page (`/Bookings/Edit/{id}`).
- Server-side protection so Reception cannot alter invoice/billing values during checkout.
- Meal plan fields on bookings: RO, BB, HB, FB.
- Adult and child counts on bookings.
- Invoice now shows rooms, meal plan and guest counts.
- Daily Restaurant Meal Summary for Admin/Reception (`/Reports/DailyMeals`).
- A4 landscape print layout for the restaurant summary.
- Daily BB/HB/FB room and pax totals.
- Breakfast, lunch and dinner preparation totals.

## Meal calculation rules

- RO: no included meals.
- BB: breakfast.
- HB: breakfast + dinner.
- FB: breakfast + lunch + dinner.
- Breakfast is counted for guests who stayed the previous night, including checkout morning.
- Lunch and dinner are not counted after checkout.

## Existing SQLite databases

No database deletion is required. On startup, `DatabaseSchemaUpgrade` adds these missing columns to `Bookings` when needed:

- `MealPlan` (default `RO`)
- `Adults` (default `1`)
- `Children` (default `0`)

Existing bookings therefore remain intact and can be updated by Admin before checkout.

## Role behavior

- Admin: create/view bookings, edit booking/invoice values before checkout, print invoices, reports and meal summaries.
- Reception: create bookings, check in/out, view/print invoices and reports, print Daily Meal Summary. Reception cannot change stored invoice values during checkout.
- Restaurant: Restaurant POS/Kitchen access unchanged.
- Housekeeping: Housekeeping access unchanged.
