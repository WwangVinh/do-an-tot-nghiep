FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY OtoBackend.sln ./
COPY CoreEntities/CoreEntities.csproj CoreEntities/
COPY LogicBusiness/LogicBusiness.csproj LogicBusiness/
COPY SqlServer/SqlServer.csproj SqlServer/
COPY OtoBackend/OtoBackend.csproj OtoBackend/

RUN dotnet restore OtoBackend/OtoBackend.csproj

COPY . .
WORKDIR /src/OtoBackend
RUN dotnet publish OtoBackend.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "OtoBackend.dll"]
