# Deployment notes

## Hostinger shared hosting

Normal Hostinger shared web hosting does not provide the .NET runtime, so this ASP.NET Core project cannot run there directly. Use a VPS, Azure App Service, Google Cloud VM/Cloud Run, Render-compatible container hosting, or run on a hotel PC over the LAN.

## Linux VPS summary

1. Install ASP.NET Core Runtime 10.
2. Publish on Windows:

```powershell
dotnet publish -c Release -o publish
```

3. Upload the `publish` directory.
4. Run:

```bash
dotnet HotelBookingSystem.dll --urls http://127.0.0.1:5050
```

5. Configure systemd and Nginx.
6. Back up `hotel.db` regularly.
