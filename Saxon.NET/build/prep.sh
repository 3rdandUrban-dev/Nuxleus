#!/bin/bash
rm -rf Saxon/net/sf/saxon/ant Saxon/net/sf/saxon/dom Saxon/net/sf/saxon/dom4j Saxon/net/sf/saxon/java Saxon/net/sf/saxon/javax Saxon/net/sf/saxon/jdom/ Saxon/net/sf/saxon/pull/PullToStax.java Saxon/net/sf/saxon/pull/StaxBridge.java Saxon/net/sf/saxon/s9api Saxon/net/sf/saxon/xom Saxon/net/sf/saxon/xpath Saxon/net/sf/saxon/xqj
find Saxon/ -name *.java > allsources.lst
