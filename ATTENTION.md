# გაშვება

1. `EQIMTANVIZITI/appsettings.json`-ში შეცვალე: `ConnectionStrings`, `Jwt:Key`, `Smtp` (Gmail-ისთვის App Password).
2. Visual Studio -> Tools -> NuGet Package Manager -> Package Manager Console (Default project: EQIMTANVIZITI):
   `Add-Migration InitialCreate`
   `Update-Database`
   (ან ტერმინალში EQIMTANVIZITI საქაღალდეში: `dotnet tool install --global dotnet-ef`, `dotnet ef migrations add InitialCreate`, `dotnet ef database update`)
3. გაუშვი პროექტი და ტესტები `EQIMTANVIZITI.http` ფაილიდან.

Flow: Register -> Verify -> Login (token) -> Reservation -> Prescriptions
Roles: Admin (seed: admin@eqimtanviziti.local / Admin12345), Doctor (ადმინი ქმნის), Patient (რეგისტრაცია).
