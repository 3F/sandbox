@echo off

set gtools=..\..\..\.tools
set tools=..\tools

%gtools%\hmsbuild %tools%\.MulLowNoCorrShifts.compressor /p:core="%cd%\MulLowNoCorrShifts16.cs" /p:output="%cd%\MulLowNoCorrShifts16.algo" /nologo /v:m /m:4