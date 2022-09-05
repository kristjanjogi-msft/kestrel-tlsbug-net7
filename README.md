# kestrel-tlsbug-net7

1. Clone the repository:
`git clone https://github.com/kristjanjogi-msft/kestrel-tlsbug-net7.git`

1. `cd .\kestrel-tlsbug-net7`

1. Build the Docker image for the server
`docker build -t kestrelbug -f .\Server\Dockerfile .`

1. Run the server as a Docker container
`docker run -d --env ASPNETCORE_URLS="https://+" -p 9876:443 kestrelbug`

1. Run the client
`dotnet run --project .\Client\Client.csproj`

1. Check the server logs
`docker logs [container_id]`

1. Witness TLS handshake errors
```
dbug: Microsoft.AspNetCore.Server.Kestrel.Https.Internal.HttpsConnectionMiddleware[1]
      Failed to authenticate HTTPS connection.
      System.Security.Authentication.AuthenticationException: Authentication failed, see inner exception.
       ---> Interop+OpenSsl+SslException: SSL Handshake failed with OpenSSL error - SSL_ERROR_SSL.
       ---> Interop+Crypto+OpenSslCryptographicException: error:140D9115:SSL routines:ssl_get_prev_session:session id context uninitialized
         --- End of inner exception stack trace ---
         at Interop.OpenSsl.DoSslHandshake(SafeSslHandle context, ReadOnlySpan`1 input, Byte[]& sendBuf, Int32& sendCount)
         at System.Net.Security.SslStreamPal.HandshakeInternal(SafeFreeCredentials credential, SafeDeleteSslContext& context, ReadOnlySpan`1 inputBuffer, Byte[]& outputBuffer, SslAuthenticationOptions sslAuthenticationOptions, SelectClientCertificate clientCertificateSelectionCallback)
         --- End of inner exception stack trace ---
         at System.Net.Security.SslStream.ForceAuthenticationAsync[TIOAdapter](Boolean receiveFirst, Byte[] reAuthenticationData, CancellationToken cancellationToken)
         at Microsoft.AspNetCore.Server.Kestrel.Https.Internal.HttpsConnectionMiddleware.OnConnectionAsync(ConnectionContext context)
```
