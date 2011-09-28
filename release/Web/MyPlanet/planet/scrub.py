"""
Process a set of configuration defined sanitations on a given feed.
"""

# Standard library modules
import time
# Planet modules
import planet, config, shell

type_map = {'text': 'text/plain', 'html': 'text/html',
    'xhtml': 'application/xhtml+xml'}

def scrub(feed_uri, data):

    # some data is not trustworthy
    for tag in config.ignore_in_feed(feed_uri).split():
        if tag.find('lang')>=0: tag='language'
        if data.feed.has_key(tag): del data.feed[tag]
        for entry in data.entries:
            if entry.has_key(tag): del entry[tag]
            if entry.has_key(tag + "_detail"): del entry[tag + "_detail"]
            if entry.has_key(tag + "_parsed"): del entry[tag + "_parsed"]
            for key in entry.keys():
                if not key.endswith('_detail'): continue
                for detail in entry[key].copy():
                    if detail == tag: del entry[key][detail]

    # adjust title types
    if config.title_type(feed_uri):
        title_type = config.title_type(feed_uri)
        title_type = type_map.get(title_type, title_type)
        for entry in data.entries:
            if entry.has_key('title_detail'):
                entry.title_detail['type'] = title_type

    # adjust summary types
    if config.summary_type(feed_uri):
        summary_type = config.summary_type(feed_uri)
        summary_type = type_map.get(summary_type, summary_type)
        for entry in data.entries:
            if entry.has_key('summary_detail'):
                entry.summary_detail['type'] = summary_type

    # adjust content types
    if config.content_type(feed_uri):
        content_type = config.content_type(feed_uri)
        content_type = type_map.get(content_type, content_type)
        for entry in data.entries:
            if entry.has_key('content'):
                entry.content[0]['type'] = content_type

    # some people put html in author names
    if config.name_type(feed_uri).find('html')>=0:
        from shell.tmpl import stripHtml
        if data.feed.has_key('author_detail') and \
            data.feed.author_detail.has_key('name'):
            data.feed.author_detail['name'] = \
                str(stripHtml(data.feed.author_detail.name))
        for entry in data.entries:
            if entry.has_key('author_detail') and \
                entry.author_detail.has_key('name'):
                entry.author_detail['name'] = \
                    str(stripHtml(entry.author_detail.name))
            if entry.has_key('source'):
                source = entry.source
                if source.has_key('author_detail') and \
                    source.author_detail.has_key('name'):
                    source.author_detail['name'] = \
                        str(stripHtml(source.author_detail.name))

    # handle dates in the future
    future_dates = config.future_dates(feed_uri).lower()
    if future_dates == 'ignore_date':
      now = time.gmtime()
      if data.feed.has_key('updated_parsed') and data.feed['updated_parsed']:
        if data.feed['updated_parsed'] > now: del data.feed['updated_parsed']
      for entry in data.entries:
        if entry.has_key('published_parsed') and entry['published_parsed']:
          if entry['published_parsed'] > now:
            del entry['published_parsed']
            del entry['published']
        if entry.has_key('updated_parsed') and entry['updated_parsed']:
          if entry['updated_parsed'] > now:
            del entry['updated_parsed']
            del entry['updated']
    elif future_dates == 'ignore_entry':
      now = time.time()
      if data.feed.has_key('updated_parsed') and data.feed['updated_parsed']:
        if data.feed['updated_parsed'] > now: del data.feed['updated_parsed']
      data.entries = [entry for entry in data.entries if 
        (not entry.has_key('published_parsed') or not entry['published_parsed']
          or entry['published_parsed'] <= now) and
        (not entry.has_key('updated_parsed') or not entry['updated_parsed']
          or entry['updated_parsed'] <= now)]
