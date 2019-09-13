#Infoscreen application for Raspberry Pi

Contains server-side .NET Core web application and install script to configure clients.

*Only works properly on RPI 3 and above*

##TODO
Currently the application uses chromium to dislay everything. However, there is a memory leak in that causes the application to fill the entire /dev/shm partition of the Pi until it crashes. This is especially a problem when there are many videos to be played. Currently this is resolved by running a cron job every minute that checks is /dev/shm is almost fully used. If so, chromium is killed and the window manager is restarted. A better solution is needed.

Note that this bug does not affect Chromium on Ubuntu Mate. However, Ubuntu Mate uses the OpenGL video drivers by default which have poor video performance on the Pi.

Best solution would be to integrate OMXplayer in the browser. Plugins for this exist.
