cd QuickRestart
SET DOTNET=C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727
SET PATH=%DOTNET%
csc.exe /win32icon:nelderim.ico /debug /nowarn:0618 /nologo /out:..\QuickRestart.exe /unsafe /recurse:*.cs
#PAUSE
cd ..
title Nelderim - QuickRestart dla RunUO 2.0 SVN 1
echo off
del QuickRestart.pdb
cls