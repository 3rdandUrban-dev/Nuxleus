Welcome to WebPath

Read this first:

If you get an error message "ImportError: No module named lex", you need to separately obtain Python Lex-Yacc <http://www.dabeaz.com/ply> and install it or put the file in the same directory. All third-party code was scrubbed from this project to prepare it for release as open source. If you think you can do a better job packaging, feel free to pitch in. :-)

About WebPath:

This is a small implementation of XPath 2.0 using a novel parsing technique called Top Down Operator Precedence, for which I owe thanks to Douglas Crockford for introducing me to it. I never thought I'd be saying this, bu at its core, XPath 2.0 is a surprisingly simple and elegant language.

The primary goals in developing this were rapid development (it was substantially completed during a "Hack Day" at Yahoo!) and providing a platform for ready experimentation. An explicit non-goal was strict conformance to the XPath-family of specifications, though with a release to open source, that direction is definitely open.

This software was presented and demonstrated first at XML 2007. The slides are online at http://dubinko.info/events/XML2007/

What next for this code? Instead of writing a long diatribe here, I'll put it in my blog. Come visit http://dubinko.info/blog, where I'm sure to be spouting off about something XPath-related. :)

Enjoy, and let me know what cool things you put this code to use with.

January 24, 2008
Micah Dubinko