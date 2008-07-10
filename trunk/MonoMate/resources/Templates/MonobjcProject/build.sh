DIST_NAME='{TEMPLATE.VAR:NAME}'

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

	# compile .xib-files
	./tools/XibCompiler ./build/require-mono-version/$DIST_NAME.app
}

# works only if compile has been called before
function buildnomono {
	nant build-nomono -buildfile:./tools/OSX.build -D:proj.name=$DIST_NAME

	# clean up
	rm -R ./build/obj

	# use specialized startup script
	cp ./tools/startup-nomono.sh ./build/no-mono-version/$DIST_NAME.app/Contents/MacOS/$DIST_NAME
	chmod +x ./build/no-mono-version/$DIST_NAME.app/Contents/MacOS/$DIST_NAME

	# compile .xib-files
	./tools/XibCompiler ./build/no-mono-version/$DIST_NAME.app
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
