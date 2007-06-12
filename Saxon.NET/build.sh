#!/bin/bash

SAXONAPILIB=https://svn.sourceforge.net/svnroot/saxon/latest8.9/bn/csource/api/
SAXONCMD=https://svn.sourceforge.net/svnroot/saxon/latest8.9/bn/csource/cmd/
DIST=Saxon.NET
VERSION=8.9.0.3

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
