# -*- coding: utf-8 -*-
from __future__ import with_statement

from base64 import b64encode
from datetime import datetime

import threading, time
import sha, os, os.path

from bridge import Element as E
from bridge import Document, PI
from amplee.atompub.member.atom import EntryResource
from amplee.atompub.member.generic import MediaResource
from amplee.utils import get_isodate, generate_uuid_uri, parse_query_string
from bridge.common import LLUP_NS, LLUP_PREFIX, ATOM10_NS, ATOM10_PREFIX, THR_NS, THR_PREFIX
from bucker.api.message import PushMessage
from llup.api.notification import *

from core.utils import transform_member_resource, render, \
     update_collection_page, update_entry_page, atom_to_rss, \
     update_category_list
from core.queue import QueueManager

__all__ = ['EventMember', 'EventHandler',
           'EventFormMember', 'EventFormHandler']

###########################################################
# The following two classes handle the
# application/atom+xml;type=entry media-type
###########################################################
class EventMember(EntryResource):
    def __init__(self, collection, **kwargs):
        EntryResource.__init__(self, collection, **kwargs)

    def generate_atom_id(self, entry, slug=None):
        if not slug:
            title = entry.get_child('title', entry.xml_ns)
            if title:
                slug = title.xml_text
            if not slug:
                slug = '%f' % time.time()
        return u'tag:%s:%s' % (self.collection.name_or_id,
                               sha.new(slug).hexdigest())

    def generate_resource_id(self, entry, slug=None):
        if slug:
            return slug.replace(' ','_').decode('utf-8')
        title = entry.get_child('title', entry.xml_ns)
        if title:
            return title.xml_text.replace(' ','_')

        return str(int(time.time()))
        
