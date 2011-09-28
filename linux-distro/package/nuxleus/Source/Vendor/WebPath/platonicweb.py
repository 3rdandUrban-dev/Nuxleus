# Copyright (c) 2008 Yahoo! Inc.  All rights reserved.
# The copyrights embodied in the content of this file are licensed
# by Yahoo! Inc. under the BSD (revised) open source license


import unittest
from xml.dom.minidom import parse, parseString
from xml.parsers.expat import ExpatError, ErrorString
import urllib2
import tempfile
import os

def get(url):
    """get a particular URL, no matter how grotty, and return XML
    
    Callers should be able to handle, BadStatusLine or URLError
    exceptions (Some proxies can occasionally return nonstandard codes) """
    
    fullurl = "http://cgi.w3.org/cgi-bin/tidy?forceXML=on&docAddr=" + urllib2.quote(url)

    try:
        page = urllib2.urlopen(fullurl)
        markup = page.read()
    except urllib2.httplib.BadStatusLine, e:
        msg = "Bad Status Line Exception reading " + url + " " + e.code
        print "***" + msg
        raise
    except urllib2.URLError, e:
        msg = "Connection error reading " + url + " " + e.code
        print "***" + msg
        raise
    except Exception, e:
        msg = "Unknown exception reading " + url
        if hasattr(e, 'code'):
            print e.code
        print "***" + msg
        raise
    
    dom = None
    try:
        dom = parseString(markup)
    except ExpatError, e:
        # even with Tidy, still see occasional
        # wf-errors. At least keep a record of the offenders...
        print "PARSE ERROR:", ErrorString(e.code), " line:" + str(e.lineno), " offset:", str(e.offset), url
        print markup.splitlines()[e.lineno - 1]
        if not os.path.isdir("unparsable"): os.mkdir("unparsable")
        import time
        tstr = str(time.time())
        f=open("unparsable/" + tstr + ".html","w").write(markup)
        if f: f.close()

    return dom

class TestPlatonicWeb(unittest.TestCase):
    def testGet(self):
        dom = get("http://www.dubinko.info")
        self.assert_(dom is not None)
        
class TestKnownTroublesomeURLs(unittest.TestCase):
    def testDirtyGet(self):
        dom = get("http://www.yahoo.com")
        self.assert_(dom is not None)

class TestKnownTroublesomeURLs2(unittest.TestCase):
    def testDirtyGet2(self):
        dom = get("http://www.ysearchblog.com")
        self.assert_(dom is not None)

class TestKnownTroublesomeURLs3(unittest.TestCase):
    def testDirtyGet3(self):
        dom = get("http://korjov-dmitry.livejournal.com/37371.html")
        self.assert_(dom is not None)

class TestKnownTroublesomeURLs4(unittest.TestCase):
    def testDirtyGet4(self):
        dom = get("http://www.become.com")
        self.assert_(dom is not None)
                                                                       

def main():    
    unittest.main()
    
if __name__ == "__main__":
    main()
