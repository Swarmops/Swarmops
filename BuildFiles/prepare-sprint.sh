#!/bin/bash

#Before this, a manual git pull should be done

#Download localizations
cd Localization
./download.sh
cd ..

#Update lines-of-code count
./update-kaylock-count.sh

#After this, a manual git push should be done
