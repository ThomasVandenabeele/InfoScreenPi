APP_NAME=InfoScreenPi

all: build create start

build:
	docker build .

create:
	docker create -v $(pwd)/InfoScreenDB.db:/app/InfoScreenDB.db -p 80:80 --name $(APP_NAME)

start:
	docker start
