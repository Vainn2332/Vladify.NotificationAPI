FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Vladify.NotificationAPI/Vladify.NotificationAPI.csproj Vladify.NotificationAPI/
COPY Vladify.BusinessLogic/Vladify.BusinessLogic.csproj Vladify.BusinessLogic/
COPY Vladify.DataAccess/Vladify.DataAccess.csproj Vladify.DataAccess/
RUN dotnet restore Vladify.NotificationAPI/Vladify.NotificationAPI.csproj

COPY Vladify.NotificationAPI/ Vladify.NotificationAPI/
COPY Vladify.BusinessLogic/ Vladify.BusinessLogic/
COPY Vladify.DataAccess/ Vladify.DataAccess/
RUN dotnet publish Vladify.NotificationAPI/Vladify.NotificationAPI.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Vladify.NotificationAPI.dll"]
