#!/bin/sh

cp ./build/Release/MonoChecker ./SimpleTODO.app/Contents/Resources/
cp -R ./MonoChecker.nib ./SimpleTODO.app/Contents/Resources/

./SimpleTODO.app/Contents/Resources/MonoChecker