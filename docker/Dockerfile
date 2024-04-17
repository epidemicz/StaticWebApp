# Stage 1: Build the application with dotnet sdk and create a dev certificate
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy as build

# create an app directory for the build
RUN mkdir /app

# set the work directory and copy the source to it
WORKDIR /app
COPY /src .

# run the build
RUN dotnet build -c Release -o build

# create a dev certificate
RUN dotnet dev-certs https --trust

### ### ### ### ### ### ### ### ### ### ###

# Stage 2: Build the runtime image with the application and the dev certificate
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy

# environment variables
ENV SERVER_PORT=8085
ENV DOTNET_RUNNING_IN_CONTAINER=true

# create the wwwroot directory
RUN mkdir -p /app/wwwroot

# copy the dev certificate & the built application
COPY --from=build /root/.dotnet/corefx/cryptography/x509stores/my /root/.dotnet/corefx/cryptography/x509stores/my
COPY --from=build /app/build /app

# volumes and ports
VOLUME /app/wwwroot
EXPOSE $SERVER_PORT

# set work directory to /app and start the application
WORKDIR /app
ENTRYPOINT dotnet StaticWebApp.dll