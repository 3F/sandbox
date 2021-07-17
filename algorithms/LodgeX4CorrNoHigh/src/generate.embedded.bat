@echo off

set gtools=..\..\..\.tools
set tools=..\tools

%gtools%\hmsbuild %tools%\.LodgeX4CorrNoHigh.compressor /p:core="%cd%\LodgeX4CorrNoHigh.cs" /p:output="%cd%\LodgeX4CorrNoHigh.algo.embd" /nologo /v:m /m:4