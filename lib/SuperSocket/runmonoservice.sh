#!/bin/sh
mono-service -l:/tmp/supersocket.lock -m:supersocket --debug SuperSocket.SocketService.exe
