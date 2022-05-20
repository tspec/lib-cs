#!/bin/bash

set -e

dotnet_args="-c release"
RED='\033[0;31m'
GREEN='\033[0;32m'
RESET='\033[0m'
  
trap 'trap_errors $? $LINENO' EXIT

trap_errors() {
  local err=$1
  local line=$2
  if [ "$1" != "0" ]; then
    printf "\n${RED}ERROR ${err} on line ${line}${RESET}\n\n"
  else
    printf "\n${GREEN}SUCCESS${RESET}\n\n"
  fi
}

title() {
  echo
  echo "___"
  echo "___ $1 ________________________________________"
}

foot() {
  echo "-----------------------------------------------"
  echo
}

success() {
  local msg=$1
  echo
  echo "${GREEN}SUCCESS:I ${msg}${RESET}"
  echo
}

run_pack() {
  local name=$1

  title "Pack $name"

  pushd tspec-$name
  dotnet pack $dotnet_args -o ../dist
  popd

  foot
}

nuget_push() {
  pushd dist
  dotnet nuget push *.nupkg -k $(cat ~/.keys/nuget)
  popd
}

run_tests() {
  title "Tests"
  dotnet test $dotnet_args
  foot
}

run_examples() {
  title "Examples"
  pushd tspec-example
  dotnet run
  popd
  foot
}

clean_dist() {
  title "Clean packages"
  rm -rfv dist
  foot
}

clean_prj() {
  local name=$1

  title "Clean project $name"

  pushd tspec-$name
  rm -rfv bin obj
  popd
}

clean() {
  clean_dist
  clean_prj core
  clean_prj report-json
  clean_prj test
  clean_prj example
}

pack() {
  clean
  run_tests
  run_examples
  run_pack core
  run_pack report-json
}

cd $(dirname $0)

if [ "$1" = "pack" ]
then
  pack
elif [ "$1" = "test" ]
then
  run_tests
  run_examples
elif [ "$1" = "clean" ]
then
  clean
elif [ "$1" = "pack_and_nuget_push" ]
then
  pack
  nuget_push
else
  echo
  echo "Usage:"
  echo "  $0 <pack|test|clean|pack_and_nuget_push>"
  echo
  exit 2
fi


