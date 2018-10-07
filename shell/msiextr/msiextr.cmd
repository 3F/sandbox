@echo off

set in=%1
set fpath=%in:~1,-1%
set odir=%fpath:~0,-4%_msi

echo extract into %odir% ...
msiexec /a "%fpath%" /qb TARGETDIR="%odir%"

REM # ~