FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Directory.Packages.props ./
COPY InsERT.CurrencyApp.sln ./

COPY src/ ./src/
COPY tests/ ./tests/

RUN dotnet restore InsERT.CurrencyApp.sln

COPY . .

WORKDIR /app/src/InsERT.CurrencyApp.TransactionService
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

COPY --from=build /publish ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "InsERT.CurrencyApp.TransactionService.dll"]
