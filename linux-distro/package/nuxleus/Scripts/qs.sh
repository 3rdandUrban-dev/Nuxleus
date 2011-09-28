#!/bin/sh
#
# qs - Manage a bucker queue server 
#
PID_FILE=/tmp/qs.pid
PORT=9876
MEMCACHED_SERVERS=127.0.0.1:11211

function check_status  {
    if [ -f $PID_FILE ]; then
        PID=`cat $PID_FILE`
        result=`ps -p $PID|grep $PID|grep -v grep`
	case $result in
		"") echo "queue server is not running" ;;
		*) echo "queue server is running with PID: $PID" ;;
	esac
    else
	echo "queue server is not running"
    fi
}

function kill_server {
    if [ -f $PID_FILE ]; then
        PID=`cat $PID_FILE`
        result=`ps -p $PID|grep $PID|grep -v grep`
	case $result in
		*) 
		`kill -USR1 $PID`
		`kill -15 $PID`
		`kill -9 $PID`
		;;
	esac
    fi
    
}

case $1 in

start)
  
  if [ $2 ]; then
      MEMCACHED_SERVERS = $2
  fi
  
  if [ $3 ]; then
      PORT = $3
  fi
  
  echo "Starting the queue server on port $PORT against memcached servers at $MEMCACHED_SERVERS"
  /usr/local/bin/queue-server.py -s $MEMCACHED_SERVERS -p $PORT -i $PID_FILE &
;;

stop)
  echo "Stopping the queue server."
  kill_server
;;

status)
	check_status
;;

*)
  echo "usage: $0 start|stop|status";
;;

esac

# end of qs.sh
