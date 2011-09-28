# -*- coding: utf-8 -*-

from llup.publisher.bucker_queue import run
from bucker.lib.logger import Logger

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

def serve():
    options = parse_commandline()
    servers = options.servers.split(',')

    logger = Logger(stdout=True)

    from llup.lib.option import Options
    opts = Options(servers=servers, queue_id=None,
                   idgen=('127.0.0.1', 9877), 
                   bus_port=9878, 
                   ip='127.0.0.1', port=9876, queues=[u'test'])
    run('memcached', opts)

if __name__ == '__main__':
    serve()
