@echo off

set tools=..\tools

%tools%\hmsbuild %tools%\.LodgeX4CorrNoHigh.compressor /p:core="%cd%\LodgeX4CorrNoHigh.cs" /p:output="%cd%\LodgeX4CorrNoHigh.algo.embd" /nologo /v:m /m:4