# -*- coding: utf-8 -*-

__doc__ = """
This script will run a queue server interfacting with memecached
to persist any messages sent to the queue server.



The compulsory command line options are:

 * -s | --servers : Comma separated list of memcached instance addresses as IP:PORT.

 * -q | --queue : A string that identifies the queue server within the memcached instances. This need to be as unique and opaque as possible as it only has an internal significance.

 * -g | --id-generator : Address of the queue message identifier generators IP:PORT.



The optional command line option:

 * -l | --logger-path : Absolute path of the logger file
"""

def parse_commandline():
    from optparse import OptionParser
    parser = OptionParser()
    parser.add_option("-s", "--servers", dest="servers",
                      help="comma separated lists of IP:PORT of memcached servers")
    parser.add_option("-q", "--queue", dest="queue",
                      help="queue identifier")
    parser.add_option("-g", "--id-generator", dest="id_generator",
                      help="IP:PORT of the ID generator server address")
    parser.add_option("-l", "--logger-path", dest="logger_path",
                      help="absolute path to the log file")
    (options, args) = parser.parse_args()

    return options

def run():
    options = parse_commandline()

    from bucker.provider.memcached import MemcachedQueue
    servers = options.servers.split(',')
    ip, port = options.id_generator.split(':')
    queue = MemcachedQueue(servers=servers, queue_list_name=options.queue,
                           id_gen_address=(ip, int(port)))

    logger = None
    if options.logger_path:
        from bucker.lib.logger import Logger
        logger = Logger(options.logger_path, False)
        queue.set_logger(logger)

    MemcachedQueue.setService(queue)
    queue.activate()

    from bucker.provider.memcached import MemcachedQueueProtocol
    from Kamaelia.Chassis.ConnectedServer import SimpleServer

    def make_protocol():
        mqp = MemcachedQueueProtocol()
        if logger:
            mqp.set_logger(logger)
        return mqp

    s = SimpleServer(protocol=make_protocol, port=9876)
    s.run()

if __name__ == '__main__':
    run()
