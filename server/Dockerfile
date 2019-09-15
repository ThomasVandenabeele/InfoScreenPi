FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

RUN apt-get update
RUN apt-get install -y sqlite3 libsqlite3-dev

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
#RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .

#USER root
#RUN chmod 655 /app/InfoScreenDB.db
RUN apt-get update \
 && apt-get install -y --no-install-recommends ffmpeg libgdiplus libc6-dev openssh-server \
 && rm -rf /var/lib/apt/lists/*


ENTRYPOINT ["dotnet", "InfoScreenPi.dll"]
