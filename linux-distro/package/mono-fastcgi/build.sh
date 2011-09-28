#!/bin/bash
MONOFASTCGI=http://mono-soc-2007.googlecode.com/svn/trunk/brian/FastCgi/

update () {
  if [ ! -d $2 ]; then
    svn checkout $1 $2
  else
    svn update $2
  fi
}

echo 'Updating mono-fastcgi'
update $MONOFASTCGI mono-fastcgi-svn
svn export mono-fastcgi-svn mono-fastcgi
V=`svnversion mono-fastcgi-svn`
tar cfz  mono-fastcgi-svnr$V.tar.gz mono-fastcgi/
rm -rf mono-fastcgi
