
#!/bin/bash
while true; do
  chromium-browser --kiosk --disk-cache-size=1000000000 --force-device-scale-factor=1.00 --noerrdialogs --disable-session-crashed-bubble --app='http://193.190.58.21/screen'
done
