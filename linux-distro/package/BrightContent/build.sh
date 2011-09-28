#!/bin/sh
#SVNREPO=http://brightcontent.googlecode.com/svn/trunk/
SVNREPO=http://brightcontent.googlecode.com/svn/branches/ampleeintegration/
DIST=BrightContent

update () {
  if [ ! -d $2 ]; then
    svn checkout $1 $2
  else
    svn update $2
  fi
}

echo 'Updating '$DIST
update $SVNREPO $DIST
VERSION=`svnversion $DIST/`


svn export $DIST/ $DIST-$VERSION
tar zcfv $DIST-svn.r$VERSION.tar.gz $DIST-$VERSION
rm -rf $DIST-$VERSION
