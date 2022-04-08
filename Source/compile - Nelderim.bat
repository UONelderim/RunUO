cd Server
SET PATH=C:\Windows\Microsoft.NET\Framework\v4.0.30319
csc.exe /win32icon:nelderim.ico /r:Ultima.dll /debug:full /nowarn:0618 /nologo /out:..\Nelderim.exe /unsafe /recurse:*.cs
PAUSE
cd ..
title Nelderim - RunUO 2.0 SVN 1
Rem del Nelderim.pdb
echo off
