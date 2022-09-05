using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;

// Example to reproduce .NET 7.0.0-preview.7.22375.6 TLS handshake bug. Shall be run on Linux.

// Self-signed certificate. CN=kestrel-tlsbug.example.com
var serverCertificateAsBytes = Convert.FromBase64String("MIIKYAIBAzCCChwGCSqGSIb3DQEHAaCCCg0EggoJMIIKBTCCBhYGCSqGSIb3DQ" +
	"EHAaCCBgcEggYDMIIF/zCCBfsGCyqGSIb3DQEMCgECoIIE/jCCBPowHAYKKoZIhvcNAQwBAzAOBAiZqcHDZk3hdgICB9AEggTYcSteh/V0OE2ZSz10" +
	"UUqL3Hu92hDZpq3aygdTdeohFTg1xHMpQRq8y7Z+G/DAiaFoB3IgO29R+KombXasBp6kOB49b2EfkQ1Mz8UY3aipvU03dVnp+sEQqmg3Bdgafmwzqo" +
	"464CIDCQgxvahWib/T1E7MtqNsiyKjzfTlc6xT4MOKO3YoRf48jBDJdpiLSsQx5EaByda/XxemWM+ykFZMYzf8zULWfStBLe0l0UynWeV7b0hgtJ+S" +
	"TYI6cfG6OBWaln6/88dIHMQDxbpX7Aq+18Iz0VGKc2u7fuJQnwT6huIGK5xc2P9590A4TjvEZbAENnIdwhRAjgp02LMUGry+/Ujvx4rrsQ0KKShT0u" +
	"wdqL9s+sFdC3QU1lVHgcLR9tdnSKSW7t7lxom6qTTVXCMOJRcbo1Z0CWmJKg2brzxmqyXhpJ89tCUSu5YOrC2ZyFHoKDXGb79XKw/LoHuibymAqY9x" +
	"gEiUBLp5LiXbOH/wVZccI5Pk+NsHB9vGpIUDz9aT7im8vcW5IozBn5gxpxS1FPYHFx+DKyi/heJPGLJh+VJNauawZo3oagm0zlHR7qzgLtwjr5bXvx" +
	"Glva1xrK0IMlLFMVZJyEKFXfwviZbLoEwa24UJECn+s+SRkMQSm9qtMTUjRxDM8Rh/Odmz+yvE9jWZ0qnv+I1DJJocyh5Zq7BOO7wAkgqBaIXANxFC" +
	"StV6mKuMo3QXGu6PUZFwHNktNroPVTcusKbnS9JzfuyGy5vwsB1yKyRxAbIK943aO8Q+x1Yo8p98VZYz7QNujbqB2NUZpWiTNC9uKz6NwhTeSSotp7" +
	"4DJ0aGcMjQKkokpUuTDlQtSJ8tk79LwS/KagU+ZaHftnaL5L1Meox2GoZtyMUwGsdDcJ7BVq9/Mimhw9poaX54gXTOuaSu4tKbmDUMoJy4bUDhz8oR" +
	"ECUwPxzoxbE6NEHFP4Vi+Yq38DDOE0mhaDC9Jlq0U7BWBvFDO3HOH2fbVcZ6heLo3I9HH3JMF+GrERuQJNJrBa7RH7JPBwOERdJ/qsR5BG/xrgTYEJ" +
	"tPsIgQFZMgkSR3Yuh0nYCN0Wjsoq1Nn4bzfvSmP+Hc8MtUvJXD/C1uuO9+aOV7+JSNaplRSBOie7nG1/8ssLM06StmqJOFOB+Z2XVxiC2+3p/yTVZw" +
	"T7/m29YrZbr7GQWJzpHohZO7OEP6NT7lHeGIHDwF5Y97z+NJUo6n/rrZYnGep81j1OU66QqG7I1vIOOjZBW8OcQWrnRWg6G2Hi+FssUc71x7l2e17k" +
	"UwF99BP17CVOI2Hsfuf2OS72lRoVvyf82V2I0c2GxZZBxeb7mUBOFBOZv9/iuouDF27FiNUQxIN07ksnwlRIZ1hs5Le+bkGigeO6D7eZ/tkGKCt07c" +
	"Ox1TWp2TNp+pQVC6zvN5cwUaAU/NVOZZXsRbWcvdxRRz6Lzma1PY4iVjx3z6dp03+ic4IDzolAWp2pXXvlEftn82nFBRLdYFaKtD8pBkzkcJSqNkXy" +
	"Dd5vKbjAmsrDwqRlC21d7q2DhsV6/RyzpcMiSnL9NNThXehg7ybwOpFDdq5rmD7OhcwExNy0WZME6BeO155Qp03xkOTgckHXk90FsoXz50ewz/nfvZ" +
	"Ry0lWdPCbqgguzN3nKl2XQFRB9YlOPt9qlhP8NoV+TGB6TATBgkqhkiG9w0BCRUxBgQEAQAAADBXBgkqhkiG9w0BCRQxSh5IADIAOQBhADkAZABmAD" +
	"kAMwAtAGEAMAA2AGMALQA0AGIAMgBmAC0AYgAyADgAZgAtAGUAYgA2AGMANABmAGEANwAyADcAOAAwMHkGCSsGAQQBgjcRATFsHmoATQBpAGMAcgBv" +
	"AHMAbwBmAHQAIABFAG4AaABhAG4AYwBlAGQAIABSAFMAQQAgAGEAbgBkACAAQQBFAFMAIABDAHIAeQBwAHQAbwBnAHIAYQBwAGgAaQBjACAAUAByAG" +
	"8AdgBpAGQAZQByMIID5wYJKoZIhvcNAQcGoIID2DCCA9QCAQAwggPNBgkqhkiG9w0BBwEwHAYKKoZIhvcNAQwBAzAOBAgCK/Kp6vVmWQICB9CAggOg" +
	"ec2WbgYVUliuZlnkEvQg6b2fQvVhNEnNNn5nEP2NuKTcIaCUy2nU4ZcU7Doy8geX5YHLorViUmrzMwPNpRrGHtoVnRK1nM1/PKSy97NDJhHkEVRqQ9" +
	"5FLNvcyyutmCCKENIof9nytLkS464AmEz7NmqK3RFy/G1aa26biMqIMmEDeDlsIul/8rIClx+9X2rpisHwBOi1KmYNZFq6WdvTNdRhWnohr74ygbZN" +
	"OXKjAS4KAP7SHvb7ODBGdvl5gidhUM1MqxAg710A1WbHU6AuitJYVWsJdOxSOS/hdb8zgAwQHND6mxnIf7mj8JTj8n8g7m08y4xZNOiuZMCUpGnj+V" +
	"nUbrzYlPH8TnseqeehOhHadpirNCnH57vtLX9zKbbCQCRsAmfEVjfujQPCM2TCbuAtb7dggBCLIyM6lo+xVh+5j+ehvT/XO5f+W/0HmlfxmWFipKdX" +
	"ePXBRoqnnVd+xzUwlD35vmVfrrIIp4UtT+gprGGvMd+mP9/s8QnDp55YFSzpD74nR/T6Fwn5Ft/GYzqxJSkEZCgeU/b10W2q/0VO8ntBAVcBztUS5r" +
	"RBgBGBfTy1dFLboUjAu/RR4j6sSEGvc1P9PCDsBszfgQynakFkVKZBXUOpFrHtdlCnqXrn22jxlVT+Z533k/GuX/s3ppbtD9TXPd4TOwhI/SZpPjg8" +
	"u/1iMCyTymhXLyYRoRrx9yTMdzQhqWfKWHvvfkHtd8VE5P+ojOCBUWWcakfq+DGiMdVbvKgxx7HAsQsVEqWs+xXCX894b4CNGTOLgh3u84DJCZZjhL" +
	"HZWX6B2qP5LpBQprBppe3LYIWyZU5ZySM4V60eqRAVSha2AYvmZJYX/ntvwPWc8xdBQUysOvkcylbK4zMZ3uXm+ILWQ08Nus80tofrybvl4zx5FoqY" +
	"x+d+CtOWBdjXMdF0hgFsWo9TbVmliCk5hoOzkuwGjh3+cYPHKMKRy29UyJ+g/CDwEzdK1fzRRR1YOGgrRbe30FXRoM+IaSE5mR+l2lhljns2N9FBzA" +
	"+48DoBPfj6rY8V2lrFS7I+/lKLDE4vPeanMU2R4FbQPSpY9NwnJrHe6xHYBwtjgekzQiGrFNMofxEY5xng2V05/DjEevhUXijPvN75nigXBJzszk9V" +
	"xPlTpdFN9xWAvvkkXMLuPRvjtFAJkAq1UYO9zGYE6Zj8Q4mFvXigUD5VFz3Cyur0Ankvibyo4czDMXglMruSfWcLTdyMIi/fjTA7MB8wBwYFKw4DAh" +
	"oEFFRn04MKJdU9e4b7LkuiOB2F0KstBBRzccpGv8kCmi4TorIrmJ8pNhAvAwICB9A=");

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
	serverOptions.ConfigureHttpsDefaults(options =>
	{
		options.ServerCertificate = new X509Certificate2(serverCertificateAsBytes);

		// This line activates the TLS bug. However, the same does not occur on .NET 6.
		options.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
	});
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
