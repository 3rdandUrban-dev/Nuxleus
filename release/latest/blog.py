# -*- coding: utf-8 -*-

import os.path

import cherrypy

from amplee.loader import loader
from amplee.handler.store.cp import Service, Store
from amplee.indexer import *

from bridge import Element as E

base_dir = os.getcwd()

__all__ = ['base_dir']

import sys
sys.path.append(base_dir)
from core.navig import Blog, Section, Event
from core.queue import QueueManager, QueueHandler
from core.utils import update_home_page, update_collection_page,\
     generate_default_resource_not_found, generate_event_form_page, \
     update_category_list

def setup_indexer():
    ind = Indexer()
    container = ShelveContainer(os.path.join(base_dir, 'index.p'))
    #container = MemcacheContainer(['127.0.0.1:7878'])
    #container = MemoryContainer()
    aid = AtomIDIndexer(name="entry_id", container=container)
    cid = CategoryIndex(name="category_index", container=container)
    ind.register(aid)
    ind.register(cid)

    return ind

def setup_store():
    service, conf = loader(os.path.join(base_dir, 'blog.conf'),
                           encoding='ISO-8859-1', base_path=base_dir)

    return service, conf

def setup_apps():
    indexer = setup_indexer()
    service, conf = setup_store()

    initialize_queues(conf)
    
    servdoc = service.service

    update_home_page(servdoc)
    generate_default_resource_not_found(servdoc)
    generate_event_form_page(servdoc)
    update_category_list(E.load(file(os.path.join(base_dir, 'static', 'catlist.atom'), 'r').read()).xml_root)
    
    workspace = service.get_workspace('blog')

    root = Blog(service, indexer)
    root.pub = Service(service)
    
    col = workspace.get_collection('music')
    col.add_indexer(indexer)
    col.reload_members()
    update_collection_page(servdoc, col.feed_handler.public_feed, col.name_or_id)
    
    root.music = Section(col)
    root.pub.music = Store(col, strict=True)
    
    col = workspace.get_collection('event')
    col.add_indexer(indexer)
    col.reload_members()
    update_collection_page(servdoc, col.feed_handler.public_feed, col.name_or_id)
    
    root.event = Event(col)
    root.pub.event = Store(col, strict=True)

    return root, conf

def initialize_queues(conf):
    manager = QueueManager.get_manager()
    queues = conf.get_all_values('queues')
    for queue_id in queues:
        host, port = conf.get('queues', queue_id).split(':')
        manager.handlers[queue_id] = QueueHandler(queue_id, host, int(port))

def on_startup():
    cherrypy.log("Starting services")
    manager = QueueManager.get_manager()
    for queue_id in manager.handlers:
        manager.handlers[queue_id].register()
        manager.handlers[queue_id].start()

def on_shutdown():
    cherrypy.log("Stopping services")
    manager = QueueManager.get_manager()
    for queue_id in manager.handlers:
        manager.handlers[queue_id].unregister()
        manager.handlers[queue_id].join()

def setup_server():
    app, config = setup_apps()

    method_dispatcher = cherrypy.dispatch.MethodDispatcher()
    conf = {'/': {'tools.etags.on': True,
                  'tools.etags.autotags': False},
            '/pub': {'request.dispatch': method_dispatcher,},
            '/css': {'tools.staticdir.on': True,
                     'tools.staticdir.dir': os.path.join(base_dir, 'static', 'css'),},
            '/media': {'tools.staticdir.on': True,
                       'tools.staticdir.dir': os.path.join(base_dir, 'static', 'media')},
            '/categories/feed': {'tools.staticfile.on': True,
				 'tools.staticfile.filename': os.path.join(base_dir, 'static', 'catlist.atom'),
				 'tools.staticfile.content_types': {'atom': 'application/atom+xml'}},
            '/js': {'tools.staticdir.on': True,
                    'tools.staticdir.dir': os.path.join(base_dir, 'static', 'js')}}

    server_conf = {'global': {'server.socket_port': 12000,
                              'server.socket_host': '0.0.0.0',
                              'tools.proxy.on': True,
                              'tools.proxy.base': 'dev.amp.fm',
                              'error_page.404': os.path.join(base_dir, 'static', 'notfound.html'),
                              'request.show_tracebacks': True,
                              'log.screen': False,
                              'log.error_file': os.path.join(base_dir, 'logs', "error.log"),
                              'log.access_file': os.path.join(base_dir, 'logs', "access.log"),}}

    
    cherrypy.config.update(server_conf)
    cherrypy.tree.mount(app, '/blog', config=conf)
    cherrypy.server.quickstart()
    cherrypy.engine.on_start_engine_list.append(on_startup)
    cherrypy.engine.on_stop_engine_list.append(on_shutdown)
    cherrypy.engine.start()
    
if __name__ == '__main__':
    setup_server()
