#!/bin/bash

SAXONAPILIB=https://saxon.svn.sourceforge.net/svnroot/saxon/latest9.1/bn/csource/api/
SAXONCMD=https://saxon.svn.sourceforge.net/svnroot/saxon/latest9.1/bn/csource/cmd/
DIST=Saxon.NET
VERSION=9.1.0.1

update () {
  if [ ! -d $2 ]; then
    svn checkout $1 $2
  else
    svn update $2
  fi
}

echo 'Updating Saxon.Api'
update $SAXONAPILIB Saxon.Api

echo 'Updating Saxon.Cmd'
update $SAXONCMD Saxon.Cmd

svn export Saxon.Api/ api
svn export Saxon.Cmd/ cmd
