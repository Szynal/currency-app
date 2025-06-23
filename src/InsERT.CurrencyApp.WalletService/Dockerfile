# -------------------- Build stage --------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Directory.Packages.props ./
COPY InsERT.CurrencyApp.sln ./

COPY src/InsERT.CurrencyApp.WalletService/InsERT.CurrencyApp.WalletService.csproj ./src/InsERT.CurrencyApp.WalletService/

COPY ./src ./src
COPY ./tests ./tests

RUN dotnet restore InsERT.CurrencyApp.sln

WORKDIR /src/src/InsERT.CurrencyApp.WalletService
RUN dotnet publish -c Release -o /app/publish --no-restore

# -------------------- Runtime stage --------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

LABEL maintainer="Pawel Szynal <pawel@szynal.info>" \
      version="1.0.0" \
      description="WalletService microservice for CurrencyApp"

ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "InsERT.CurrencyApp.WalletService.dll"]
