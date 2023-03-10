# FRONTEND BUILDER CONTAINER
FROM node:14-alpine as frontend-builder
WORKDIR /app

# Install dependencies
COPY Frontend/package.json ./
RUN npm install

# Copy source code and build distributables
COPY ./Frontend ./
RUN npm run build


# BACKEND BUILDER CONTAINER
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS backend-builder
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY SchnappsAndLiquor.csproj ./
COPY SchnappsAndLiquor.sln ./
COPY wss/ ./wss
RUN dotnet restore SchnappsAndLiquor.csproj

# Copy everything else and build
COPY SchnappsAndLiquor/ ./SchnappsAndLiquor
RUN sed -i "s?../../../../Frontend/dist/saufen/?/app/saufen/?g" ./SchnappsAndLiquor/App.config && \
    dotnet publish ./SchnappsAndLiquor.sln -o output --runtime linux-x64

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY --from=backend-builder /app/output .
COPY --from=frontend-builder /app/dist/saufen ./saufen
EXPOSE 8080
ENTRYPOINT ["dotnet", "SchnappsAndLiquor.dll"]
