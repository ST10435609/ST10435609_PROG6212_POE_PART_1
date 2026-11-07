# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copy only the csproj first so Docker can cache the restore layer
# and skip re-downloading packages on every rebuild
COPY RaceDay.Mvc/RaceDay.Mvc.csproj RaceDay.Mvc/
RUN dotnet restore RaceDay.Mvc/RaceDay.Mvc.csproj

COPY RaceDay.Mvc/ RaceDay.Mvc/
WORKDIR /src/RaceDay.Mvc
RUN dotnet publish -c Release -o /app/publish --no-restore

# ---- Runtime stage ----
# smaller runtime-only image, no SDK bloat in the final container
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "RaceDay.Mvc.dll"]
