# Hack
install:
	mkdir -p ${DESTDIR}/usr/bin
	mkdir -p ${DESTDIR}/usr/lib/ikvm-dev
	mkdir -p ${DESTDIR}/usr/lib/pkgconfig
	cp bin/*.exe bin/*.dll bin/*.DLL ../LICENSE ${DESTDIR}/usr/lib/ikvm-dev
	cp ikvm-dev ikvmc-dev ikvmstub-dev ${DESTDIR}/usr/bin-dev
	cp ikvm-dev.pc ${DESTDIR}/usr/lib/pkgconfig/ikvm-dev.pc
	chmod 755 ${DESTDIR}/usr/bin/ikvm-dev
	chmod 755 ${DESTDIR}/usr/bin/ikvmc-dev
	chmod 755 ${DESTDIR}/usr/bin/ikvmstub-dev
