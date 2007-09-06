#!/bin/bash
NAME=nuxleus
DIRNAME=nuxroot/srv
VERSION=0.2.3.2
REPO=http://nuxleus.googlecode.com/svn/trunk/nuxleus/nuxroot/srv

update () {
  if [ ! -d $2 ]; then
    svn checkout $1 $2
  else
    svn update $2
  fi
}

echo 'Updating SVN'
update $REPO $DIRNAME
V=`svnversion $DIRNAME`
PROJECTREVISION=$NAME-$VERSION.svnr$V
EXPORTDIR=srv
svn export $DIRNAME $EXPORTDIR
tar cfz  $PROJECTREVISION.tar.gz $EXPORTDIR
rm -rf $EXPORTDIR

