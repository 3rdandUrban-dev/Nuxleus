# -*- coding: utf-8 -*-

def run():
    from bucker.lib.midgen import MessageIdService, MessageIdProtocol
    service, shutdownservice, serv = MessageIdService.getService()
    serv.activate()
    
    def make_protocol():
        return MessageIdProtocol()

    from Kamaelia.Chassis.ConnectedServer import SimpleServer
    s = SimpleServer(protocol=make_protocol, port=9877)
    s.run()

if __name__ == '__main__':
    run()
