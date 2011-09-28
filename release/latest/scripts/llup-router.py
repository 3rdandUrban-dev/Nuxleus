# -*- coding: utf-8 -*-

import sys

from bucker.api.client import KQueueClient
from Kamaelia.Internet.TCPClient import TCPClient

from llup.filter.category import CategoryFilter
from llup.api.notification import Category
from llup.router.bucker_queue import RouterPeer, Router
from bucker.lib.logger import Logger

def run():
    logger = Logger(stdout=True)

    if len(sys.argv) == 1:
        rp = RouterPeer(('127.0.0.1', 9878), KQueueClient, 
                        [CategoryFilter([Category(term=u'indie')])], 
                        qid='test')
       
        rp.activate()
    
        r = Router(rp, 9879)
        r.run()
    else:
        listen_on_port = int(sys.argv[1])
        bind_to_port = int(sys.argv[2])
        term = unicode(sys.argv[3])
        rp = RouterPeer(('127.0.0.1', bind_to_port), TCPClient, 
                        [CategoryFilter([Category(term=term)])])
        rp.activate()
    
        r = Router(rp, listen_on_port)
        r.run()

if __name__ == '__main__':
    run()
