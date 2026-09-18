# Kingswood Hotel PMS

ASP.NET Core Razor Pages hotel management system for Kingswood Tea Factory Hotel.

## Main features

- Secure, separate department logins
- Roles: Admin, Reception, Restaurant and Housekeeping
- Passwords stored as secure hashes, never as readable text
- First-run administrator setup with no built-in/default password
- Admin user-account management, role changes, activation/deactivation and password reset
- Dashboard with room, arrival, departure and revenue summaries
- Room management and room-status workflow
- Customer profiles
- One booking with multiple rooms
- Available-room filtering for selected check-in/check-out dates
- Duplicate and overlapping-booking prevention
- Group check-in and checkout
- Restaurant, bar, laundry, seasonal charges and discounts
- Automatic restaurant room-charge posting
- Standard printable invoice with restaurant order details
- Data-only invoice for pre-printed Kingswood letterhead
- FullCalendar booking calendar with separate room colours
- Housekeeping status page
- Restaurant POS and Kitchen Live
- Menu category, menu item and restaurant table management
- Hotel and restaurant reports
- SQLite database with sample rooms, customers, menu items and tables

## Requirements

- .NET 10 SDK
- Internet access on first run to restore NuGet packages and load Bootstrap/FullCalendar CDN assets

The project targets .NET 10 and uses EF Core SQLite 10.0.10.

## First run on Windows

Open Command Prompt or the VS Code terminal:

```powershell
cd C:\dotnet-projects\KingswoodHotelPMS
dotnet restore
dotnet run
```

Open:

```text
http://localhost:5050
```

The database `hotel.db` is created automatically.

### First login setup

The first time the system opens, it redirects to **First-Time Security Setup**.

1. Enter the administrator's name.
2. Choose an administrator username.
3. Create a password containing at least 8 characters, one letter and one number.
4. Click **Create Administrator**.

There is no default username or password stored in the source code.

## Create separate department logins

Sign in as Admin and open:

```text
User Accounts
```

Create a separate username and temporary password for each employee. The employee must change the temporary password after the first login.

### Role permissions

- **Admin**: complete system access and user-account management
- **Reception**: dashboard, rooms, customers, bookings, invoices, calendar and reports
- **Restaurant**: POS, Kitchen Live, menus, categories and tables
- **Housekeeping**: housekeeping room-status page

## Restaurant bill to room bill workflow

1. The guest must be checked in.
2. Open **Restaurant → POS**.
3. Select **Charge to Room**.
4. Select the in-house booking and send the order.
5. In **Kitchen Live**, move the order through Pending → Preparing → Ready → Served.
6. When the order is marked **Served**, its amount is automatically added to the booking's `RestaurantBill` and the booking grand total is recalculated.
7. The checkout screen shows the restaurant amount as read-only to prevent accidental deletion.
8. Checkout is blocked while the booking has pending restaurant room-charge orders.
9. The invoice lists the posted restaurant orders and their amounts.

Cancelled orders are not added to the room bill.

## Reset the database

Stop the project and delete:

```text
hotel.db
```

Run the project again. The database and sample data will be recreated. This permanently deletes existing hotel data and user accounts.

## Run on the same Wi-Fi

```powershell
dotnet run --urls "http://0.0.0.0:5050"
```

Find the server PC IPv4 address:

```powershell
ipconfig
```

Other devices can open:

```text
http://YOUR-PC-IP:5050
```

Allow TCP port 5050 through Windows Firewall.

## Publish

```powershell
dotnet publish -c Release -o publish
```

The published files are created in the `publish` folder.

## Important production note

`Database.EnsureCreated()` is used so the project starts easily. `DatabaseSchemaUpgrade` safely creates the new user-account table when an older `hotel.db` is reused. Before a large production deployment, move to EF Core migrations and make regular backups of `hotel.db`.

## Pre-printed letterhead alignment

Open a booking and click **Letterhead**. The system prints only invoice data. To adjust printer alignment, edit:

```text
Pages/Bookings/LetterheadInvoice.cshtml
```

Adjust the CSS `top`, `left`, and `right` values by 1–3 mm.

## v3 Admin Edit & Daily Meal Summary

This build includes Admin-only pre-checkout booking/invoice editing and a Reception-printable daily restaurant meal summary with BB/HB/FB, room, pax, breakfast, lunch and dinner totals. Existing SQLite databases are upgraded non-destructively at startup with MealPlan, Adults and Children booking fields.
