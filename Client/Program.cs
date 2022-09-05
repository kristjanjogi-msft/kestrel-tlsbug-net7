using System;
using System.Net.Http;
using System.Threading;

var requestUri = "https://localhost:9876";

for (var i = 1; i <= 10; i++)
{
	var clientHandler = new HttpClientHandler
	{
		ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
	};

	var client = new HttpClient(clientHandler);

	var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

	try
	{
		var response = await client.SendAsync(request);

		Console.WriteLine($"[{i}] success: {response.StatusCode}");
	}
	catch (Exception ex)
	{
		Console.WriteLine($"[{i}] an error occurred: {ex}.");
	}

	Thread.Sleep(2000);
}
