#!/bin/bash
rm -rf Saxon/net/sf/saxon/ant
find Saxon/ -name *.java > allsources.lst
patch Saxon/net/sf/saxon/number/Numberer_fr.java < Numberer_fr.java.patch
patch Saxon/net/sf/saxon/number/Numberer_de.java < ./Numberer_de.java.patch
