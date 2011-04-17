#!/bin/bash
NAME=Ruby.NET
REPO=http://rubydotnetcompiler.googlecode.com/svn/trunk/

update () {
  if [ ! -d $2 ]; then
    svn checkout $1 $2
  else
    svn update $2
  fi
}

echo 'Updating SVN'
update $REPO $NAME
svn export $NAME $NAME-svn
V=`svnversion $NAME`
tar cfz  $NAME-svnr$V.tar.gz $NAME-svn
rm -rf $NAME-svn
