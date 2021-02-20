#!/usr/bin/env bash

export DB_HOST='localhost';
export DB_PORT='Port=5432';
export DB_NAME='squadio-1';
export DB_USER='postgres';
export DB_PASSWORD='postgres';
export ASPNETCORE_ENVIRONMENT='Development';

cd ./Squadio.DAL/
dotnet ef --startup-project ../Squadio.API/ migrations add --context SquadioDbContext AddedBaseModel
cd ..