# -*- coding: utf-8 -*-

import threading, time
import sha, os, os.path
from StringIO import StringIO
from xml.sax.saxutils import unescape, escape

from bridge import Element as E
from bridge import Document, PI
from amplee.atompub.member.atom import EntryResource
from amplee.atompub.member.generic import MediaResource
from amplee.utils import parse_multiform_data, safe_quote, \
     get_isodate, generate_uuid_uri, parse_query_string
from bridge.common import ATOM10_NS, ATOM10_PREFIX, THR_NS, THR_PREFIX

__all__ = ['GenericImageMember', 'GenericImageHandler']

###########################################################
# The following two classes handle the
# image media types
###########################################################
class GenericImageMember(MediaResource):
    def __init__(self, collection, **kwargs):
        MediaResource.__init__(self, collection, **kwargs)

    def generate_atom_id(self, entry=None, slug=None):
        if not slug:
            title = entry.get_child('title', entry.xml_ns)
            if title:
                slug = title.xml_text
            if not slug:
                slug = '%f' % time.time()
        return u'tag:%s:%s' % (self.collection.name_or_id,
                               sha.new(slug).hexdigest())

    def generate_resource_id(self, entry=None, slug=None):
        if slug:
            return slug.replace(' ','_').decode('utf-8')
        title = entry.get_child('title', entry.xml_ns)
        if title:
            return title.xml_text.replace(' ','_')

        return str(int(time.time()))

    def create(self, source, slug=None):
        source = MediaResource.create(self, source, slug)
        length = int(self.raw_headers['content-length'])
        return (source, length)

    def update(self, source, existing_member):
        source = MediaResource.update(self, source, existing_member)
        length = int(self.raw_headers['content-length'])
        return (source, length)
        
class GenericImageHandler(object):
    def __init__(self, member_type):
        self.member_type = member_type
        self.lock = threading.Lock()

    def on_update_feed(self, member):
        member.collection.feed_handler.set(member.collection.feed)
        
    def on_create(self, member, content):
        image, length = content
        
        image = image.read(length)
        if image:
            try:
                self.lock.acquire()
                file(os.path.join(self.member_type.params['photos_path'],
                                  member.media_id), 'wb').write(image)
            finally:
                self.lock.release()

        return member, image

    def on_update(self, existing_member, new_member, new_content):
        new_member.update_dates()
        
        image, length = new_content
        image = image.read(length)
        if image:
            try:
                self.lock.acquire()
                file(os.path.join(self.member_type.params['photos_path'],
                                  member.media_id), 'wb').write(image)
            finally:
                self.lock.release()

        return new_member, image

    def on_delete(self, member):
        try:
            self.lock.acquire()
            try:
                os.unlink(os.path.join(self.member_type.params['photos_path'],
                                       member.media_id))
            except OSError:
                pass
        finally:
            self.lock.release()
         
