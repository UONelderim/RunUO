rm Nelderim.exe
mcs -out:../../Nelderim.exe -d:MONO -optimize+ -debug+ -unsafe -recurse:Source/Server/*.cs