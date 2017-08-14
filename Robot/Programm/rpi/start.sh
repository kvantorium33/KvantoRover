#!/bin/bash

HOSTNAME=$hostname
PORT=8099
DATA=""
RETRY=15

echo $HOSTNAME

while [ $RETRY -gt 0 ]
do
    DATA=$(wget -O - -q -t 1 http://$HOSTNAME:$PORT)
    if [ $? -eq 0 ]
    then
        break
    else
        echo "Retrying Again" >&2
        uvc_stream -p $PORT -d /dev/video0 -r 960x720 -f 30 &
        let RETRY-=1
        sleep 30
    fi
done

echo "Server is UP"
