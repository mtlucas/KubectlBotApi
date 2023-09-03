#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "KubectlBotApi.csproj"
RUN dotnet build "KubectlBotApi.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR /src
RUN dotnet publish "KubectlBotApi.csproj" -c Release -o publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY kubectl .
COPY --from=publish /src/publish .
ENTRYPOINT ["dotnet", "KubectlBotApi.dll"]