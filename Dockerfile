FROM ubuntu:18.04

RUN apt-get update && apt-get install wget gnupg2 apt-utils -y

RUN wget -qO- http://packages.swarmops.com/swarmops-packages.gpg.key | apt-key add -
RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
RUN echo deb http://packages.swarmops.com/ bionic contrib > /etc/apt/sources.list.d/swarmops.list
RUN echo "deb https://download.mono-project.com/repo/ubuntu stable-bionic main" > /etc/apt/sources.list.d/mono-official-stable.list
RUN apt-get update

RUN apt-get install swarmops-backend -y

RUN echo "Yo"

CMD [ "/bin/bash" ]
