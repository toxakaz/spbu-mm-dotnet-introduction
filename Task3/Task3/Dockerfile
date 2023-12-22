FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

ENV Latitude=59.87519
ENV Longitude=29.825433
ENV StormglassApiKey=8ffc2534-a0e3-11ee-950b-0242ac130002-8ffc25a2-a0e3-11ee-950b-0242ac130002
ENV TomorrowApiKey=NEm4jY12nsgwxdBPIB0giRPYw6QoNCso
ENV OpenWeatherMapApiKey=e9c987890ed0c0633ae357f938a6c247

USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Task3/Task3.csproj", "Task3/"]
RUN dotnet restore "Task3/Task3.csproj"
COPY . .
WORKDIR "/src/Task3"
RUN dotnet build "Task3.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Task3.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Task3.dll"]
