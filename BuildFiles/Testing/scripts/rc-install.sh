#!/bin/bash

ISDEBIAN=$(cat /etc/os-release | grep "ID=debian" | wc -l)
ISUBUNTU=$(cat /etc/os-release | grep "ID=ubuntu" | wc -l)

if [[ $ISDEBIAN -gt 0 ]]; then
  echo "Debian detected"
elif [[ $ISUBUNTU -gt 0 ]]; then
  echo "Ubuntu detected"
else
  echo "Operating system not detected, aborting"
  exit 1
fi


