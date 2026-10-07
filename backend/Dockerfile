FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY nuget.config .
COPY src/FloriculturaGestao.Domain/FloriculturaGestao.Domain.csproj src/FloriculturaGestao.Domain/
COPY src/FloriculturaGestao.Application/FloriculturaGestao.Application.csproj src/FloriculturaGestao.Application/
COPY src/FloriculturaGestao.Infrastructure/FloriculturaGestao.Infrastructure.csproj src/FloriculturaGestao.Infrastructure/
COPY src/FloriculturaGestao.API/FloriculturaGestao.API.csproj src/FloriculturaGestao.API/
RUN dotnet restore src/FloriculturaGestao.API/FloriculturaGestao.API.csproj
COPY src/ src/
WORKDIR /src/src/FloriculturaGestao.API
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FloriculturaGestao.API.dll"]
