#!/bin/bash

echo "-----------------------------------------"
echo "Swarmops Release Candidate script running"
echo "-----------------------------------------"
echo " "

ISDEBIAN=$(cat /etc/os-release | grep "ID=debian" | wc -l)
ISUBUNTU=$(cat /etc/os-release | grep "ID=ubuntu" | wc -l)

# detect operating system

if [[ $ISDEBIAN -gt 0 ]]; then
  echo "Debian detected"
elif [[ $ISUBUNTU -gt 0 ]]; then
  echo "Ubuntu detected"
else
  echo "Operating system not detected, aborting"
  exit 1
fi

# remove the autoexecuting commands, if there were
# any before

if [ -f /home/rick/.bash_profile ]; then
  rm /home/rick/.bash_profile
fi

# detect if running as root already; if not, add sudo to
# required commands

SUDO=''

if [ $EUID -ne 0 ]; then
    SUDO='sudo'
fi

echo "Step 1 - autoremoving, updating, and upgrading packages:"
echo " "

$SUDO apt -y autoremove
$SUDO apt -y update
$SUDO apt -y upgrade

if [ -f /var/run/reboot-required ]; then
  echo "./install-release-candidate.sh" > /home/rick/.bash_profile
  echo " "
  echo "After upgrading, a REBOOT is required to continue."
  echo "The script will continue when you ssh to the machine after reboot."
  echo " "
  read -p "Press ENTER to reboot..."
  $SUDO reboot now
fi

echo " "
echo "Step 2 - Installing MySQL or MariaDB server:"
echo " "

$SUDO apt -y install mysql-server

if [ $ISDEBIAN -gt 0 ]; then
  echo " "
  echo "Step 3 - Configuring MariaDB for root@localhost login"
  echo " "
  echo "GRANT ALL PRIVILEGES on *.* to 'root'@'localhost' IDENTIFIED BY 'sandbox';" > /tmp/mariadbroot.sql
  echo "FLUSH PRIVILEGES;" >> /tmp/mariadbroot.sql

  $SUDO mysql -u root -psandbox < /tmp/mariadbroot.sql
else
  echo " "
  echo "Step 3 - No configuration of MySQL required, skipped"
fi

echo " "
echo "Step 4 - Installing Swarmops Release Candidate:"
echo " "

$SUDO apt -y install swarmops-frontend-rc swarmops-backend-rc

