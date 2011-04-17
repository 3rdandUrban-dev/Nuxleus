# Hack
install:
	mkdir -p ${DESTDIR}/usr/bin
	mkdir -p ${DESTDIR}/usr/lib/ecj
	ikvmc -out:ecj.exe ecj-3.4M2.jar
	cp ecj.exe ${DESTDIR}/usr/lib/ecj
	cp ecj ${DESTDIR}/usr/bin
	chmod 755 ${DESTDIR}/usr/bin/ecj
