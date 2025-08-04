#!/bin/bash

set -e

PROJECT=time-track-service
if [ "$1" == "azure" ]; then 
   echo "setup for AZURE Run"
   IMAGE_VERSION=$2
   TARGET_DIR=$3
else 
   echo "setup for local Run"
   IMAGE_VERSION=0.0.1
   TARGET_DIR=out
   mkdir $TARGET_DIR
fi

docker build --build-arg PAT=$4 -f ./$PROJECT/Dockerfile --target build -t test-image:$IMAGE_VERSION .

docker create -ti --name testcontainer test-image:$IMAGE_VERSION

echo "copying test files" 
mkdir $TARGET_DIR/testresults 

docker cp testcontainer:/src/$PROJECT.Tests/TestResults/TestResults.xml $TARGET_DIR/testresults
docker cp testcontainer:/src/$PROJECT.Tests/coverage.opencover.xml $TARGET_DIR/testresults 
docker cp testcontainer:/src/$PROJECT.Tests/coverage.cobertura.xml $TARGET_DIR/testresults 

docker rm -fv testcontainer
