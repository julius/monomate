#
# Author:
#   Julius Eckert
#

#
# Copyright (C) 2008 Julius Eckert (http://www.julius-eckert.com)
#
# Permission is hereby granted, free of charge, to any person obtaining
# a copy of this software and associated documentation files (the
# "Software"), to deal in the Software without restriction, including
# without limitation the rights to use, copy, modify, merge, publish,
# distribute, sublicense, and/or sell copies of the Software, and to
# permit persons to whom the Software is furnished to do so, subject to
# the following conditions:
# 
# The above copyright notice and this permission notice shall be
# included in all copies or substantial portions of the Software.
# 
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
# EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
# MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
# NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
# LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
# OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
# WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#


DIST_NAME='MonoMate'

function usage {
	echo "Usage: ./build.sh [option]"
	echo "[option]:"
	echo "	-compile		compile the project"
	echo "	-run			builds the 'require-mono-version' and runs it"
	echo "	-build			builds all deployment versions"
	echo ""
	exit
}


function compile {
	nant compile -buildfile:./tools/OSX.build -D:proj.name=$DIST_NAME
}

function buildmono {
	nant build-mono -buildfile:./tools/OSX.build -D:proj.name=$DIST_NAME
	if [ $? != 0 ]
	then
		exit
	fi
	
	# insert the monochecker
	cp -R ./tools/monochecker/* ./build/require-mono-version/$DIST_NAME.app/Contents/Resources/
	cp -R ./essentials/nomono/* ./build/require-mono-version/$DIST_NAME.app/Contents/Resources/
	
	# use specialized startup script
	cp ./tools/startup-mono.sh ./build/require-mono-version/$DIST_NAME.app/Contents/MacOS/$DIST_NAME
	chmod +x ./build/require-mono-version/$DIST_NAME.app/Contents/MacOS/$DIST_NAME
}

# works only if buildmono has been called before
function buildnomono {
	mkbundle2 -L ./tools/lib/*.dll -z -o ./build/obj/$DIST_NAME --deps ./build/obj/$DIST_NAME.exe  
	nant build-nomono -buildfile:./tools/OSX.build -D:proj.name=$DIST_NAME

	# clean up
	rm -R ./build/obj

	# use specialized startup script
	cp ./tools/startup-nomono.sh ./build/no-mono-version/$DIST_NAME.app/Contents/MacOS/$DIST_NAME
	chmod +x ./build/no-mono-version/$DIST_NAME.app/Contents/MacOS/$DIST_NAME
	
	# copy the dylibs and make them working
	cp ./tools/monolib/*.dylib ./build/no-mono-version/$DIST_NAME.app/Contents/Resources/
	install_name_tool -change /Library/Frameworks/Mono.framework/Versions/1.9/lib/libmono.0.0.0.dylib ./libmono.0.0.0.dylib ./build/no-mono-version/$DIST_NAME.app/Contents/Resources/$DIST_NAME
	install_name_tool -change /Library/Frameworks/Mono.framework/Versions/1.9/lib/libgthread-2.0.0.1400.1.dylib ./libgthread-2.0.0.1400.1.dylib ./build/no-mono-version/$DIST_NAME.app/Contents/Resources/$DIST_NAME
	install_name_tool -change /Library/Frameworks/Mono.framework/Versions/1.9/lib/libglib-2.0.0.1400.1.dylib ./libglib-2.0.0.1400.1.dylib ./build/no-mono-version/$DIST_NAME.app/Contents/Resources/$DIST_NAME
	install_name_tool -change /Library/Frameworks/Mono.framework/Versions/1.9/lib/libintl.8.0.1.dylib ./libintl.8.0.1.dylib ./build/no-mono-version/$DIST_NAME.app/Contents/Resources/$DIST_NAME
}

function run {
	./build/require-mono-version/$DIST_NAME.app/Contents/MacOS/$DIST_NAME
}

if [ ! $1 ] 
then
	usage
fi

if [ $1 = "-compile" ]
then
	compile
	exit
fi

if [ $1 = "-build" ]
then
	buildmono
	buildnomono
	exit
fi

if [ $1 = "-run" ]
then
	buildmono
	run
	exit
fi


usage
