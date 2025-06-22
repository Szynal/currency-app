FROM busybox
COPY . /tmp/context
RUN find /tmp/context -maxdepth 2 -type f -or -type d