#!/bin/bash
NAME=IronRuby
REPO=http://ironruby.rubyforge.org/svn/trunk/

update () {
  if [ ! -d $2 ]; then
    svn checkout $1 $2
  else
    svn update $2
  fi
}

echo 'Updating SVN'
update $REPO $NAME
patch -p0 < staticObject.patch
V=`svnversion $NAME`
EXPORTDIR=$NAME-svnr$V
svn export $NAME $EXPORTDIR
cp $NAME.build $EXPORTDIR/src/$NAME.build
cp ironruby.pc rbx makefile $EXPORTDIR/ 
tar cfz  $EXPORTDIR.tar.gz $EXPORTDIR
rm -rf $EXPORTDIR
