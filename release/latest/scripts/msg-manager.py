# -*- coding: utf-8 -*-

def parse_commandline():
    from optparse import OptionParser
    parser = OptionParser()
    parser.add_option("-s", "--server", dest="server",
                      help="queue server address IP:PORT")
    parser.add_option("-l", "--logger-path", dest="logger_path",
                      help="absolute path to the log file")
    (options, args) = parser.parse_args()

    return options

def run():
    options = parse_commandline()
    from Kamaelia.Chassis.ConnectedServer import SimpleServer
    
    from bucker.lib.logger import Logger
    logger = None
    if options.logger_path:
        from bucker.lib.logger import Logger
        logger = Logger(options.logger_path, True)

    from bucker.api.bus import MessageManagerServer, MessageManagerProtocol
    ip, port = options.server.split(':')
    mgr = MessageManagerServer(qs_host=ip, qs_port=int(port))
    if logger:
        mgr.set_logger(logger)
    MessageManagerServer.setService(mgr)
    mgr.activate()

    m = SimpleServer(protocol=MessageManagerProtocol, port=9875)
    m.run()

if __name__ == '__main__':
    run()
