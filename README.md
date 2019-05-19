# Barker

Barker is a twitter clone built using ASP.NET Core 2.1.

Technologies used:

 * ASP.NET core MVC
 * ASP.NET identity
 * Entity Framework core
 * PostgreSQL
 * Bootstrap 
 
To run barker, you'll need to have [.NET Core 2.1 or above](https://dotnet.microsoft.com/download) and preferably Docker.

 To run,
 
 1. clone this repository
 2. run `dotnet restore`
 3. run the command
 
  > docker run -p 5432:5432 --name barkerDev -e POSTGRES_DB=BarkerDb -e POSTGRES_USER=barker -e POSTGRES_PASSWORD=barker -d postgres
  
  4. run `dotnet build`
  5. run `export ASPNETCORE_ENVIRONMENT=Development`
  6. run `dotnet run`

## images

![Login/Register page](https://i.imgur.com/D8H4ycR.jpg)

![Homepage](https://i.imgur.com/iYlTvdM.png)

![Search page](https://i.imgur.com/Hc8DG4S.png)

![Settings page](https://i.imgur.com/DmIAfQW.png)
