#! /bin/bash

base="/usr/local/infoscreen"

#Install packages
sudo apt-get -y update
sudo apt-get -y upgrade
sudo apt-get -y dist-upgrade
sudo apt-get -y install chromium-browser lightdm 

sed -i "s/#disable_overscan=1/disable_overscan=1/" /boot/config.txt

#Install scripts
mkdir $base

cat << EOF >> $base/manage_memory.sh
if df -h | grep '/dev/shm' | grep -o '[8-9][0-9]%' > /dev/null; then
  killall chromium-browse
  sudo service lightdm restart
fi
EOF

chmod +x $base/manage_memory.sh


#Autostart chromium
cat << EOF >> /home/pi/.Xsession
xset s off
xset -dpms
xset s noblank
sed -i 's/"exited_cleanly": false/"exited_cleanly": true/' ~/.config/chromium-browser Default/Preferences
chromium-browser --noerrdialogs --kiosk http://193.190.58.21/screen  --disable-translate --window-size=1920,1080 --window-position=0,0
EOF

#Cron jobs
crontab -l -u pi > $base/cron.txt
cat << EOF >> $base/cron.txt
0 7 * * 1-6 sudo service lightdm start 
0 23 * * 1-6 killall chromium-browse; sudo service lightdm stop
* * * * * $base/manage_memory.sh
EOF

crontab -u pi $base/cron.txt
rm $base/cron.txt


#Autologin
sed -i "s/# autologin-user = .*$/autologin-user=pi/" /etc/lightdm/lightdm.conf
sed -i "s/exit 0/sudo systemctl start lightdm\nexit 0/" /etc/rc.local

#desktop
#sed -i 's/width=[0-9]*/width=0/; s/height=[0-9]*/height=0/' /home/pi/.config/lxpanel/LXDE-pi/panels/panel

#disable lock screen and boot stuff
sed -i 's/quiet splash plymouth.ignore-serial-consoles/consoleblank=0 loglevel=1 logo.nologo quiet plymouth.ignore-serial-consoles/' /boot/cmdline.txt

#Disable cursor
sed -i 's/#xserver-command=X/xserver-command=X -nocursor/' /etc/lightdm/lightdm.conf

#Increase swap
swapoff -a
dd if=/dev/zero of=/swapfile bs=1M count=2048 status=progress
chmod 0600 /swapfile
mkswap /swapfile
swapon /swapfile
sed -i 's/100/2048/' /etc/dphys-swapfile



#Increase /dev/shm
echo 'tmpfs  /dev/shm  tmpfs  defaults,size=2g  0  0' >> /etc/fstab

#Increase video memory
echo 'gpu_mem=256' >> /boot/config.txt

reboot
