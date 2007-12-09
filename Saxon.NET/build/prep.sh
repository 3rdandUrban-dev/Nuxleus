#!/bin/bash
rm -rf Saxon/net/sf/saxon/ant Saxon/net/sf/saxon/xqj Saxon/net/sf/saxon/s9api Saxon/net/sf/saxon/pull/PullToStax.java Saxon/net/sf/saxon/pull/StaxBridge.java
find Saxon/ -name *.java > allsources.lst
