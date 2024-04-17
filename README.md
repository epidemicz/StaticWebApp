# Static Web App for Godot (games)

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/S6S3NKK32)  

![](/img/icon-sm.png)
## Overview

This is a simple web server pre-configured to host godot games exported to html.  

There are some pre-configured headers that are required for 
godot games, but this can also be used to quickly serve any static files from a directory.    

Can be ran as a docker image or as a stand-alone binary.  

## Usage
Below are some of the ways you can get started.

### Docker
- `8085` is the default container port.  
- `/app/wwwroot` is the volume that will be served.  

#### Run under a temporary docker container
Serve the contents of the current directory on port 8085:
```
docker run --rm -ti -p 8085:8085 -v .:/app/wwwroot epidemicz/swag
```

#### Pull the docker image from docker hub

```
docker pull epidemicz/swag:latest
```

#### Using docker compose
If you clone the repository, you can also use the `docker-compose` script to start a server.  See the `docker/.env` file to customize the `SERVER_PORT` and `SERVE_DIRECTORY`.
```
docker compose --project-directory docker up
```

### Running as a stand-alone binary
A few command line arguments are available:
|Argument|Example Usage|
|--|--|
|PATH|PATH=c:\dump (this needs to be an absolute path)
|SERVER_PORT|SERVER_PORT=8085|

If you clone the repository, you can run the app directly with `dotnet run`.  
This command will serve the directory `c:\dump` on port 9999:

```
dotnet run path=c:\dump port=9999
```

### As a stand-alone binary
Same as the above, just pass in the args.
```
swag.exe path=c:\dump
```

## Building the container
Building the container from the root directory:
```
docker build -f docker/Dockerfile -t epidemicz/swag:latest .
```
## Updating the container
Updating the container on docker hub:
```
docker push epidemicz/swag:latest
```
