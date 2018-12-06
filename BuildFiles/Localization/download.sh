#!/bin/sh
crowdin download translations

echo "Downloading latest build..."

wget https://crowdin.com/backend/download/project/activizr.zip

echo "Unpacking..."

unzip activizr.zip

echo "Deploying..."

cp -r Logic/* ../../Logic/Resources/
cp -r Site/* ../../Site/App_GlobalResources/
rm -rf Logic
rm -rf Site
rm activizr.zip
