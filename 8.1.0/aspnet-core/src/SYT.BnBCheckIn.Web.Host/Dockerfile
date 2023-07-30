FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src
COPY ["src/SYT.BnBCheckIn.Web.Host/SYT.BnBCheckIn.Web.Host.csproj", "src/SYT.BnBCheckIn.Web.Host/"]
COPY ["src/SYT.BnBCheckIn.Web.Core/SYT.BnBCheckIn.Web.Core.csproj", "src/SYT.BnBCheckIn.Web.Core/"]
COPY ["src/SYT.BnBCheckIn.Application/SYT.BnBCheckIn.Application.csproj", "src/SYT.BnBCheckIn.Application/"]
COPY ["src/SYT.BnBCheckIn.Core/SYT.BnBCheckIn.Core.csproj", "src/SYT.BnBCheckIn.Core/"]
COPY ["src/SYT.BnBCheckIn.EntityFrameworkCore/SYT.BnBCheckIn.EntityFrameworkCore.csproj", "src/SYT.BnBCheckIn.EntityFrameworkCore/"]
WORKDIR "/src/src/SYT.BnBCheckIn.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/SYT.BnBCheckIn.Web.Host", "src/SYT.BnBCheckIn.Web.Host"]
COPY ["src/SYT.BnBCheckIn.Web.Core", "src/SYT.BnBCheckIn.Web.Core"]
COPY ["src/SYT.BnBCheckIn.Application", "src/SYT.BnBCheckIn.Application"]
COPY ["src/SYT.BnBCheckIn.Core", "src/SYT.BnBCheckIn.Core"]
COPY ["src/SYT.BnBCheckIn.EntityFrameworkCore", "src/SYT.BnBCheckIn.EntityFrameworkCore"]
WORKDIR "/src/src/SYT.BnBCheckIn.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "SYT.BnBCheckIn.Web.Host.dll"]
