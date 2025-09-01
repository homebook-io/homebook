#!/bin/bash
set -e

# Start nginx in background
nginx -g 'daemon off;' &

# Start HomBook.Backend
exec dotnet /opt/homebook/HomeBook.Backend.dll
