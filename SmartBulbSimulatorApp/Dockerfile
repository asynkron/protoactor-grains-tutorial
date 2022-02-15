# Stage 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder

WORKDIR /app/src

# Restore
COPY /ProtoClusterTutorial/*.csproj ./ProtoClusterTutorial/
COPY /SmartBulbSimulatorApp/*.csproj ./SmartBulbSimulatorApp/


RUN dotnet restore ./SmartBulbSimulatorApp/SmartBulbSimulatorApp.csproj -r linux-x64

# Build
COPY . .

RUN dotnet publish ./SmartBulbSimulatorApp/SmartBulbSimulatorApp.csproj -c Release -o /app/publish -r linux-x64 --no-self-contained --no-restore

# Stage 2 - Publish
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

RUN addgroup --system --gid 101 app \
    && adduser --system --ingroup app --uid 101 app


COPY --from=builder --chown=app:app /app/publish .

USER app
 
ENTRYPOINT ["./SmartBulbSimulatorApp"]