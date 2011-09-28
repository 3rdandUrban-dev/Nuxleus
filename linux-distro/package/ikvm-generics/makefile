# Hack
install:
	mkdir -p ${DESTDIR}/usr/bin
	mkdir -p ${DESTDIR}/usr/lib/ikvm-generics
	mkdir -p ${DESTDIR}/usr/lib/pkgconfig
	cp bin/*.exe bin/*.dll bin/*.DLL ../LICENSE ${DESTDIR}/usr/lib/ikvm-generics
	cp ikvm-generics ikvmc-generics ikvmstub-generics ${DESTDIR}/usr/bin
	cp ikvm-generics.pc ${DESTDIR}/usr/lib/pkgconfig/ikvm-generics.pc
	chmod 755 ${DESTDIR}/usr/bin/ikvm-generics
	chmod 755 ${DESTDIR}/usr/bin/ikvmc-generics
	chmod 755 ${DESTDIR}/usr/bin/ikvmstub-generics
