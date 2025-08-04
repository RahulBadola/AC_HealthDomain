#!/bin/bash

if [ "$LOCAL" != "1" ]; then 
    until curl --head localhost:15000 
    do 
        echo "Waiting for Sidecar"
        sleep 3
    done
    echo "Sidecar available"
fi
dotnet /app/time-track-dshp.dll