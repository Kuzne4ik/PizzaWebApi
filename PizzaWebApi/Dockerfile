#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 1400

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PizzaWebApi/PizzaWebApi.Web.csproj", "PizzaWebApi/"]
COPY ["PizzaWebApi.Infrastructure/PizzaWebApi.Infrastructure.csproj", "PizzaWebApi.Infrastructure/"]
COPY ["PizzaWebApi.Core/PizzaWebApi.Core.csproj", "PizzaWebApi.Core/"]
COPY ["PizzaWebApi.SharedKernel/PizzaWebApi.SharedKernel.csproj", "PizzaWebApi.SharedKernel/"]
RUN dotnet restore "PizzaWebApi/PizzaWebApi.Web.csproj"
COPY . .
WORKDIR "/src/PizzaWebApi"
RUN dotnet build "PizzaWebApi.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PizzaWebApi.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PizzaWebApi.Web.dll"]