class EventHandler(object):
    def __init__(self, member_type):
        # Instance of amplee.handler.MemberType
        self.member_type = member_type
        self.qid = self.member_type.params['queue_id']
	self.seen_categories = []
        self.lock = threading.Lock()
                
    def on_update_feed(self, member):
        member.collection.feed_handler.set(member.collection.feed)
        
    def on_create(self, member, content):
        return member, None
    
    def on_created(self, member):
        # In case the POSTed atom entry had the
        # app:control/app:draft set to 'yes'
        # Then we don't want the recipe to appear into the
        # public feed.
        if not member.draft:
            public = transform_member_resource(member)
            if public:
                member.collection.feed_handler.add(public)

                path = os.path.join(self.member_type.params['blog_entry_path'], member.media_id)
                servdoc = member.collection.workspace.service.service
                with self.lock:
                    update_entry_page(servdoc, public, path)
                    update_collection_page(servdoc,
                                           member.collection.feed_handler.public_feed,
                                           member.collection.name_or_id)
                    atom_to_rss(member.collection.feed_handler.public_feed,
                                member.collection.name_or_id)

		    update_category_list(public.xml_root)

        manager = QueueManager.get_manager()
        handler = manager.get_handler(self.qid)
        if handler:
            n = Notification(u'create')
            categories = member.atom.get_children('category', ATOM10_NS)
            for cat in categories:
                n.categories.append(Category(cat.get_attribute_value('term'),
                                             cat.get_attribute_value('scheme'),
                                             cat.get_attribute_value('label')))
                                             
            links = member.atom.get_children('link', ATOM10_NS)
            for link in links:
                n.links.append(Link(link.get_attribute_value('href'),
                                    link.get_attribute_value('rel'),
                                    link.get_attribute_value('type')))

            authors = member.atom.get_children('author', ATOM10_NS)
            for author in authors:
                name = author.get_child('name', ATOM10_NS)
                if name: name = name.xml_text
                else: name = None

                uri = author.get_child('uri', ATOM10_NS)
                if uri: uri = uri.xml_text
                else: uri = None

                email = author.get_child('email', ATOM10_NS)
                if email: email = email.xml_text
                else: email = None

                n.authors.append(Author(name, uri, email))

            m = PushMessage()
            m.request_id = str(time.time())
            m.qid = self.qid
            m.payload = b64encode(n.xml())
            handler.process(m)

    def on_update(self, existing_member, new_member, new_content):
        # We ensure that dates will be modified
        new_member.update_dates()
        return new_member, None

    def on_updated(self, member):
        path = os.path.join(self.member_type.params['blog_entry_path'], member.media_id)
        if not member.draft:
            public = transform_member_resource(member)
            if public:
                member.collection.feed_handler.replace(public)
                servdoc = member.collection.workspace.service.service
                
                with self.lock:
                    update_entry_page(servdoc, public, path)
                    update_collection_page(servdoc,
                                           member.collection.feed_handler.public_feed,
                                           member.collection.name_or_id)
                    atom_to_rss(member.collection.feed_handler.public_feed,
                                member.collection.name_or_id)
        else:
            member.collection.feed_handler.remove(entry)
            with self.lock:
                try:
                    os.unlink(path)
                except OSError:
                    pass
                init_collection_page(member.collection.workspace.service.service,
                                     member.collection.feed_handler.public_feed,
                                     member.collection.name_or_id)
                atom_to_rss(member.collection.feed_handler.public_feed,
                            member.collection.name_or_id)
        
        manager = QueueManager.get_manager()
        handler = manager.get_handler(self.qid)
        if handler:
            n = Notification(u'update')
            categories = member.atom.get_children('category', ATOM10_NS)
            for cat in categories:
                n.categories.append(Category(cat.get_attribute_value('term'),
                                             cat.get_attribute_value('scheme'),
                                             cat.get_attribute_value('label')))
                                             
            links = member.atom.get_children('link', ATOM10_NS)
            for link in links:
                n.links.append(Link(link.get_attribute_value('href'),
                                    link.get_attribute_value('rel'),
                                    link.get_attribute_value('type')))

            authors = member.atom.get_children('author', ATOM10_NS)
            for author in authors:
                name = author.get_child('name', ATOM10_NS)
                if name: name = name.xml_text
                else: name = None

                uri = author.get_child('uri', ATOM10_NS)
                if uri: uri = uri.xml_text
                else: uri = None

                email = author.get_child('email', ATOM10_NS)
                if email: email = email.xml_text
                else: email = None

                n.authors.append(Author(name, uri, email))
            
            m = PushMessage()
            m.request_id = str(time.time())
            m.qid = self.qid
            m.payload = b64encode(n.xml())
            handler.process(m)

    def on_deleted(self, member):
        member.collection.feed_handler.remove(member.atom)
        with self.lock:
            path = os.path.join(self.member_type.params['blog_entry_path'], member.media_id)
            try:
                os.unlink(path)
            except OSError:
                pass
            update_collection_page(member.collection.workspace.service.service,
                                   member.collection.feed_handler.public_feed,
                                   member.collection.name_or_id)
            atom_to_rss(member.collection.feed_handler.public_feed,
                        member.collection.name_or_id)
 
        manager = QueueManager.get_manager()
        handler = manager.get_handler(self.qid)
        if handler:
            n = Notification(u'delete')
            categories = member.atom.get_children('category', ATOM10_NS)
            for cat in categories:
                n.categories.append(Category(cat.get_attribute_value('term'),
                                             cat.get_attribute_value('scheme'),
                                             cat.get_attribute_value('label')))
                                             
            links = member.atom.get_children('link', ATOM10_NS)
            for link in links:
                n.links.append(Link(link.get_attribute_value('href'),
                                    link.get_attribute_value('rel'),
                                    link.get_attribute_value('type')))

            authors = member.atom.get_children('author', ATOM10_NS)
            for author in authors:
                name = author.get_child('name', ATOM10_NS)
                if name: name = name.xml_text
                else: name = None

                uri = author.get_child('uri', ATOM10_NS)
                if uri: uri = uri.xml_text
                else: uri = None

                email = author.get_child('email', ATOM10_NS)
                if email: email = email.xml_text
                else: email = None

                n.authors.append(Author(name, uri, email))
            
            m = PushMessage()
            m.request_id = str(time.time())
            m.qid = self.qid
            m.payload = b64encode(n.xml())
            handler.process(m)

