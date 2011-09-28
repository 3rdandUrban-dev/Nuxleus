#!/bin/bash

BZRBASEURI=http://intertwingly.net/code
BZRREPO=venus
DIST=PlanetVenus
VERSION=bzr

update () {
  if [ ! -d $2 ]; then
    bzr get $1/$2
    cd $2/
  else
    cd $2/
    bzr pull
  fi
}

echo 'Updating PlanetVenus Source'
update $BZRBASEURI $BZRREPO

bzr export $DIST-$VERSION
mv $DIST-$VERSION ../

cd ../

cp makefile $DIST-$VERSION/ 

tar zcfv $DIST-$VERSION.tar.gz $DIST-$VERSION

rm -rf $DIST-$VERSION

