#!/bin/bash

OUTPUT_DIR="publish"

PROJECT_FILE=$(find . -maxdepth 1 -name "*.csproj" | head -n 1)

# FLAGS_x64="-c Release --self-contained -p:PublishSingleFile=true -p:PublishTrimmed=true"
FLAGS_x64="-c Release -p:PublishAot=true"

if [ -z "$PROJECT_FILE" ]; then
    echo "Error: .csproj file not found."
    exit 1
fi

dotnet publish "$PROJECT_FILE" -r linux-x64 $FLAGS_x64 -o "$OUTPUT_DIR/linux-x64"
