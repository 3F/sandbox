@echo off
:: Copyright (c) 2018  Denis Kuzmin [ entry.reg@gmail.com ]
:: Specially for https://github.com/3F/Examples
:: Bash version / [[ Batch version ]]

setlocal enableDelayedExpansion

set ERR_SUCCESS=0
set ERR_FAIL=1

set _url=%1
set "_dst=%2"

if "%_url%"=="" (
    echo Allowed commands: %~nx0 {url} [{destination_path}]
    echo Sample: %~nx0 https://github.com/3F/Examples/tree/master/DllExport/BasicExport
    exit /B %ERR_SUCCESS%
)

if "%_dst%"=="" (
    for /f "tokens=4 delims=/" %%a in ("%_url%") do (
        set _dst=%%a
    )
)

for /f "tokens=1,2,3,4 delims=/" %%a in ("%_url%") do (
    set _repo=%%a//%%b/%%c/%%d
)

for /f "tokens=6* delims=/" %%a in ("%_url%") do (
    set _rpath=%%b
)

call :viaSvn || call :viaGit || call :failed
exit /B %ERRORLEVEL%

::  ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ 
::   ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ 

:viaSvn

echo trying via svn ...

:: set svnurl=!_url:/tree/%_branch%/=.git/trunk/!

set svnurl=%_repo%.git/trunk/%_rpath%

svn export %svnurl% %_dst%/%_rpath% || exit /B %ERR_FAIL%

exit /B %ERR_SUCCESS%


:viaGit

echo trying via git ...

git clone --depth=1 --no-checkout %_repo%.git %_dst% || exit /B %ERR_FAIL%
cd %_dst%

git config core.sparseCheckout true
echo %_rpath%>> .git/info/sparse-checkout

git checkout
rmdir /S/Q .git

exit /B %ERR_SUCCESS%


:stderr
echo. %* 1>&2
exit /B 0


:failed

call :stderr  
call :stderr Something went wrong when executing 'svn' and 'git'.
call :stderr Please check their existence, network connection, and fatal description above for details about error.
call :stderr  

exit /B %ERR_FAIL%