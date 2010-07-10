# -*- coding: utf-8 -*-

def parse_commandline():
    from optparse import OptionParser
    parser = OptionParser()
    parser.add_option("-s", "--server", dest="server",
                      help="queue server address IP:PORT")
    parser.add_option("-q", "--queues", dest="queues",
                      help="queues to monitor")
    parser.add_option("-l", "--logger-path", dest="logger_path",
                      help="absolute path to the log file")
    (options, args) = parser.parse_args()

    return options

def run():
    options = parse_commandline()

    from bucker.lib.logger import Logger
    logger = None
    if options.logger_path:
        from bucker.lib.logger import Logger
        logger = Logger(options.logger_path, True)

    from Kamaelia.Chassis.ConnectedServer import SimpleServer

    from bucker.api.bus import ServiceBusServer, ServiceBusProtocol
    ip, port = options.server.split(':')
    queues = [unicode(q) for q in options.queues.split(',')]
    server = ServiceBusServer(queues=queues, host=ip, port=int(port),
                              manager_host='127.0.0.1', manager_port=9875)
    if logger:
        server.set_logger(logger)
    ServiceBusServer.setService(server)
    server.activate()

    s = SimpleServer(protocol=ServiceBusProtocol, port=9888)
    s.run()

if __name__ == '__main__':
    run()
