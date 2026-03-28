# Stage 1: Build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copy the solution file from the ROOT of your repo
COPY Volynpack_Backend.sln ./

# 2. Copy project files (preserving the 'src/Project' directory structure)
COPY src/Api/*.csproj ./src/Api/
COPY src/Application/*.csproj ./src/Application/
COPY src/Infrastructure/*.csproj ./src/Infrastructure/
COPY src/Domain/*.csproj ./src/Domain/

# 3. Restore using the solution file now sitting in /src inside the container
RUN dotnet restore Volynpack_Backend.sln

# 4. Copy the entire source directory
COPY src/ ./src/

# 5. Publish the API (pointing to the correct path inside /src)
RUN dotnet publish src/Api/Api.csproj -c Release -o /app/publish

# ---

# Stage 2: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Api.dll"]