###########################################################
# The following two classes handle the
# application/x-www-form-urlencoded media-type
###########################################################
class EventFormMember(MediaResource):
    def __init__(self, collection, **kwargs):
        MediaResource.__init__(self, collection, **kwargs)

    def generate_atom_id(self, slug=None):
        if not slug:
            slug = '%f' % time.time()
        return u'tag:%s:%s' % (self.collection.name_or_id,
                               sha.new(slug).hexdigest())

    def generate_resource_id(self, entry, slug=None):
        return str(int(time.time()))

    def create(self, source, slug=None):
        length = int(self.raw_headers['content-length'])
        qs = parse_query_string(source.read(length))

        qs = MediaResource.create(self, qs, slug)

        return qs
    
    def update(self, source, existing_member=None):
        length = int(self.raw_headers['content-length'])
        qs = parse_query_string(source.read(length))

        qs = MediaResource.update(self, qs, existing_member)

        return qs
    
class EventFormHandler(object):
    def __init__(self, member_type):
        # Instance of amplee.handler.MemberType
        self.member_type = member_type
        self.qid = self.member_type.params['queue_id']
        self.lock = threading.Lock()
                
    def on_update_feed(self, member):
        member.collection.feed_handler.set(member.collection.feed)
        
    def on_create(self, member, content):
        entry = member.atom

        title = entry.get_child('title', entry.xml_ns)
        if not title:
            E(u'title', namespace=entry.xml_ns, prefix=entry.xml_prefix, parent=entry)
        title.xml_text = content.get('name')
        
        author = entry.get_child('author', entry.xml_ns)
        author_name = self.member_type.params['name']
        if author:
            name = author.get_child('name', entry.xml_ns)
            if not name:
                name = E(u'name', namespace=entry.xml_ns, prefix=entry.xml_prefix, parent=author)
            name.xml_text = author_name
        else:
            author = E(u'author', namespace=entry.xml_ns, prefix=entry.xml_prefix, parent=entry)
            E(u'name', namespace=entry.xml_ns, prefix=entry.xml_prefix, parent=author)
            name.xml_text = author_name

        location = content.get('location', None)
        if location:
            lat, lg = location.decode('utf-8').split(' ')
            g = E(u'point', content=location.decode('utf-8'),
                  namespace=u'http://www.georss.org/georss', prefix=u'georss', parent=entry)
            geo = E('Point', namespace=u'http://www.w3.org/2003/01/geo/wgs84_pos#', prefix=u'geo', parent=entry)
            E(u'lat', content=lat, namespace=u'http://www.w3.org/2003/01/geo/wgs84_pos#', prefix=u'geo', parent=geo)
            E(u'long', content=lg, namespace=u'http://www.w3.org/2003/01/geo/wgs84_pos#', prefix=u'geo', parent=geo)

	#cherrypy.request._content = content

        
        startdate = content.get('startdate', None)
        if startdate:
            try:
            	tokens = startdate.split('/')
            	dt = get_isodate(datetime(int(tokens[2]), int(tokens[0]), int(tokens[1])))
            except:
                dt = unicode(startdate)
            E(u'start-time', content=dt, namespace=LLUP_NS, prefix=LLUP_PREFIX, parent=entry)

        enddate = content.get('enddate', None)
        if enddate:
            try:
                tokens = enddate.split('/')
                dt = get_isodate(datetime(int(tokens[2]), int(tokens[0]), int(tokens[1])))
            except:
                dt = unicode(enddate)
            E(u'expires', content=dt, namespace=LLUP_NS, prefix=LLUP_PREFIX, parent=entry)
            E(u'end-time', content=dt, namespace=LLUP_NS, prefix=LLUP_PREFIX, parent=entry)

        genre = content.get('genre', None)
        if genre:
            genre = genre.decode('utf-8')
            attrs = {u'term': genre, u'label': genre.capitalize(),
                     u'scheme': u'http://personplacething.info/music/genre'}
            E(u'category', attributes=attrs, namespace=entry.xml_ns,
              prefix=entry.xml_prefix, parent=entry)

        cats = content.get('tags', None)
        if cats:
            cats = cats.split(',')
            for cat in cats:
                cat = cat.decode('utf-8')
                attrs = {u'term': cat, u'label': cat}
                E(u'category', attributes=attrs, namespace=entry.xml_ns,
                  prefix=entry.xml_prefix, parent=entry)

        desc = content.get('description', '')
        content = entry.get_child('content', entry.xml_ns)
        if not content:
            content = E(u'content', namespace=entry.xml_ns,
                        prefix=entry.xml_prefix, parent=entry)
        content.xml_text = desc

	#print entry.xml()

        return member, None
    
    def on_created(self, member):
        # In case the POSTed atom entry had the
        # app:control/app:draft set to 'yes'
        # Then we don't want the recipe to appear into the
        # public feed.
        
        public = None
        if not member.draft:
            public = transform_member_resource(member)
            if public:
                member.collection.feed_handler.add(public)
                
                path = os.path.join(self.member_type.params['blog_entry_path'], member.media_id)
                servdoc = member.collection.workspace.service.service
                with self.lock:
                    update_entry_page(servdoc, public, path)
                    update_collection_page(servdoc,
                                           member.collection.feed_handler.public_feed,
                                           member.collection.name_or_id)
                    atom_to_rss(member.collection.feed_handler.public_feed,
                                member.collection.name_or_id)

                    update_category_list(public.xml_root)

            manager = QueueManager.get_manager()
            handler = manager.get_handler(self.qid)
            #print handler
	    #print public.xml_root.xml()
	    if handler:
            	n = Notification(u'create')
            	categories = public.xml_root.get_children('category', ATOM10_NS)
                for cat in categories:
                    n.categories.append(Category(cat.get_attribute_value('term'),
                                                 cat.get_attribute_value('scheme'),
                                                 cat.get_attribute_value('label')))
                    
                links = public.xml_root.get_children('link', ATOM10_NS)
                for link in links:
                    n.links.append(Link(link.get_attribute_value('href'),
                                        link.get_attribute_value('rel'),
                                        link.get_attribute_value('type')))
                    
                authors = public.xml_root.get_children('author', ATOM10_NS)
                for author in authors:
                    name = author.get_child('name', ATOM10_NS)
                    if name: name = name.xml_text
                    else: name = None

                    uri = author.get_child('uri', ATOM10_NS)
                    if uri: uri = uri.xml_text
                    else: uri = None
                    
                    email = author.get_child('email', ATOM10_NS)
                    if email: email = email.xml_text
                    else: email = None

                    n.authors.append(Author(name, uri, email))

	        #print n.xml()
                m = PushMessage()
                m.request_id = str(time.time())
                m.qid = self.qid
                m.payload = b64encode(n.xml())
                handler.process(m)

    def on_update(self, existing_member, new_member, new_content):
        # We ensure that dates will be modified
        new_member.update_dates()
        
        entry = new_member.atom
        author = entry.get_child('author', entry.xml_ns)
        author_name = self.member_type.params['name']
        if author:
            name = author.get_child('name', entry.xml_ns)
            if not name:
                name = E(u'name', namespace=entry.xml_ns, prefix=entry.xml_prefix)
            name.xml_text = author_name
        else:
            author = E(u'author', namespace=entry.xml_ns, prefix=entry.xml_prefix)
            E(u'name', namespace=entry.xml_ns, prefix=entry.xml_prefix, parent=author)
            name.xml_text = author_name

        return new_member, None

    def on_updated(self, member):
        path = os.path.join(self.member_type.params['blog_entry_path'], member.media_id)
        if not member.draft:
            public = transform_member_resource(member)
            if public:
                member.collection.feed_handler.replace(public)
                servdoc = member.collection.workspace.service.service
                
                with self.lock:
                    update_entry_page(servdoc, public, path)
                    update_collection_page(servdoc,
                                           member.collection.feed_handler.public_feed,
                                           member.collection.name_or_id)
                    atom_to_rss(member.collection.feed_handler.public_feed,
                                member.collection.name_or_id)
        else:
            member.collection.feed_handler.remove(entry)
            with self.lock:
                try:
                    os.unlink(path)
                except OSError:
                    pass
                init_collection_page(member.collection.workspace.service.service,
                                     member.collection.feed_handler.public_feed,
                                     member.collection.name_or_id)
                atom_to_rss(member.collection.feed_handler.public_feed,
                            member.collection.name_or_id)
        
        manager = QueueManager.get_manager()
        handler = manager.get_handler(self.qid)
	#print handler
	#print member.atom.xml()
        if handler:
            n = Notification(u'update')
            categories = member.atom.get_children('category', ATOM10_NS)
            for cat in categories:
                n.categories.append(Category(cat.get_attribute_value('term'),
                                             cat.get_attribute_value('scheme'),
                                             cat.get_attribute_value('label')))
                                             
            links = member.atom.get_children('link', ATOM10_NS)
            for link in links:
                n.links.append(Link(link.get_attribute_value('href'),
                                    link.get_attribute_value('rel'),
                                    link.get_attribute_value('type')))

            authors = member.atom.get_children('author', ATOM10_NS)
            for author in authors:
                name = author.get_child('name', ATOM10_NS)
                if name: name = name.xml_text
                else: name = None

                uri = author.get_child('uri', ATOM10_NS)
                if uri: uri = uri.xml_text
                else: uri = None

                email = author.get_child('email', ATOM10_NS)
                if email: email = email.xml_text
                else: email = None

                n.authors.append(Author(name, uri, email))
            
	    #print n.xml()
            m = PushMessage()
            m.request_id = str(time.time())
            m.qid = self.qid
            m.payload = b64encode(n.xml())
            handler.process(m)

    def on_deleted(self, member):
        member.collection.feed_handler.remove(member.atom)
        with self.lock:
            path = os.path.join(self.member_type.params['blog_entry_path'], member.media_id)
            try:
                os.unlink(path)
            except OSError:
                pass
            update_collection_page(member.collection.workspace.service.service,
                                   member.collection.feed_handler.public_feed,
                                   member.collection.name_or_id)
            atom_to_rss(member.collection.feed_handler.public_feed,
                        member.collection.name_or_id)
 
        manager = QueueManager.get_manager()
        handler = manager.get_handler(self.qid)
        if handler:
            n = Notification(u'delete')
            categories = member.atom.get_children('category', ATOM10_NS)
            for cat in categories:
                n.categories.append(Category(cat.get_attribute_value('term'),
                                             cat.get_attribute_value('scheme'),
                                             cat.get_attribute_value('label')))
                                             
            links = member.atom.get_children('link', ATOM10_NS)
            for link in links:
                n.links.append(Link(link.get_attribute_value('href'),
                                    link.get_attribute_value('rel'),
                                    link.get_attribute_value('type')))

            authors = member.atom.get_children('author', ATOM10_NS)
            for author in authors:
                name = author.get_child('name', ATOM10_NS)
                if name: name = name.xml_text
                else: name = None

                uri = author.get_child('uri', ATOM10_NS)
                if uri: uri = uri.xml_text
                else: uri = None

                email = author.get_child('email', ATOM10_NS)
                if email: email = email.xml_text
                else: email = None

                n.authors.append(Author(name, uri, email))
            
            m = PushMessage()
            m.request_id = str(time.time())
            m.qid = self.qid
            m.payload = b64encode(n.xml())
            handler.process(m)
