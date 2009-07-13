CSC=gmcs

all:Saxon.Api.dll Transform.exe Query.exe

Saxon.Api.dll: getsvn
	$(CSC) -t:library -keyfile:Extf.Net.snk -out:Saxon.Api.dll -pkg:ikvm -r:saxon9.dll -out:Saxon.Api.dll -recurse:api/*.cs

Transform.exe: Saxon.Api.dll
	$(CSC) -t:exe -r:saxon9.dll -pkg:ikvm -out:Transform.exe cmd/Transform.cs

Query.exe: Saxon.Api.dll
	$(CSC) -t:exe -r:saxon9.dll -pkg:ikvm -out:Query.exe cmd/Query.cs
        
getsvn:
	sh ./build.sh
install: 
	mkdir -p ${DESTDIR}/usr/bin
	mkdir -p ${DESTDIR}/usr/lib/saxon
	mkdir -p ${DESTDIR}/usr/lib/pkgconfig
	cp *.exe *.dll ${DESTDIR}/usr/lib/saxon
	cp transform query ${DESTDIR}/usr/bin
	#cp saxon.pc ${DESTDIR}/usr/lib/pkgconfig/saxon.pc
	chmod 755 ${DESTDIR}/usr/bin/transform
	chmod 755 ${DESTDIR}/usr/bin/query
