PROG = start-stop-daemon

all: start-stop-daemon 

start-stop-daemon:
	gcc start-stop-daemon.c -o start-stop-daemon

install:
	$(INSTALL) -m 0755 -o root -g root $(PROG) $(prefix)/sbin

clean:
	rm -f $(PROG)
