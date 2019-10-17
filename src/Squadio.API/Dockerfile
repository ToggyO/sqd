FROM mcr.microsoft.com/dotnet/core/sdk:3.0
ARG source
WORKDIR /app
EXPOSE 80
#CMD echo ${source:-obj/Docker/publish}
#RUN echo ${source:-obj/Docker/publish}
#COPY ${source:-obj/Docker/publish} .
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "Squadio.API.dll"]
