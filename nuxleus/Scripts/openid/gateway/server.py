# -*- coding: utf-8 -*-
import cherrypy
from openidgateway import make_openidgateway

if __name__ == '__main__':
    global_conf = {'engine.autoreload_on' : False,
                   'server.socket_port' : 4000, 
                   'server.socket_host': '127.0.0.1',
                   'server.socket_queue_size': 25,
                   'log.screen': True,
                   'log.access_file': './access.log',
                   'log.error_file': './error.log',
                   'checker.on': False,
                   'tools.proxy.on': True,
                   'tools.proxy.base': 'http://dev.amp.fm'}

    cherrypy.config.update(global_conf)
    cherrypy.checker.on = False

    app_conf = {
        'debug': True,
        'base_url': 'http://dev.amp.fm/gatekeeper/',
        'xsltemplate_namespace': 'http://amp.fm/python/xsltemplates/',
    }
    
    app = make_openidgateway(app_conf)
    cherrypy.tree.graft(app, script_name='/gatekeeper')

    class Dummy(object):
        pass

    cherrypy.quickstart(Dummy(), '/')
    #cherrypy.server.quickstart()
    #try:
    #    cherrypy.engine.start()
    #except KeyboardInterrupt:
    #    cherrypy.engine.stop()

    
