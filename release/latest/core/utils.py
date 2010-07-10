# -*- coding: utf-8 -*-

import os, os.path
from StringIO import StringIO
import markdown
from Ft.Xml.Xslt import Transform
from xml.sax.saxutils import unescape
from bridge import Element as E
from bridge import Attribute as A
from bridge.common import ATOM10_PREFIX, ATOM10_NS, XML_NS, XML_PREFIX
from bridge.filter.atom import lookup_links
from amplee.utils import get_isodate

__all__ = ['transform_member_resource', 'render',
           'update_home_page', 'update_collection_page',
           'update_entry_page', 'atom_to_rss', 'render_search_result',
           'generate_event_form_page', 'update_category_list']

base_dir = os.getcwd()

def transform_member_resource(member, xslt_path=None):
    """Transforms the member entry content from Markdown to HTML
    then returns a public version of the member entry.

    This entry can be used for the public feed aggregation for instance.

    You may pass the URI or path of an XSLT resource to be inserted as
    a processing instruction.
    """
    content = member.atom.get_child('content', member.atom.xml_ns)
    
    # Nex twe transform the Markdown content into XHTML
    recipe_html = u''
    if content:
        recipe_html = markdown.Markdown(content.xml_text).toString()
        recipe_html = recipe_html.strip()

    recipe_html = unescape(recipe_html)
        
    # Finally we create a public atom entry from the member resource entry
    public = member.prepare_for_public(content=recipe_html, media_type=u'application/xml',
                                       xslt_path=xslt_path)

    #point = member.atom.get_child('point', 'http://www.georss.org/georss')
    #if point:
    #   E(u'point', content=point.xml_text,
    #     prefix=u'georss', namespace=u'http://www.georss.org/georss', parent=public.xml_root)

    # We indicate where the comments to the recipe will be found
    # by using RFC 4685
    attributes = {u'rel': u'replies', u'type': u'application/atom+xml',
                  u'href': u'%s/comments' % member.public_uri}
    E(u'link', attributes=attributes, prefix=public.xml_root.xml_prefix,
      namespace=public.xml_root.xml_ns, parent=public.xml_root)

    return public    
 
def render(e, xslt_path, params=None):
    params = params or {}
    return Transform(e.xml(), xslt_path, params=params)

def generate_default_resource_not_found(service):
    page = render(service, os.path.join(base_dir, 'static', 'xslt', 'layout.xsl'))
    page = page % u"This resource could not be found."
    file(os.path.join(base_dir, 'static', 'notfound.html'), 'w').write(page)

def update_home_page(service):
    page = render(service, os.path.join(base_dir, 'static', 'xslt', 'layout.xsl'))
    content = markdown.Markdown(file(os.path.join(base_dir, 'static', "homepage.txt")).read()).toString()
    page = page % content.strip()
    file(os.path.join(base_dir, 'static', 'homepage.html'), 'w').write(page)

def render_search_result(service, feed, params=None):
    page = render(service, os.path.join(base_dir, 'static', 'xslt', 'layout.xsl'), params=params)
    page = page % render(feed, os.path.join(base_dir, 'static', 'xslt', 'feed.xsl'), params=params)
    return page
    
def update_collection_page(service, feed, name):
    link = feed.xml_root.filtrate(lookup_links, rel='self')[0].get_attribute_value('href', u'')
    params = {u'atom_feed_uri': '%satom' % link,
              u'rss_feed_uri': '%srss' % link}
    page = render(service, os.path.join(base_dir, 'static', 'xslt', 'layout.xsl'), params)
    page = page % render(feed, os.path.join(base_dir, 'static', 'xslt', 'feed.xsl'))
    file(os.path.join(base_dir, 'static', name), 'w').write(page)

def update_entry_page(service, entry, path):
    page = render(service, os.path.join(base_dir, 'static', 'xslt', 'layout.xsl'))
    page = page % render(entry, os.path.join(base_dir, 'static', 'xslt', 'entry.xsl'))
    file(path, 'w').write(page)

def generate_event_form_page(service):
    page = render(service, os.path.join(base_dir, 'static', 'xslt', 'layout.xsl'))
    page = page % render(E('dummy'), os.path.join(base_dir, 'static', 'xslt', 'eventform.xsl'))
    file(os.path.join(base_dir, 'static', 'eventform.html'), 'w').write(page)

def atom_to_rss(feed, name):
    """Transforms an Atom feed into a RSS equivalent
    and saves it into the static directory under the given name"""
    feed = feed.xml_root
    rss = E(u'rss', attributes={u'version': u'2.0'})
    base = feed.get_attribute_value('base')
    if base:
        A(u'base', value=base, prefix=XML_PREFIX, namespace=XML_NS)
    channel = E(u'channel', parent=rss)
    
    id = feed.get_child('id', feed.xml_ns)
    E(u'guid', content=id.xml_text, parent=channel)

    title = feed.get_child('title', feed.xml_ns)
    E(u'title', content=title.xml_text, parent=channel)
    E(u'description', content=title.xml_text, parent=channel)

    link = feed.filtrate(lookup_links, rel='self')
    E(u'link', content=link[0].get_attribute_value('href', u''), parent=channel)

    entries = feed.get_children('entry', feed.xml_ns)
    for entry in entries[:10]:
        item = E(u'item', parent=channel)
        # We use the link[rel=@self]/@href instead of atom:id here
        # as it's possible that some clients expect a deferencable URI here
        link = entry.filtrate(lookup_links, rel='self')
        E(u'guid', content=base+link[0].get_attribute_value('href', u''), parent=item)

        title = entry.get_child('title', entry.xml_ns)
        E(u'title', content=title.xml_text, parent=item)
        
        author = entry.get_child('author', entry.xml_ns).get_child('name', entry.xml_ns)
        E(u'author', content=author.xml_text, parent=item)

        link = entry.filtrate(lookup_links, rel='alternate')
        E(u'link', content=base+link[0].get_attribute_value('href', u''), parent=item)

        content = entry.get_child('content', entry.xml_ns)
        if content:
            E(u'description', content=content.collapse(), parent=item)
        
        categories = entry.get_children('category', entry.xml_ns)
        for cat in categories:
            attr = {}
            scheme = cat.get_attribute_value('scheme')
            if scheme:
                attr[u'domain'] = scheme
            E(u'category', attributes=attr,
              content=cat.get_attribute_value('term', u''), parent=item)
    
    file(os.path.join(base_dir, 'static', '%s.rss' % name), 'w').write(rss.xml())

_seen_categories = []

def update_category_list(entry):
    catlist = E.load(file(os.path.join('static', 'catlist.atom'), 'r')).xml_root
    categories = entry.xml_root.get_children('category', ATOM10_NS)
    modified = False
    for category in categories:
        token = category.get_attribute_value('term', '') + category.get_attribute_value('scheme', '')
        if token not in _seen_categories:
           modified = True
           _seen_categories.append(token)
           attrs = {}
           term = category.get_attribute_value('term', None)
           if term: attrs[u'term'] = term

           scheme = category.get_attribute_value('scheme', None)
           if scheme: attrs[u'scheme'] = scheme

           label = category.get_attribute_value('label', None)
           if label: attrs[u'label'] = label

           E(u'category', attributes=attrs, namespace=ATOM10_NS, prefix=ATOM10_PREFIX, parent=catlist)

    if modified:
       updated = catlist.get_child('updated', ATOM10_NS)
       updated.xml_text = get_isodate()
       file(os.path.join('static', 'catlist.atom'), 'w').write(catlist.xml())

