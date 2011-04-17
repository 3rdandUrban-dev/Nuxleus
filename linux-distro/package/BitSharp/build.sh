#!/bin/sh
SVNREPO=http://svn.myrealbox.com/source/trunk/bitsharp/src/
DIST=BitSharp
VERSION=svn

update () {
  if [ ! -d $2 ]; then
    svn checkout $1 $2
  else
    svn update $2
  fi
}

echo 'Updating BitSharp'
update $SVNREPO bitsharp

svn export bitsharp/ BitSharp
tar zcfv BitSharp.tar.gz BitSharp
rm -rf BitSharp
