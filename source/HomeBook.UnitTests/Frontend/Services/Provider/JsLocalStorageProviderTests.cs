using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Services.Provider;
using Microsoft.JSInterop;
using NSubstitute;

namespace HomeBook.UnitTests.Frontend.Services.Provider;

[TestFixture]
public class JsLocalStorageProviderTests
{
    private IJSRuntime _mockJsRuntime;
    private IJsLocalStorageProvider _localStorageProvider;

    [SetUp]
    public void SetUp()
    {
        _mockJsRuntime = Substitute.For<IJSRuntime>();
        _localStorageProvider = new JsLocalStorageProvider(_mockJsRuntime);
    }

    [TestFixture]
    public class SetItemAsyncTests : JsLocalStorageProviderTests
    {
        [Test]
        public async Task SetItemAsync_WithValidKeyAndValue_ShouldCallJSRuntime()
        {
            // Arrange
            const string key = "test-key";
            const string value = "test-value";

            // Act
            await _localStorageProvider.SetItemAsync(key, value);

            // Assert
            await _mockJsRuntime.Received(1).InvokeVoidAsync(
                "localStorage.setItem",
                Arg.Any<CancellationToken>(),
                key,
                value);
        }

        [Test]
        public async Task SetItemAsync_WithCancellationToken_ShouldPassTokenToJSRuntime()
        {
            // Arrange
            const string key = "test-key";
            const string value = "test-value";
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            // Act
            await _localStorageProvider.SetItemAsync(key, value, cancellationToken);

            // Assert
            await _mockJsRuntime.Received(1).InvokeVoidAsync(
                "localStorage.setItem",
                cancellationToken,
                key,
                value);
        }

        [Test]
        public void SetItemAsync_WithNullKey_ShouldThrowArgumentException()
        {
            // Act & Assert
            Should.ThrowAsync<ArgumentException>(() =>
                _localStorageProvider.SetItemAsync(null!, "value"));
        }

        [Test]
        public void SetItemAsync_WithEmptyKey_ShouldThrowArgumentException()
        {
            // Act & Assert
            Should.ThrowAsync<ArgumentException>(() =>
                _localStorageProvider.SetItemAsync(string.Empty, "value"));
        }

        [Test]
        public void SetItemAsync_WithWhitespaceKey_ShouldThrowArgumentException()
        {
            // Act & Assert
            Should.ThrowAsync<ArgumentException>(() =>
                _localStorageProvider.SetItemAsync("   ", "value"));
        }

        [Test]
        public void SetItemAsync_WithNullValue_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Should.ThrowAsync<ArgumentNullException>(() =>
                _localStorageProvider.SetItemAsync("key", null!));
        }

