IPCE=https://svn.sourceforge.net/svnroot/fepy/IPCE

update () {
  if [ ! -d $2 ]; then
    svn checkout $1 $2
  else
    svn update $2
  fi
}

echo 'Updating IPCE'
update $IPCE IPCE

cd rPath/
./download.sh
./build.sh
V=`svnversion`
mv -f ipce-svn$V.tar.gz ../
