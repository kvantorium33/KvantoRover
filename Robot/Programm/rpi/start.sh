#!/bin/bash

pidof uvc_stream >/dev/null
if [[ $? -ne 0 ]] ; then
  echo "Restarting uvc_stream: $(date)"
  /usr/local/bin/uvc_stream -p 8099 -d /dev/video0 -r 960x720 -f 30
else
  echo  "uvc_stream running"
fi
