#!/bin/bash

#Before this, a manual git pull should be done

#Download localizations
cd Localization
./download.sh
cd ..

#Update lines-of-code count
cd ..
BuildFiles/update-kaylock-count.sh
cd BuildFiles

#After this, a manual git push should be done
