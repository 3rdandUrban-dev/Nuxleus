# -*- coding: utf-8 -*-

from Axon.Component import component
from Axon.Ipc import shutdownMicroprocess, producerFinished
from Kamaelia.Util.NullSink import nullSinkComponent
    
from bucker.api.client import KQueueClient
from bucker.api.message import ListQueues, GetMessage

class BaseService(component):
    Inboxes = {"inbox"    : "Message instance to be sent to the queue",
	       "control"  : "Shutdown the client stream", } 
     
    Outboxes = {"outbox"  : "Message instance parsed from the queue response",
		"signal"  : "Shutdown signal",
                "log"     : "", }
     
    def __init__(self, queues, host='127.0.0.1', port=9878): 
        super(BaseService, self).__init__()
        self.queues = queues
        self.host = host
        self.port = port
	self.logger = None

    def set_logger(self, logger):
        self.logger = logger

    def initialiseComponent(self):
        if self.logger:
           self.link((self, "log"), (self.logger, "inbox"))
	   self.addChildren(self.logger)
	   self.logger.activate()

        self.client = KQueueClient(self.host, self.port)
	self.addChildren(self.client)
	self.link((self.client, 'outbox'), (self, 'inbox'))
	self.link((self, 'outbox'), (self.client, 'inbox'))
	self.client.activate()

	return 1

    def main(self):
        yield self.initialiseComponent()

	lq = ListQueues()
	lq.queues.extend(self.queues)
	self.send(lq, 'outbox')

	while 1: 
           if self.dataReady("control"):
              mes = self.recv("control")

	      if isinstance(mes, shutdownMicroprocess) or \
		   isinstance(mes, producerFinished):
		 self.client.send(producerFinished(), "control")
		 break
	      yield 1

	   if self.dataReady("inbox"):
	      m = self.recv("inbox")
              if self.logger:
                 self.send(m.xml(), 'log')
	      if isinstance(m, GetMessage):
                 self.handle_new_message(m)
	      yield 1

	   if not self.anyReady():
	      self.pause()

	   yield 1

        yield 1

    def handle_new_message(self, m):
        pass

