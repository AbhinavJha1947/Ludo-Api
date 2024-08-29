# Use the official .NET 8.0 SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the csproj file(s) and restore any dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application code
COPY . ./

# Build the application
RUN dotnet publish -c Release -o out

# Use the official .NET 8.0 runtime image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory inside the container
WORKDIR /app

# Copy the built application from the previous stage
COPY --from=build /app/out .

# Expose port 80
EXPOSE 80

# Define the entry point for the application
ENTRYPOINT ["dotnet", "LudoGameApi.dll"]
