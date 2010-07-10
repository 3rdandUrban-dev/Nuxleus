# -*- coding: utf-8 -*-

import threading, time
import sha, os, os.path

from bridge import Element as E
from bridge import Document, PI
from amplee.atompub.member.atom import EntryResource
from amplee.utils import get_isodate, generate_uuid_uri
from bridge.common import ATOM10_NS, ATOM10_PREFIX, THR_NS, THR_PREFIX

from core.utils import transform_member_resource, render, \
     update_collection_page, update_entry_page, atom_to_rss

__all__ = ['AtomEntryMember', 'AtomEntryHandler']

###########################################################
# The following two classes handle the
# application/atom+xml;type=entry media-type
###########################################################
class AtomEntryMember(EntryResource):
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
        
class AtomEntryHandler(object):
    def __init__(self, member_type):
        # Instance of amplee.handler.MemberType
        self.member_type = member_type
        self.lock = threading.Lock()

    def on_update_feed(self, member):
        """Called anytime the member is created, edited or deleted,
        so that you can decide whether or not the collection feed of
        must also be updated.

        Here we always update it:
        * member.collection.feed will force the reconstruction of the
          feed.
        * member.collection.feed_handler.set() will reset the feed
        to its new state.

        Since those two operations can be costly you may decide to avoid
        them here and delay when the collection feed is updated by another
        mean.
        """
        member.collection.feed_handler.set(member.collection.feed)
        
    def on_create(self, member, content):
        """
        Called by the HTTP handler to complete the creation of the
        resource. The ``member`` parameter is an instance of
        RecipeEntryMember and the ``content`` parameter is the POSTed
        entry as a byte string.

        This returns the member itself (that you could therefore alter
        here if needed) and None as no content should be stored in
        addition to the member resource.
        """
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
                try:
                    self.lock.acquire()
                    update_entry_page(servdoc, public, path)
                    update_collection_page(servdoc,
                                           member.collection.feed_handler.public_feed,
                                           member.collection.name_or_id)
                    atom_to_rss(member.collection.feed_handler.public_feed,
                                member.collection.name_or_id)
                finally:
                    self.lock.release()
                

    def on_update(self, existing_member, new_member, new_content):
        """
        On an update/edit operation, we receive three arguments.
        The ``existing_parameter`` is the member as it currently
        exists in the store. The ``new_member`` is the member as it was
        generated from the nwly sent content but not yet persisted
        into the store. The ``new_content`` is the content sent with
        the request in case your handler needs to access it.

        It will simply replace the existing member by the new one
        without much more processing. However in a more advanced
        application, one could perform some diff between the two
        and decide whether or not it can realize the operation.
        """
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
                
                try:
                    self.lock.acquire()
                    update_entry_page(servdoc, public, path)
                    update_collection_page(servdoc,
                                           member.collection.feed_handler.public_feed,
                                           member.collection.name_or_id)
                    atom_to_rss(member.collection.feed_handler.public_feed,
                                member.collection.name_or_id)
                finally:
                    self.lock.release()
        else:
            member.collection.feed_handler.remove(entry)
            try:
                self.lock.acquire()
                try:
                    os.unlink(path)
                except OSError:
                    pass
                init_collection_page(member.collection.workspace.service.service,
                                     member.collection.feed_handler.public_feed,
                                     member.collection.name_or_id)
                atom_to_rss(member.collection.feed_handler.public_feed,
                            member.collection.name_or_id)
            finally:
                self.lock.release()
        

    def on_deleted(self, member):
        """
        On a delete operatuion we simply remove the atom entry
        from the collection feed.
        """
        member.collection.feed_handler.remove(member.atom)
        try:
            self.lock.acquire()
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
        finally:
            self.lock.release()
 
