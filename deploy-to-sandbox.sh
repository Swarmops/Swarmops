#!/bin/bash
#

cp -Rv Basic/obj/Release/* Site5/Bin/
cp -Rv Logic/obj/Release/* Site5/Bin/
cp -Rv Database/obj/Release/* Site5/Bin/
cp -Rv Interface/obj/Release/* Site5/Bin/

rm -rf /var/www/pirate.activizr.com/*
cp -Rv Site5/* /var/www/pirate.activizr.com/
chown -R www-data:www-data /var/www/pirate.activizr.com/*

