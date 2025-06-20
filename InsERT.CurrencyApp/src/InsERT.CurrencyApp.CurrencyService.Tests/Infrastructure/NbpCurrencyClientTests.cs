using System.Net;
using System.Text;
using System.Text.Json;
using InsERT.CurrencyApp.Abstractions.Serialization;
using InsERT.CurrencyApp.CurrencyService.Configuration;
using InsERT.CurrencyApp.CurrencyService.Infrastructure.Nbp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace InsERT.CurrencyApp.CurrencyService.Tests.Infrastructure;

public class NbpCurrencyClientTests
{
    [Fact]
    public async Task GetLatestTableAsync_ShouldReturnData_WhenValidJson()
    {
        // Arrange
        var expected = new List<NbpTable>
        {
            new()
            {
                Table = "B",
                EffectiveDate = DateTime.Today.ToString("yyyy-MM-dd"),
                Rates =
                [
                    new() { Code = "EUR", Currency = "Euro", Mid = 4.23m }
                ]
            }
        };

        var json = JsonSerializer.Serialize(expected, JsonSerializationProfiles.CaseInsensitive);

        var messageHandlerMock = new Mock<HttpMessageHandler>();
        messageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

        var httpClient = new HttpClient(messageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.nbp.pl/api")
        };

        var logger = Mock.Of<ILogger<NbpCurrencyClient>>();

        var options = Options.Create(new NbpClientSettings
        {
            BaseUrl = "https://api.nbp.pl/api",
            TableBEndpoint = "/exchangerates/tables/B?format=json",
            RetryCount = 3,
            BackoffSeconds = 2
        });

        var client = new NbpCurrencyClient(httpClient, logger, options);

        // Act
        var result = await client.FetchLatestRatesTableAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result.Rates, r => r.Code == "EUR" && r.Mid == 4.23m);
    }
}
