#!/bin/bash

# Updates LOC count for Swarmops. Run from parent folder,
# like 'BuildFiles/update-loc.sh' for the output to write to right folder.

# Update the LOC count on every sprint deploy.

find . \( -name '*.cs' -o -name '*.as?x' \) | xargs wc -l > LINESOFCODE
