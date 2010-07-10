#!/bin/bash
grub --no-floppy --batch <<EOF
device (hd0) nuxleus.core.img
geometry (hd0) 4096 16 63
root (hd0,0)
setup (hd0)
quit
EOF