        [Test]
        public async Task SetItemAsync_WhenJSRuntimeThrows_ShouldPropagateException()
        {
            // Arrange
            const string key = "test-key";
            const string value = "test-value";
            JSException expectedException = new JSException("JS Error");

            _mockJsRuntime.When(x => x.InvokeVoidAsync(
                    "localStorage.setItem",
                    Arg.Any<CancellationToken>(),
                    key,
                    value))
                .Do(_ => throw expectedException);

            // Act & Assert
            JSException exception = await Should.ThrowAsync<JSException>(() =>
                _localStorageProvider.SetItemAsync(key, value));

            exception.Message.ShouldBe("JS Error");
        }
    }

    [TestFixture]
    public class GetItemAsyncTests : JsLocalStorageProviderTests
    {
        [Test]
        public async Task GetItemAsync_WithValidKey_ShouldReturnValue()
        {
            // Arrange
            const string key = "test-key";
            const string expectedValue = "test-value";

            _mockJsRuntime.InvokeAsync<string?>(
                    "localStorage.getItem",
                    Arg.Any<CancellationToken>(),
                    key)
                .Returns(expectedValue);

            // Act
            string? result = await _localStorageProvider.GetItemAsync(key);

            // Assert
            result.ShouldBe(expectedValue);
        }

        [Test]
        public async Task GetItemAsync_WithNonExistentKey_ShouldReturnNull()
        {
            // Arrange
            const string key = "non-existent-key";

            _mockJsRuntime.InvokeAsync<string?>(
                    "localStorage.getItem",
                    Arg.Any<CancellationToken>(),
                    key)
                .Returns((string?)null);

            // Act
            string? result = await _localStorageProvider.GetItemAsync(key);

            // Assert
            result.ShouldBeNull();
        }

        [Test]
        public async Task GetItemAsync_WithCancellationToken_ShouldPassTokenToJSRuntime()
        {
            // Arrange
            const string key = "test-key";
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            // Act
            await _localStorageProvider.GetItemAsync(key, cancellationToken);

            // Assert
            await _mockJsRuntime.Received(1).InvokeAsync<string?>(
                "localStorage.getItem",
                cancellationToken,
                key);
        }

        [Test]
        public void GetItemAsync_WithNullKey_ShouldThrowArgumentException()
        {
            // Act & Assert
            Should.ThrowAsync<ArgumentException>(() =>
                _localStorageProvider.GetItemAsync(null!));
        }

        [Test]
        public void GetItemAsync_WithEmptyKey_ShouldThrowArgumentException()
        {
            // Act & Assert
            Should.ThrowAsync<ArgumentException>(() =>
                _localStorageProvider.GetItemAsync(string.Empty));
        }

        [Test]
        public void GetItemAsync_WithWhitespaceKey_ShouldThrowArgumentException()
        {
            // Act & Assert
            Should.ThrowAsync<ArgumentException>(() =>
                _localStorageProvider.GetItemAsync("   "));
        }
    }

    [TestFixture]
    public class RemoveItemAsyncTests : JsLocalStorageProviderTests
    {
        [Test]
        public async Task RemoveItemAsync_WithValidKey_ShouldCallJSRuntime()
        {
            // Arrange
            const string key = "test-key";

            // Act
            await _localStorageProvider.RemoveItemAsync(key);

            // Assert
            await _mockJsRuntime.Received(1).InvokeVoidAsync(
                "localStorage.removeItem",
                Arg.Any<CancellationToken>(),
                key);
        }

        [Test]
        public async Task RemoveItemAsync_WithCancellationToken_ShouldPassTokenToJSRuntime()
        {
            // Arrange
            const string key = "test-key";
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            // Act
            await _localStorageProvider.RemoveItemAsync(key, cancellationToken);

            // Assert
            await _mockJsRuntime.Received(1).InvokeVoidAsync(
                "localStorage.removeItem",
                cancellationToken,
                key);
        }

        [Test]
        public void RemoveItemAsync_WithNullKey_ShouldThrowArgumentException()
        {
            // Act & Assert
            Should.ThrowAsync<ArgumentException>(() =>
                _localStorageProvider.RemoveItemAsync(null!));
        }

        [Test]
        public void RemoveItemAsync_WithEmptyKey_ShouldThrowArgumentException()
        {
            // Act & Assert
            Should.ThrowAsync<ArgumentException>(() =>
                _localStorageProvider.RemoveItemAsync(string.Empty));
        }

        [Test]
        public void RemoveItemAsync_WithWhitespaceKey_ShouldThrowArgumentException()
        {
            // Act & Assert
            Should.ThrowAsync<ArgumentException>(() =>
                _localStorageProvider.RemoveItemAsync("   "));
        }
    }

    [TestFixture]
    public class ClearAsyncTests : JsLocalStorageProviderTests
    {
        [Test]
        public async Task ClearAsync_ShouldCallJSRuntime()
        {
            // Act
            await _localStorageProvider.ClearAsync();

            // Assert
            await _mockJsRuntime.Received(1).InvokeVoidAsync(
                "localStorage.clear",
                Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task ClearAsync_WithCancellationToken_ShouldPassTokenToJSRuntime()
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            // Act
            await _localStorageProvider.ClearAsync(cancellationToken);

            // Assert
            await _mockJsRuntime.Received(1).InvokeVoidAsync(
                "localStorage.clear",
                cancellationToken);
        }
    }

    [TestFixture]
    public class GetLengthAsyncTests : JsLocalStorageProviderTests
    {
        [Test]
        public async Task GetLengthAsync_ShouldReturnLength()
        {
            // Arrange
            const int expectedLength = 5;

            _mockJsRuntime.InvokeAsync<int>(
                    "localStorage.length",
                    Arg.Any<CancellationToken>())
                .Returns(expectedLength);

            // Act
            int result = await _localStorageProvider.GetLengthAsync();

            // Assert
            result.ShouldBe(expectedLength);
        }

        [Test]
        public async Task GetLengthAsync_WithCancellationToken_ShouldPassTokenToJSRuntime()
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            // Act
            await _localStorageProvider.GetLengthAsync(cancellationToken);

            // Assert
            await _mockJsRuntime.Received(1).InvokeAsync<int>(
                "localStorage.length",
                cancellationToken);
        }
    }

    [TestFixture]
    public class GetKeyAsyncTests : JsLocalStorageProviderTests
    {
        [Test]
        public async Task GetKeyAsync_WithValidIndex_ShouldReturnKey()
        {
            // Arrange
            const int index = 0;
            const string expectedKey = "test-key";

            _mockJsRuntime.InvokeAsync<string?>(
                    "localStorage.key",
                    Arg.Any<CancellationToken>(),
                    index)
                .Returns(expectedKey);

            // Act
            string? result = await _localStorageProvider.GetKeyAsync(index);

            // Assert
            result.ShouldBe(expectedKey);
        }

        [Test]
        public async Task GetKeyAsync_WithInvalidIndex_ShouldReturnNull()
        {
            // Arrange
            const int index = 999;

            _mockJsRuntime.InvokeAsync<string?>(
                    "localStorage.key",
                    Arg.Any<CancellationToken>(),
                    index)
                .Returns((string?)null);

            // Act
            string? result = await _localStorageProvider.GetKeyAsync(index);

            // Assert
            result.ShouldBeNull();
        }

        [Test]
        public async Task GetKeyAsync_WithCancellationToken_ShouldPassTokenToJSRuntime()
        {
            // Arrange
            const int index = 0;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            // Act
            await _localStorageProvider.GetKeyAsync(index, cancellationToken);

            // Assert
            await _mockJsRuntime.Received(1).InvokeAsync<string?>(
                "localStorage.key",
                cancellationToken,
                index);
        }

        [Test]
        public void GetKeyAsync_WithNegativeIndex_ShouldThrowArgumentOutOfRangeException()
        {
            // Act & Assert
            Should.ThrowAsync<ArgumentOutOfRangeException>(() =>
                _localStorageProvider.GetKeyAsync(-1));
        }
    }

    [TestFixture]
    public class ContainsKeyAsyncTests : JsLocalStorageProviderTests
    {
        [Test]
        public async Task ContainsKeyAsync_WithExistingKey_ShouldReturnTrue()
        {
            // Arrange
            const string key = "existing-key";
            const string value = "some-value";

            _mockJsRuntime.InvokeAsync<string?>(
                    "localStorage.getItem",
                    Arg.Any<CancellationToken>(),
                    key)
                .Returns(value);

            // Act
            bool result = await _localStorageProvider.ContainsKeyAsync(key);

            // Assert
            result.ShouldBeTrue();
        }

        [Test]
        public async Task ContainsKeyAsync_WithNonExistingKey_ShouldReturnFalse()
        {
            // Arrange
            const string key = "non-existing-key";

            _mockJsRuntime.InvokeAsync<string?>(
                    "localStorage.getItem",
                    Arg.Any<CancellationToken>(),
                    key)
                .Returns((string?)null);

            // Act
            bool result = await _localStorageProvider.ContainsKeyAsync(key);

            // Assert
            result.ShouldBeFalse();
        }

        [Test]
        public async Task ContainsKeyAsync_WithEmptyStringValue_ShouldReturnTrue()
        {
            // Arrange
            const string key = "empty-value-key";

            _mockJsRuntime.InvokeAsync<string?>(
                    "localStorage.getItem",
                    Arg.Any<CancellationToken>(),
                    key)
                .Returns(string.Empty);

            // Act
            bool result = await _localStorageProvider.ContainsKeyAsync(key);

            // Assert
            result.ShouldBeTrue();
        }

        [Test]
        public async Task ContainsKeyAsync_WithCancellationToken_ShouldPassTokenCorrectly()
        {
            // Arrange
            const string key = "test-key";
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            _mockJsRuntime.InvokeAsync<string?>(
                    "localStorage.getItem",
                    cancellationToken,
                    key)
                .Returns("value");

            // Act
            await _localStorageProvider.ContainsKeyAsync(key, cancellationToken);

            // Assert
            await _mockJsRuntime.Received(1).InvokeAsync<string?>(
                "localStorage.getItem",
                cancellationToken,
                key);
        }

        [Test]
        public void ContainsKeyAsync_WithNullKey_ShouldThrowArgumentException()
        {
            // Act & Assert
            Should.ThrowAsync<ArgumentException>(() =>
                _localStorageProvider.ContainsKeyAsync(null!));
        }

        [Test]
        public void ContainsKeyAsync_WithEmptyKey_ShouldThrowArgumentException()
        {
            // Act & Assert
            Should.ThrowAsync<ArgumentException>(() =>
                _localStorageProvider.ContainsKeyAsync(string.Empty));
        }

        [Test]
        public void ContainsKeyAsync_WithWhitespaceKey_ShouldThrowArgumentException()
        {
            // Act & Assert
            Should.ThrowAsync<ArgumentException>(() =>
                _localStorageProvider.ContainsKeyAsync("   "));
        }
    }

    [TestFixture]
    public class IntegrationTests : JsLocalStorageProviderTests
    {
        [Test]
        public async Task LocalStorageWorkflow_SetGetRemove_ShouldWorkCorrectly()
        {
            // Arrange
            const string key = "workflow-key";
            const string value = "workflow-value";

            _mockJsRuntime.InvokeAsync<string?>(
                    "localStorage.getItem",
                    Arg.Any<CancellationToken>(),
                    key)
                .Returns(value, (string?)null);

            // Act & Assert - Set item
            await _localStorageProvider.SetItemAsync(key, value);
            await _mockJsRuntime.Received(1).InvokeVoidAsync(
                "localStorage.setItem",
                Arg.Any<CancellationToken>(),
                key,
                value);

            // Act & Assert - Get item
            string? retrievedValue = await _localStorageProvider.GetItemAsync(key);
            retrievedValue.ShouldBe(value);

            // Act & Assert - Check contains
            bool containsKey = await _localStorageProvider.ContainsKeyAsync(key);
            containsKey.ShouldBeTrue();

            // Act & Assert - Remove item
            await _localStorageProvider.RemoveItemAsync(key);
            await _mockJsRuntime.Received(1).InvokeVoidAsync(
                "localStorage.removeItem",
                Arg.Any<CancellationToken>(),
                key);

            // Act & Assert - Check after removal
            bool containsAfterRemoval = await _localStorageProvider.ContainsKeyAsync(key);
            containsAfterRemoval.ShouldBeFalse();
        }
    }
}
