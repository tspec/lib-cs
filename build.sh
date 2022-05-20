#!/bin/bash

set -e

dotnet_args="-c release"

title() {
  echo
  echo "___"
  echo "___ $1 ________________________________________"
}

foot() {
  echo "-----------------------------------------------"
  echo
}

run_pack() {
  local name=$1

  title "Pack $name"

  pushd tspec-$name
  dotnet pack $dotnet_args -o ..
  popd

  foot
}

run_tests() {
  title "Tests"
  dotnet test $dotnet_args
  foot
}

clean_pkgs() {
  title "Clean packages"
  rm -vf *.nupkg
  rm -vf *.snupkg
  foot
}

clean_prj() {
  local name=$1

  title "Clean project $name"

  pushd tspec-$name
  rm -rf bin obj
  popd
}

clean() {
  clean_pkgs
  clean_prj core
  clean_prj report-json
  clean_prj test
  clean_prj example
}

if [ "$1" = "pack" ]
then
  clean
  run_tests
  run_pack core
  run_pack report-json
elif [ "$1" = "test" ]
then
  run_tests
elif [ "$1" = "clean" ]
then
  clean
else
  echo
  echo "Usage:"
  echo "  $0 <pack|test|clean>"
  echo
  exit 2
fi


