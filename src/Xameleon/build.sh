#!/bin/bash
NAME=Xameleon
VERSION=0.2.3
REPO=http://extf.googlecode.com/svn/branches/development-0.2.3/WebApp/

update () {
  if [ ! -d $2 ]; then
    svn checkout $1 $2
  else
    svn update $2
  fi
}

echo 'Updating SVN'
update $REPO $NAME
V=`svnversion $NAME`
PROJECTREVISION=$NAME-$VERSION.svnr$V
EXPORTDIR=webapp
svn export $NAME $EXPORTDIR
mkdir -p transform/functions/fxsl-xslt2/
svn export $NAME/transform/functions/fxsl-xslt2/f transform/functions/fxsl-xslt2/f
tar cfz  $PROJECTREVISION.tar.gz $EXPORTDIR
tar cfz  fxsl-xslt2.tar.gz transform
rm -rf $EXPORTDIR transform

