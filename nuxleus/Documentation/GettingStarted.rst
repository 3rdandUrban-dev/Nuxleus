==========================================
 Getting Started with Nuxleus Development
==========================================

First off check out nuxleus from the repo on google code. 

svn co http://nuxleus.googlecode.com/svn/trunk/nuxcleus .

You will be specifically working with the "nuxleus" directory.

Building Nuxleus
================

To build Nuxleus you will be using nant. You will also be using mono
so be sure you have already installed the mono runtime. You will also
need to have Python 2.5 installed. 

You can build nuxleus by typing "nant" in the nuxleus directory. That
will build everything and you will have a "~/nuxleus/src/build/"
directory that contains the built dlls. This also contains a copy of
xsp2.exe, the mono web server.


Running Nuxleus
===============

To actually run the server a few things need to happen. First, the
queue server must be run. This depends on Memcache also being run on
port 9876.


The Queue Server
----------------

The Queue Server is a python applcation you will need to install. To
install the app along with the necessary libaries, go to
"~/nuxleus/src/Dependencies/Queue/" where you will find a set of
directories. You will want to install all these libraries in the
following order.

  - axon
  - kaemalia
  - cherrypy
  - bridge
  - bucker
  - python-memcached-1.40
  - llup

The most important piece is that axon and kaemalia get installed first
as the following packages such as llup and bucker require them. To
actually install the libraries, go into the directory and run:

  python setup.py install

With the libararies and queue server installed you can start it via
a shell script with:
  
  ~/nuxleus/Scripts/qs.sh start

You will also need to get memcache up and running via:

  memcached -d


Starting Nuxleus
----------------

With the Queue Server running you can start the actual web server that
will accept requests. To start the server:

  mono ~/nuxleus/src/build/bin/xsp2.exe ~/nuxleus/Web/Development/

That will start the server and it will use the files in the
"~/nuxleus/Web/Development/" directory for handling requests. 

You should now be able to visit http://localhost:8080 to see the
application running. 
