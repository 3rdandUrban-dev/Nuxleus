# -*- coding: utf-8 -*-

from base64 import b64decode

import httplib2
import amara
from bridge.common import DC_NS, DC_PREFIX

from hachoir_core.error import error, HachoirError
from hachoir_parser import guessParser, createParser
from hachoir_metadata import extractMetadata
from hachoir_core.stream import StringInputStream
from hachoir_parser.audio.id3 import ID3v1

from base import BaseService

def qname(local_name, prefix=None):
    if prefix:
        return "%s:%s" % (prefix, local_name)

    return local_name

def extract_metadata(audio):
    parser = guessParser(StringInputStream(audio))
    
    if not parser:
        raise ValueError("Could not parse the stream")

    return extractMetadata(parser)
    
def extract_metadata_from_file(filename):
    parser = createParser(filename)
    
    if not parser:
        raise ValueError("Could not parse %s" % filename)

    return extractMetadata(parser)

class MetadataExtractor(BaseService):
    def __init__(self, queues):
        super(MetadataExtractor, self).__init__(queues=queues)
        self.h = httplib2.Http('.cache')

    def handle_new_message(self, m):
        blip = amara.parse(b64decode(m.payload))
        blip.xmlns_prefixes['atom'] = u'http://www.w3.org/2005/Atom'
        url = blip.xml_xpath('/llup:notification/atom:link[@type="audio/mpeg"]')

        if not url:
            return

        url = url[0].href
        r, c = self.h.request(url)
        metadata = extract_metadata(c)

        doc = amara.parse('./entry-sample.atom')
        entry = doc.entry

        author = metadata.getItem('author', 0)
        if author:
            entry.xml_append(doc.xml_create_element(qname(u'creator', DC_PREFIX),
                                                  ns=DC_NS, content=author.value))
        creation_date = metadata.getItem('creation_date', 0)
        if creation_date:
            cd = unicode(creation_date.value.strftime('%Y'))
            entry.xml_append(doc.xml_create_element(qname(u'issued', DC_PREFIX),
                                                  ns=DC_NS, content=cd))
            
        mime_type = metadata.getItem('mime_type', 0)
        if mime_type:
            entry.xml_append(doc.xml_create_element(qname(u'format', DC_PREFIX),
                                                    ns=DC_NS, content=mime_type.value))

        duration = metadata.getItem('duration', 0)
        if duration:
            entry.xml_append(doc.xml_create_element(qname(u'format.length', DC_PREFIX),
                                                    ns=DC_NS, content=unicode(duration.value)))

        rights = metadata.getItem('copyright', 0)
        if rights:
            entry.xml_append(doc.xml_create_element(qname(u'rights', DC_PREFIX),
                                                  ns=DC_NS, content=rights.value))

        genre = metadata.getItem('music_genre', 0)
        if genre:
            genre = ID3v1.genre_name[int(genre.value[1:-1])]
            entry.xml_append(doc.xml_create_element(qname(u'description', DC_PREFIX),
                                                  ns=DC_NS, content=genre))

        print entry.xml(force_nsdecls={DC_PREFIX: DC_NS}, indent=True)

if __name__ == '__main__':
    def run():
        me = MetadataExtractor(['test'])
        from bucker.lib.logger import Logger
        me.set_logger(Logger('./metadata.log'))
        me.run()

    run()
