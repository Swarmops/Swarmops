#!/bin/sh
echo "Downloading latest Crowdin build..."

# crowdin download translations

# echo "Downloading latest build..."

wget https://crowdin.com/backend/download/project/activizr.zip

echo "Unpacking..."

unzip activizr.zip

echo "Renaming Serbian (Latin, Cyrillic) to .Net standard..."

rename 's/\.sr\-SP\./\.sr\-Cyrl\-RS\./' Logic/*
rename 's/\.sr\-CS\./\.sr\-Latn\-RS\./' Logic/*

rename 's/\.sr\-SP\./\.sr\-Cyrl\-RS\./' Localization/*
rename 's/\.sr\-CS\./\.sr\-Latn\-RS\./' Localization/*

#echo "Deploying..."

cp -r Localization/* ../../Localization/
cp -r Site/* ../../Site/App_GlobalResources/
rm -rf Logic
rm -rf Localization
rm -rf Site
rm activizr.zip
