#!/bin/bash

exit_on_error() {
    exit_code=$1
    last_command=${@:2}
    if [ $exit_code -ne 0 ]; then
        >&2 echo "\"${last_command}\" command failed with exit code ${exit_code}."
        exit $exit_code
    fi
}

if [ $1 = 'PullRequest' ]; then 
     PR_BRANCH_NUMBER=$2
     PR_BRANCH_NAME=$(echo "$3" | cut -d '/' -f 3-)
else 
     PR_BRANCH_NUMBER=""
     PR_BRANCH_NAME=""
fi
echo "Setting PR_NUMBER=${PR_BRANCH_NUMBER} PR_BRANCH=${PR_BRANCH_NAME} for Build.Reason={$1}"
echo "##vso[task.setvariable variable=PR_NUMBER]$PR_BRANCH_NUMBER"
echo "##vso[task.setvariable variable=PR_BRANCH]$PR_BRANCH_NAME"    