# -*- coding: utf-8 -*-
import cherrypy
from openidgateway import make_openidgateway

if __name__ == '__main__':
    global_conf = {'engine.autoreload_on' : True,
                   'server.socket_port' : 4000, 
                   'server.socket_host': '127.0.0.1',
                   'server.socket_queue_size': 25,
                   'checker.on': False,
                   }

    cherrypy.config.update(global_conf)
    cherrypy.checker.on = False

    app_conf = {
        'debug': True,
        'base_url': 'http://localhost:4000/',
        'xsltemplate_namespace': 'http://amp.fm/python/xsltemplates/',
    }
    
    app = make_openidgateway(app_conf)
    cherrypy.tree.graft(app, script_name='/')

    cherrypy.server.quickstart()
    try:
       cherrypy.engine.start()
    except KeyboardInterrupt:
       cherrypy.engine.stop()

    
