# -*- coding: utf-8 -*-

import os.path
import cherrypy
import sha
from sets import Set
from urllib import unquote
from amplee.comparer import app_updated_comparer
from amplee.utils import get_isodate, \
     compute_etag_from_feed, compute_etag_from_entry

from bridge import Element as E
from bridge import Attribute as A
from bridge.common import XMLNS_NS, XMLNS_PREFIX
from bridge import Document, PI
from bridge.common import ATOM10_NS, ATOM10_PREFIX

from core.utils import transform_member_resource, render_search_result

__all__ = ['Blog', 'Section', '']

class Blog(object):
    def __init__(self, service, indexer):
        self.service = service
        self.indexer = indexer
        
    @cherrypy.expose
    def index(self):
        path = os.path.join('static', 'homepage.html')
        page = file(path).read()
        cherrypy.response.headers['ETag'] = sha.new(page).hexdigest()
        return page

    @cherrypy.expose
    def tag(self, value, feed=None):
        t = self.indexer.indexes['category_index'].lookup(term=value)
        items = self.indexer.to_dict(t)
        as_feed = feed

        feed = self.service.make_feed(items, entry_processor=transform_member_resource,
                                      title=u"Tag %s" % value, member_comparer=app_updated_comparer)

        params = {u'atom_feed_uri': u'/blog%s/feed' % unicode(cherrypy.request.path_info)}
        if as_feed == None:
            params[u'tag'] = value.decode('utf-8')
            return render_search_result(self.service.service, feed, params=params)
        else:
            feed = feed.xml_root
            A(u'georss', value=u'http://www.georss.org/georss',
              namespace=XMLNS_NS, prefix=XMLNS_PREFIX, parent=feed)
            A(u'geo', value=u'http://www.w3.org/2003/01/geo/wgs84_pos#',
              namespace=XMLNS_NS, prefix=XMLNS_PREFIX, parent=feed)

            #E(u'link', attributes={u'rel': u'self', u'type': u'application/atom+xml;type=feed',
            #                       u'href': params['atom_feed_uri']},
            #  namespace=feed.xml_ns, prefix=feed.xml_prefix)
            
            entries = feed.get_children('entry', feed.xml_ns)
            if entries:
                entry = entries[0]
                updated = feed.get_child('updated', feed.xml_ns)
                eupdated = entry.get_child('updated', feed.xml_ns)
                updated.xml_text = eupdated.xml_text
            cherrypy.response.headers['content-type'] = 'application/atom+xml;type=feed'
            cherrypy.response.headers['ETag'] = compute_etag_from_feed(feed)
            return feed.xml()

        raise cherrypy.NotFound()
        
    @cherrypy.expose
    def search(self, query):
        tokens = query.split(' ')
        s = Set()
        for token in tokens:
            t = self.indexer.indexes['category_index'].lookup(term=token)
            s |= t

        items = self.indexer.to_dict(s)

        feed = self.service.make_feed(items, entry_processor=transform_member_resource,
                                      title=u"Search Result", member_comparer=app_updated_comparer)

        return render_search_result(self.service.service, feed)
        
class Section(object):
    def __init__(self, collection):
        self.collection = collection

    @cherrypy.expose
    def index(self):
        path = os.path.join('static', self.collection.name_or_id)
        page = file(path).read()
        cherrypy.response.headers['ETag'] = sha.new(page).hexdigest()
        return page
        
    @cherrypy.expose
    def default(self, resource_id):
        resource_id = unquote(resource_id).decode('utf-8')
        
	if not resource_id.endswith('.atom'):
           path = os.path.join('static', 'pages', '%s' % resource_id)
           if os.path.exists(path):
              page = file(path).read()
              cherrypy.response.headers['ETag'] = sha.new(page).hexdigest()
              return page
        else:
           member_id, media_id = self.collection.convert_id(resource_id)
           member = self.collection.get_member(member_id)
           if member:
              cherrypy.response.headers['Content-Type'] = 'application/atom+xml;type=entry'
              return member.atom.xml()

        raise cherrypy.NotFound()

    @cherrypy.expose
    def atom(self):
        cherrypy.response.headers['content-type'] = 'application/atom+xml;type=feed'
        feed = self.collection.feed_handler.public_feed.xml_root
        cherrypy.response.headers['ETag'] = compute_etag_from_feed(feed)
        return self.collection.feed_handler.public_xml()
  
    @cherrypy.expose
    def rss(self):
        cherrypy.response.headers['content-type'] = 'application/xml'
        rss = file(os.path.join('static', '%s.rss' % self.collection.name_or_id)).read()
        cherrypy.response.headers['ETag'] = sha.new(rss).hexdigest()
        return rss
  

class Event(Section):
    def __init__(self, collection):
        Section.__init__(self, collection)

    @cherrypy.expose
    def add(self):
        path = os.path.join('static', 'eventform.html')
        return file(path, 'r')
