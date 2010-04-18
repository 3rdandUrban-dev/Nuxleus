CSC=gmcs

all:IronMath.dll IronPython.dll ipy.exe ipyw.exe

IronMath.dll:
	$(CSC) -t:library -out:IronMath.dll -recurse:Src/IronMath/*.cs

IronPython.dll: IronMath.dll
	$(CSC) -t:library -r:IronMath.dll -r:System.Drawing.dll -r:System.Design.dll -out:IronPython.dll -recurse:Src/IronPython/*.cs

ipy.exe: IronPython.dll
	$(CSC) -t:exe -r:IronPython.dll -out:ipy.exe -recurse:Src/IronPythonConsole/*.cs

ipyw.exe: IronPython.dll
	$(CSC) -t:winexe -r:IronPython.dll -define:IRONPYTHON_WINDOW -out:ipyw.exe -recurse:Src/IronPythonConsole/*.cs
        
install: 
	mkdir -p ${DESTDIR}/usr/bin
	mkdir -p ${DESTDIR}/usr/lib/IronPython
	mkdir -p ${DESTDIR}/usr/lib/IronPython/Lib
	cp *.exe *.dll ${DESTDIR}/usr/lib/IronPython
	cp -r Lib/* ${DESTDIR}/usr/lib/IronPython/Lib
	cp ipy ${DESTDIR}/usr/bin
	cp ipyw ${DESTDIR}/usr/bin
	chmod 755 ${DESTDIR}/usr/bin/ipy
	chmod 755 ${DESTDIR}/usr/bin/ipyw
	rm -rf Src/
