#!/bin/bash
set -e

# Ensure proper permissions for homebook directory
chmod 755 /var/lib/homebook
chown -R appuser:appgroup /var/lib/homebook

# Start nginx in background
nginx -g 'daemon off;' &

# Start HomBook.Backend
exec dotnet /opt/homebook/HomeBook.Backend.dll
