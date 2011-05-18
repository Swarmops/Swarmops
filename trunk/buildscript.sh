#!/bin/bash
#

if xbuild /logger:/opt/ccnet/Rodemeyer.MsBuildToCCnet.dll; then
	echo "Build Successful"
else
	echo "Build Failed; Exiting"
	exit 1
fi


