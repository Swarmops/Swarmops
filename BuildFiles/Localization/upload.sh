#!/bin/bash

# Go down to Dev root

cd ../../..
if [ -d temp ]; then
  rm -rf temp/*
else
  mkdir temp
fi

# Create structure

mkdir temp/Site
mkdir temp/Logic

# Copy Site resources

echo "Copying resources to temporary folder..."

#cp Swarmops/Site/App_GlobalResources/*.resx temp/Site
cat Swarmops/Localization/SourceResourceFiles.txt | xargs -n 1 -I '{}' cp Swarmops/Localization/{} temp/Localization/
cat Swarmops/Logic/Resources/SourceResourceFiles.txt | xargs -n 1 -I '{}' cp Swarmops/Logic/Resources/{} temp/Logic/
cp Swarmops/BuildFiles/Localization/crowdin.yaml temp

echo "Uploading en-US source files..."

cd temp
crowdin upload sources --tree
cd ..
rm -rf temp

cd Swarmops/BuildFiles/Localization
