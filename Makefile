APP_NAME=InfoScreenPi
IMAGE_NAME=screenpi

.PHONY: help

## USE MAKE WITH SUDO

help: root ## This help.
	@awk 'BEGIN {FS = ":.*?## "} /^[a-zA-Z_-]+:.*?## / {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}' $(MAKEFILE_LIST)

.DEFAULT_GOAL := help

run: root build create start  ## Build create and start the container.

clean-run: root remove build create start  ## Build create and start the container after stopping and removing previous container.


build: root ## Build the container and delete intermediate untagged images.
	@docker build --rm -t $(IMAGE_NAME) .
	@docker system prune -f

create: root ## Creates a writeable container from the image and prepares it for running.
	#@docker create -v $(shell pwd)/InfoScreenDB.db:/app/InfoScreenDB.db -p 80:80 --name $(APP_NAME) $(IMAGE_NAME):latest
	@docker create -v $(shell pwd)/db:/app/db -p 80:80 --name $(APP_NAME) $(IMAGE_NAME):latest

start: root ## Runs the container.
	@echo "Starting docker container:"
	@docker start $(APP_NAME)

stop: root ## Stops the container.
	@echo "Stopping docker container:"
	@docker stop $(APP_NAME)

remove: root halt ## Stop and remove a running container.
	@echo "Removing docker container:"
	@docker rm $(APP_NAME)

perm: root ## Fix permissions of SQLite DB.
	@chmod 664 InfoScreenDB.db

delete-rommel: root ## Delete untagged docker images and all docker containers.
	@docker system prune

root:
	@if ! [ "$(shell id -u)" = 0 ] ; then echo "You are not root, run this target as root please"; exit 1; fi
