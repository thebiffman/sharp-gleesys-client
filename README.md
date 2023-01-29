# sharp-glesys-client
### A C# Rest client for Gleesys API

I am writing this console application and the client library only to fulfill my needs, which is to either update records on domains with my public IP or any IP found on any adapter that starts with a given string.

This is useful to keep your domain synced with your public domain if you don't have static IP where you host services, in your homelab for example.

THe option to find an IP address based on a string is useful if you want to use your domain name internally in a network, where you want the application to find your current local network IP and update the domain with that.

You only need listrecords and updaterecord permission on the account used to access Glesys API at the moment. Which means that the application cannot create or delete records currently.
