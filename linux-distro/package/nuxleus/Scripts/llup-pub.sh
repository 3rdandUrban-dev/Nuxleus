#!/bin/sh
#
# qs - Manage a bucker queue server 
#
PID_FILE=/tmp/lluppub.pid
PORT=9879
QUEUES=
QUEUE_SERVER=127.0.0.1:9876

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
  
  echo "Starting the llup publisher server on port $PORT against queue server at $QUEUE_SERVER and monitoring $2"
  # add -v to the line below to enable logging to the console
   llup-queue-publisher.py -s $QUEUE_SERVER -q $2 -i $PID_FILE -f 5.0 &
;;

stop)
  echo "Stopping the llup publisher server."
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
