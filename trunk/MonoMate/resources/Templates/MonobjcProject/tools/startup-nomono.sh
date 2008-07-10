#!/bin/sh

# This file has been modified to fit the needs of the MonoMate Build Process


# --------------------------------------------------------------------------------
# Monobjc : a .NET/Objective-C bridge
# Copyright (C) 2007, 2008  Laurent Etiemble
# 
# This library is free software; you can redistribute it and/or
# modify it under the terms of the GNU Lesser General Public
# License as published by the Free Software Foundation; either
# version 2.1 of the License, or any later version.
# 
# This library is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
# Lesser General Public License for more details.
# 
# You should have received a copy of the GNU Lesser General Public
# License along with this library; if not, write to the Free Software
# Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
# --------------------------------------------------------------------------------

# The purposes of this script are:
# - launching a Mono application stored within a bundle
# - naming correctly the launched application
# - be independant from the Mono installation folder
# --------------------------------------------------------------------------------

# Normalize the current path and command, to find the MacOS folder of the bundle
# --------------------------------------------------------------------------------
PWD=`pwd`
MACOS_PATH=`echo "$PWD/$0" | awk -F"/" '{ for(i = 1; i <= NF - 1; i++) { printf("%s/", $i); } }'`
cd "$MACOS_PATH"
MACOS_PATH=`pwd`

# Find the base folder of the bundle
# --------------------------------------------------------------------------------
BASE_PATH=`echo "$MACOS_PATH" | awk -F"/" '{ for(i = 1; i <= NF - 2; i++) { printf("%s/", $i); } }'`

# Find the resource folder of the bundle
# --------------------------------------------------------------------------------
RESOURCES_PATH=`echo "$MACOS_PATH" | awk -F"/" '{ for(i = 1; i <= NF - 1; i++) { printf("%s/", $i); } printf("Resources"); }'`

# Export some environment variable for Mono runtime
# --------------------------------------------------------------------------------
export MONO_GDIP_USE_COCOA_BACKEND=1
export DYLD_LIBRARY_PATH=$RESOURCES_PATH:$DYLD_LIBRARY_PATH

# Extract the application and the main assembly name
# --------------------------------------------------------------------------------
APP_NAME=`echo $0 | awk -F"/" '{ printf("%s", $NF); }'`
ASSEMBLY=`echo $0 | awk -F"/" '{ printf("%s.exe", $NF); }'`

# Go to the resources folder
# Remove a symbolic link named from the application if it exists 
# Create a symbolic link named from the application to the mono command
# Launch the application by using the symbolic link.
# --------------------------------------------------------------------------------
cd "$BASE_PATH"
EXEC_PATH="./$APP_NAME"
if [ -f "$EXEC_PATH" ]; then rm -f "$EXEC_PATH" ; fi
link `echo "$RESOURCES_PATH/$APP_NAME"` "$EXEC_PATH"

exec "$EXEC_PATH"
