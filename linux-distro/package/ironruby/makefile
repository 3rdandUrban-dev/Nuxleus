all:nant

nant:
	cd src; nant

install: 
	mkdir -p ${DESTDIR}/usr/bin
	mkdir -p ${DESTDIR}/usr/lib/ironruby
	mkdir -p ${DESTDIR}/usr/lib/pkgconfig
	cp src/*.dll src/*.exe ${DESTDIR}/usr/lib/ironruby/
	chmod -R 755 ${DESTDIR}/usr/lib/ironruby
	cp License.html ${DESTDIR}/usr/lib/ironruby/License.html
	cp rbx ${DESTDIR}/usr/bin
	chmod a+x ${DESTDIR}/usr/bin/rbx
	cp ironruby.pc ${DESTDIR}/usr/lib/pkgconfig/ironruby.pc
