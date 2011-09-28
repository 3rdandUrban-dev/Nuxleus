# -*- coding: utf-8 -*-

# Central point for a blog to processes queue messages

import time
import socket
import threading
from Queue import Queue, Empty
from bucker.api.client import QueueClient
from bucker.api.message import NewQueue

__all__ = ['manager', 'QueueManager', 'QueueHandler']

class QueueManager(object):
    _manager = None
    
    def __init__(self):
        """
        Manages the pool of queue handlers.
        """
        self.handlers = {}

    def get_handler(self, qid):
        if qid in self.handlers:
            return self.handlers[qid]

    def get_manager():
        # some kind of singleton...
        if not QueueManager._manager:
            QueueManager._manager = QueueManager()
        return QueueManager._manager
    get_manager = staticmethod(get_manager)
        
class QueueHandler(threading.Thread):
    def __init__(self, qid, host, port):
        """
        A QueueHandler is just a central point for the blog to handle
        messages pushed to a given queue_id.

        This avoids duplication of code and make the blog more robust.

        The ``qid`` is a queue identifier that will created (if not yet
        in the queue server) when the register() method is called.

        The ``host`` and ``port`` are the address of the queue server
        to connect to.

        Each handler runs in its own thread.

        So for instance:

        q = QueueHandler('event', '127.0.0.1', 9876)
        q.register() #initializes and connects to the queue server
        q.start() # start the thread

        q.process(message)

        ...

        q.unregister()
        q.join()
        
        """
        threading.Thread.__init__(self)
        self.qid = qid
        self.client = QueueClient(host, port)
        self.running = False
        self._messages = Queue()

    def register(self):
        self.running = True
        self.client.connect()

        m = NewQueue()
        m.qid = self.qid.encode('utf-8')
        self.process(m)

    def unregister(self):
        self.running = False
        self.process(None)
        self.client.disconnect()

    def process(self, message):
        """
        Ask the handler to process the message.
        The message is poyt into a thread-safe container and
        processed by the queue handler as soon as it is possible.

        Therefore the caller doesn't get blocked on this call.
        """
        self._messages.put(message)
        
    def run(self):
        """
        This method runs into the thread until the handler is unregistered.
        """
        while self.running:
            m = None
            
            try:
                m = self._messages.get()
            except Empty:
                continue

            if m != None:
                self.client.send(m)
                try:
                    # we are not interested in the response but we free
                    # the associated resources on the queue then
                    self.client.recv()
                except socket.timeout:
                    pass
