#!/bin/sh
# Copyright (c) 2018  Denis Kuzmin [ entry.reg@gmail.com ]
# Specially for https://github.com/3F/Examples
# [[ Bash version ]] / Batch version

ERR_SUCCESS=0
ERR_FAIL=1

function stderr()
{
    echo "$@" 1>&2 
}

function viaSvn()
{
    local repo=$1; local rpath=$2; local dst=$3;

    echo trying via svn ...

    local svnurl=${repo}.git/trunk/$rpath

    svn export $svnurl $dst/$rpath || return $ERR_FAIL
    return $ERR_SUCCESS
}

function viaGit()
{
    local repo=$1; local rpath=$2; local dst=$3;

    echo trying via git ...

    git clone --depth=1 --no-checkout ${repo}.git $dst || return $ERR_FAIL
    cd $dst

    git config core.sparseCheckout true
    echo ${rpath}>> .git/info/sparse-checkout

    git checkout
    rm -rf .git

    return $ERR_SUCCESS
}

function failed()
{
    stderr 
    stderr Something went wrong when executing 'svn' and 'git'. 
    stderr Please check their existence, network connection, and fatal description above for details about error.
    stderr 
    return $ERR_FAIL
}

# ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ 
#  ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ 

_url=$1; _dst=$2;

if [ -z $_url ]; then
    echo Allowed commands: $0 {url} [{destination_path}]
    echo Sample: $0 https://github.com/3F/Examples/tree/master/DllExport/BasicExport
    exit $ERR_SUCCESS
fi

# _ _ _ _ _ 

if [ -z $_dst ]; then
    _dst=$(echo $_url| cut -d'/' -f 5)
fi

_repo=$(echo $_url| cut -d'/' -f -5)

# /tree/master/...
_rpath=$(echo $_url| cut -d'/' -f 8-)

# _ _ _ _ _ 

viaSvn $_repo $_rpath $_dst; lret=$?

if [[ $lret -ne $ERR_SUCCESS ]]
then
    viaGit $_repo $_rpath $_dst; lret=$?
    [[ $lret -ne $ERR_SUCCESS ]] && failed 
fi


# -
[[ $lret -ne $ERR_SUCCESS ]] && exit $ERR_FAIL || exit $ERR_SUCCESS
