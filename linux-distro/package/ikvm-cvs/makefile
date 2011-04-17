# Hack
install:
	mkdir -p ${DESTDIR}/usr/bin
	mkdir -p ${DESTDIR}/usr/lib/ikvm
	mkdir -p ${DESTDIR}/usr/lib/pkgconfig
	cp bin/*.exe bin/*.dll bin/*.DLL ../LICENSE ${DESTDIR}/usr/lib/ikvm
	cp ikvm ikvmc ikvmstub ${DESTDIR}/usr/bin
	cp ikvm.pc ${DESTDIR}/usr/lib/pkgconfig/ikvm.pc
	chmod 755 ${DESTDIR}/usr/bin/ikvm
	chmod 755 ${DESTDIR}/usr/bin/ikvmc
	chmod 755 ${DESTDIR}/usr/bin/ikvmstub
