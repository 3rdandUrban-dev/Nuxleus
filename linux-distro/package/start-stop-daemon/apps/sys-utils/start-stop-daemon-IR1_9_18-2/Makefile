PROG = start-stop-daemon

all: $(PROG)

install: all
	$(INSTALL) -m 0755 -o root -g root $(PROG) $(prefix)/sbin

clean:
	rm -f $(PROG)
