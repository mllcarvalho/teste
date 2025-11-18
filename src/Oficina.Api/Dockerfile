
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Oficina.Api/Oficina.Api.csproj", "src/Oficina.Api/"]
COPY ["src/atendimento/Oficina.Atendimento.Application/Oficina.Atendimento.Application.csproj", "src/atendimento/Oficina.Atendimento.Application/"]
COPY ["src/atendimento/Oficina.Atendimento.Domain/Oficina.Atendimento.Domain.csproj", "src/atendimento/Oficina.Atendimento.Domain/"]
COPY ["src/atendimento/Oficina.Atendimento.Infrastructure/Oficina.Atendimento.Infrastructure.csproj", "src/atendimento/Oficina.Atendimento.Infrastructure/"]
COPY ["src/common/Oficina.Common.Application/Oficina.Common.Application.csproj", "src/common/Oficina.Common.Application/"]
COPY ["src/common/Oficina.Common.Domain/Oficina.Common.Domain.csproj", "src/common/Oficina.Common.Domain/"]
COPY ["src/common/Oficina.Common.Infrastructure/Oficina.Common.Infrastructure.csproj", "src/common/Oficina.Common.Infrastructure/"]
COPY ["src/estoque/Oficina.Estoque.Application/Oficina.Estoque.Application.csproj", "src/estoque/Oficina.Estoque.Application/"]
COPY ["src/estoque/Oficina.Estoque.Domain/Oficina.Estoque.Domain.csproj", "src/estoque/Oficina.Estoque.Domain/"]
COPY ["src/estoque/Oficina.Estoque.Infrastructure/Oficina.Estoque.Infrastructure.csproj", "src/estoque/Oficina.Estoque.Infrastructure/"]

RUN dotnet restore "src/Oficina.Api/Oficina.Api.csproj"

# Copiar todo o código fonte
COPY src/ src/

WORKDIR "/src/src/Oficina.Api"
RUN dotnet build "Oficina.Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Oficina.Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Oficina.Api.dll"]