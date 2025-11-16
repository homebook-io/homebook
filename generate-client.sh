#!/bin/bash
set -e

# Configuration variables
BACKEND_CSPROJ="./source/HomeBook.Backend/HomeBook.Backend.csproj"
CLIENT_CLASS="BackendClient"
CLIENT_NAMESPACE="HomeBook.Client"
OPENAPI_FILE="./source/HomeBook.Backend/HomeBook.Backend.json"
CLIENT_OUTPUT_DIR="./source/HomeBook.Client"
CLIENT_CSPROJ="HomeBook.Client.csproj"

# Clean output dir except the client csproj
find "${CLIENT_OUTPUT_DIR}" -mindepth 1 ! -name "${CLIENT_CSPROJ}" -exec rm -rf {} +

# build backend
dotnet restore "${BACKEND_CSPROJ}"
dotnet build "${BACKEND_CSPROJ}" --no-restore -c Release

# install/update kiota
dotnet tool install --global Microsoft.OpenApi.Kiota

# Generate client
kiota generate \
    --language csharp \
    --class-name "${CLIENT_CLASS}" \
    --namespace-name "${CLIENT_NAMESPACE}" \
    --openapi "${OPENAPI_FILE}" \
    --output "${CLIENT_OUTPUT_DIR}"

echo "Client generation completed successfully!"
echo "Output directory: ${CLIENT_OUTPUT_DIR}"
