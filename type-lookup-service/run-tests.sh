#!/bin/bash

exit_on_error() {
    exit_code=$1
    last_command=${@:2}
    if [ $exit_code -ne 0 ]; then
        >&2 echo "\"${last_command}\" command failed with exit code ${exit_code}."
        exit $exit_code
    fi
}


PROJECT=type-lookup-service
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

docker build -f ./$PROJECT/Dockerfile --target build -t test-image:$IMAGE_VERSION . 

docker create -ti --name testcontainer test-image:$IMAGE_VERSION

echo "copying test files" 
mkdir $TARGET_DIR/testresults 

docker cp testcontainer:/src/$PROJECT.Tests/TestResults/TestResults.xml $TARGET_DIR/testresults

docker rm -fv testcontainer
