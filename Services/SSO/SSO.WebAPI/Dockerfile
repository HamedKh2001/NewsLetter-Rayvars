# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src

COPY ["src/Services/SSO/BRTechGroup.SSO.WebAPI/BRTechGroup.SSO.WebAPI.csproj", "src/Services/SSO/BRTechGroup.SSO.WebAPI/"]
COPY ["src/Services/SSO/BRTechGroup.SSO.Application/BRTechGroup.SSO.Application.csproj", "src/Services/SSO/BRTechGroup.SSO.Application/"]
COPY ["src/BuildingBlocks/EventBus.Messages/EventBus.Messages.csproj", "src/BuildingBlocks/EventBus.Messages/"]
COPY ["src/BuildingBlocks/SharedKernel/SharedKernel.csproj", "src/BuildingBlocks/SharedKernel/"]
COPY ["src/Services/SSO/BRTechGroup.SSO.Domain/BRTechGroup.SSO.Domain.csproj", "src/Services/SSO/BRTechGroup.SSO.Domain/"]
COPY ["src/Services/SSO/BRTechGroup.SSO.Infrastructure/BRTechGroup.SSO.Infrastructure.csproj", "src/Services/SSO/BRTechGroup.SSO.Infrastructure/"]

RUN dotnet restore "src/Services/SSO/BRTechGroup.SSO.WebAPI/BRTechGroup.SSO.WebAPI.csproj" --runtime alpine-x64
COPY . .
WORKDIR "/src/src/Services/SSO/BRTechGroup.SSO.WebAPI"
RUN dotnet publish "BRTechGroup.SSO.WebAPI.csproj" -c Release -o /app/publish \
  --no-restore \
  --runtime alpine-x64 \
  --self-contained true \
  /p:PublishTrimmed=true \
  /p:PublishSingleFile=true

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-alpine AS final
RUN apk upgrade musl
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Install libicu package
RUN apk update && apk add --no-cache icu-libs

COPY --from=build /app/publish .
ENTRYPOINT ["./BRTechGroup.SSO.WebAPI"]
