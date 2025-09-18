using System.Net;
using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Services.Exceptions;
using HomeBook.Frontend.Services.Provider;
using Moq;
using Moq.Protected;

namespace HomeBook.UnitTests.Frontend.Services.Provider;

[TestFixture]
public class ContentProviderTests
{
    private Mock<HttpMessageHandler> _mockMessageHandler;
    private HttpClient _httpClient;
    private IContentProvider _contentProvider;

    [SetUp]
    public void SetUp()
    {
        _mockMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockMessageHandler.Object)
        {
            BaseAddress = new Uri("https://test.example.com/")
        };
        _contentProvider = new ContentProvider(_httpClient);
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
    }

    [Test]
    public async Task GetContentAsync_WhenFileExists_ShouldReturnContent()
    {
        // Arrange
        const string fileName = "test-file.txt";
        const string expectedContent = "This is test content";

        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent)
        };

        _mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        string result = await _contentProvider.GetContentAsync(fileName);

        // Assert
        result.ShouldBe(expectedContent);
    }

    [Test]
    public async Task GetContentAsync_WhenFileIsEmpty_ShouldReturnEmptyString()
    {
        // Arrange
        const string fileName = "empty-file.txt";
        const string expectedContent = "";

        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent)
        };

        _mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        string result = await _contentProvider.GetContentAsync(fileName);

        // Assert
        result.ShouldBe(expectedContent);
        result.ShouldBeEmpty();
    }

    [Test]
    public async Task GetContentAsync_WhenFileNotFound_ShouldThrowHttpNotFoundException()
    {
        // Arrange
        const string fileName = "non-existent-file.txt";
        HttpRequestException httpException = new HttpRequestException("Not Found", null, HttpStatusCode.NotFound);

        _mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(httpException);

        // Act & Assert
        HttpNotFoundException exception = await Should.ThrowAsync<HttpNotFoundException>(
            () => _contentProvider.GetContentAsync(fileName));

        exception.Url.ShouldBe(fileName);
        exception.Message.ShouldBe("response is not found.");
        exception.InnerException.ShouldBe(httpException);
    }

    [Test]
    public async Task GetContentAsync_WhenHttpRequestExceptionWithOtherStatusCode_ShouldRethrowOriginalException()
    {
        // Arrange
        const string fileName = "server-error-file.txt";
        HttpRequestException httpException = new HttpRequestException("Internal Server Error", null, HttpStatusCode.InternalServerError);

        _mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(httpException);

        // Act & Assert
        HttpRequestException exception = await Should.ThrowAsync<HttpRequestException>(
            () => _contentProvider.GetContentAsync(fileName));

        exception.ShouldBe(httpException);
    }

    [Test]
    public async Task GetContentAsync_WhenHttpRequestExceptionWithoutStatusCode_ShouldRethrowOriginalException()
    {
        // Arrange
        const string fileName = "network-error-file.txt";
        HttpRequestException httpException = new HttpRequestException("Network error");

        _mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(httpException);

        // Act & Assert
        HttpRequestException exception = await Should.ThrowAsync<HttpRequestException>(
            () => _contentProvider.GetContentAsync(fileName));

        exception.ShouldBe(httpException);
    }

    [Test]
    public async Task GetContentAsync_WhenOtherExceptionOccurs_ShouldRethrowOriginalException()
    {
        // Arrange
        const string fileName = "exception-file.txt";
        InvalidOperationException originalException = new InvalidOperationException("Something went wrong");

        _mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(originalException);

        // Act & Assert
        InvalidOperationException exception = await Should.ThrowAsync<InvalidOperationException>(
            () => _contentProvider.GetContentAsync(fileName));

        exception.ShouldBe(originalException);
    }

    [Test]
    public async Task GetContentAsync_WithLongContent_ShouldReturnFullContent()
    {
        // Arrange
        const string fileName = "large-file.txt";
        string expectedContent = new string('X', 10000); // 10KB of X characters

        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent)
        };

        _mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        string result = await _contentProvider.GetContentAsync(fileName);

        // Assert
        result.ShouldBe(expectedContent);
        result.Length.ShouldBe(10000);
    }
}
