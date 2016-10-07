#!/bin/sh
crowdin-cli download translations

echo "Downloading latest build..."

wget https://crowdin.com/download/project/activizr.zip

echo "Unpacking..."

unzip activizr.zip

echo "Deploying..."

cp -r Logic/* ../../Logic/Resources/
cp -r Site/* ../../Site/App_GlobalResources/
rm -rf Logic
rm -rf Site
rm activizr.zip
