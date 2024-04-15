# Stage 1: Build the application with dotnet sdk and create a dev certificate
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy as sdk
RUN mkdir /app
WORKDIR /app
COPY /src .
RUN dotnet dev-certs https --trust
ENTRYPOINT bash
RUN dotnet build -c Release -o build

# Stage 2: Build the runtime image with the application and the dev certificate
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy
ENV SERVER_PORT=8085
ENV DOTNET_RUNNING_IN_CONTAINER=true
RUN mkdir -p /app/wwwroot
VOLUME /app/wwwroot
COPY --from=sdk /root/.dotnet/corefx/cryptography/x509stores/my /root/.dotnet/corefx/cryptography/x509stores/my
COPY --from=sdk /app/build /app
WORKDIR /app
EXPOSE $SERVER_PORT
ENTRYPOINT dotnet StaticWebApp.dll --urls "https://*:$SERVER_PORT"