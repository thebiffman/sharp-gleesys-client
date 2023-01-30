# sharp-glesys-client,
A C# Rest client for Gleesys API and a program to update A records

## Introduction

I am writing this console application and the client library only to fulfill my needs, which is to either update records on domains with my public IP or any IP found on any adapter that starts with a given string.

This is useful to keep your domain synced with your public IP if you don't have static IP where you host services, in your homelab for example.

The option to find an IP address based on a string is useful if you want to use your domain name internally in a network, where you want the application to find your current local network IP and update the domain with that.

You only need listrecords and updaterecord permission on the account used to access Glesys API at the moment. Which means that the application cannot create or delete records.

## Variables

You can run the program with a appsettings.json to set the variables or as a Docker container and supply the settings as environment variables.

| Variable | Required | Default value | Description
| --- | ----------- | --- | ---
| GLESYS_WEBSERVICE_URL | No | https://api.glesys.com | Webservice URL
| GLESYS_USE_PUBLIC_IP | No | true | Whether to use public IP or IP found by using GLESYS_IP_STARTS_WITH variable
| GLESYS_TTL | No | 300 | TTL to set on the records
| GLESYS_INTERVAL | No | 60 | Interval to run in minutes
| GLESYS_IP_STARTS_WITH | No | | String used to find IP on any adapter to use for the updates. Used together with USE_PUBLIC_IP = false
| GLESYS_USERNAME | Yes | | Username (starts with 'cl') for the API
| GLESYS_APIKEY | Yes | | API key for the account
| GLESYS_DOMAINS | Yes | | Domains and hosts that should be updated. Format is '\<domain1>#\<host1>,\<host2>\|\<domain2>#\<host1>,\<host2>'

## Docker container
There is a container that is built every time code is pushed and automatically published. The path to the image is ghcr.io/thebiffman/sharp-glesys-client:main. Below is a docker-compose template:

``` 
services:
  dns-updater:
    image: ghcr.io/thebiffman/sharp-glesys-client:main
    container_name: dns-updater
    environment: 
      - GLESYS_USERNAME=clxxxx
      - GLESYS_APIKEY=
      - GLESYS_DOMAINS=example1.com#@,www,*|example1.com#@,www,*
    restart: "unless-stopped"
```
