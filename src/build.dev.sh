#!/usr/bin/env bash

docker-compose -f docker-compose.build.yml up --build -d build

docker logs -f squadio-build

docker container stop squadio-dev
docker container rm squadio-dev
docker rmi squadio-dev

docker-compose -f docker-compose.dev.yml up -d --build

read -p "Press enter to continue"