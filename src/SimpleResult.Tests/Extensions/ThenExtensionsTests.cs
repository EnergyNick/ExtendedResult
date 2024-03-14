﻿using SimpleResult.Core;
using SimpleResult.Extensions;
using SimpleResult.TestCommon.Helpers;

namespace SimpleResult.Tests.Extensions;

public class ThenExtensionsTests
{
    [Fact]
    public void ThenExtension_WhenInvokeOnSuccessResultAndFunc_ShouldBeInvokedAndReturnTypedResult()
    {
        // Arrange
        var result = Result.Ok();

        var value = "Hello there!";
        var action = () => value;

        // Act
        var thenResult = result.Then(action);

        // Assert
        thenResult.ShouldBeSuccessAndReferenceEqualsValue(value);
    }

    [Fact]
    public void ThenExtension_WhenInvokeOnFailedResultAndFunc_ShouldNotBeInvoked()
    {
        // Arrange
        var error = new Error("Deadlock");
        var result = Result.Fail(error);

        var value = "Hello there!";
        var action = () => value;

        // Act
        var thenResult = result.Then(action);

        // Assert
        thenResult.ShouldBeFailed(error);
    }

    [Fact]
    public void ThenExtension_WhenInvokeOnSuccessResultWithFuncReturningResult_ShouldBeInvoked()
    {
        // Arrange
        var result = Result.Ok();

        var internalError = new Error("Situation is terrible!");
        var internalSuccessResult = Result.Ok();
        var internalFailedResult = Result.Fail(internalError);

        var successAction = () => internalSuccessResult;
        var failedAction = () => internalFailedResult;

        // Act
        var thenSuccessResult = result.Then(successAction);
        var thenFailedResult = result.Then(failedAction);

        // Assert
        thenSuccessResult.Should().Be(internalSuccessResult);
        thenFailedResult.Should().Be(internalFailedResult);
    }

    [Fact]
    public void ThenExtension_WhenInvokeOnFailedResultWithFuncReturningResult_ShouldNotBeInvoked()
    {
        // Arrange
        var error = new Error("Bad");
        var result = Result.Fail(error);

        var internalError = new Error("Situation is terrible!");
        var internalSuccessResult = Result.Ok();
        var internalFailedResult = Result.Fail(internalError);

        var successAction = () => internalSuccessResult;
        var failedAction = () => internalFailedResult;

        // Act
        var thenSuccessResult = result.Then(successAction);
        var thenFailedResult = result.Then(failedAction);

        // Assert
        thenSuccessResult.Should().Be(result);
        thenFailedResult.Should().Be(result);
    }

    [Fact]
    public void ThenExtension_WhenInvokeOnSuccessResultAndFuncWithResultReturn_ShouldBeInvokedAndReturnTypedResult()
    {
        // Arrange
        var result = Result.Ok();

        var value = "Hello there!";
        var error = new Error("Not functional?!");

        var expectedSuccessResult = Result.Ok(value);
        var expectedFailedResult = Result.Fail<string>(error);
        var successAction = () => expectedSuccessResult;
        var failedAction = () => expectedFailedResult;

        // Act
        var thenResultSuccess = result.Then(successAction);
        var thenResultFailed = result.Then(failedAction);

        // Assert
        thenResultSuccess.ShouldBeSuccessAndReferenceEqualsValue(value);
        thenResultSuccess.Should().Be(expectedSuccessResult);

        thenResultFailed.ShouldBeFailed(error);
        thenResultFailed.Should().Be(expectedFailedResult);
    }

    [Fact]
    public void ThenExtension_WhenInvokeOnFailedResultAndFuncWithResultReturn_ShouldNotBeInvoked()
    {
        // Arrange
        var error = new Error("Deadlock");
        var result = Result.Fail(error);

        var value = "Hello there!";
        var internalError = new Error("Not functional?!");

        var expectedSuccessResult = Result.Ok(value);
        var expectedFailedResult = Result.Fail(internalError);
        var successAction = () => expectedSuccessResult;
        var failedAction = () => expectedFailedResult;

        // Act
        var thenResultSuccess = result.Then(successAction);
        var thenResultFailed = result.Then(failedAction);

        // Assert
        thenResultSuccess.ShouldBeFailed(error);
        thenResultFailed.ShouldBeFailed(error);
    }

    [Fact]
    public void ThenExtension_WhenInvokeOnSuccessTypedResultAndFunc_ShouldBeInvokedAndReturnTypedResult()
    {
        // Arrange
        var value = "Good time need good result";
        var result = Result.Ok(value);

        var internalValue = "Hello there!";

        string? expectedValues = null;
        var action = (string x) =>
        {
            expectedValues = x;
            return internalValue;
        };

        // Act
        var thenResult = result.Then(action);

        // Assert
        thenResult.ShouldBeSuccessAndEqualsValue(internalValue);
        expectedValues.Should().Be(value);
    }

    [Fact]
    public void ThenExtension_WhenInvokeOnFailedTypedResultAndFunc_ShouldNotBeInvoked()
    {
        // Arrange
        var error = new Error("Deadlocker");
        var result = Result.Fail<string>(error);

        var internalValue = "Hello there!";

        string? expectedValues = null;
        var action = (string x) =>
        {
            expectedValues = x;
            return internalValue;
        };

        // Act
        var thenResult = result.Then(action);

        // Assert
        thenResult.ShouldBeFailed(error);
        expectedValues.Should().BeNull();

    }

    [Fact]
    public void ThenExtension_WhenInvokeOnSuccessTypedResultAndFuncWithResultReturn_ShouldBeInvokedAndReturnResult()
    {
        // Arrange
        var value = "Good time need good result";
        var result = Result.Ok(value);

        var error = new Error("Not functional?!");

        var expectedSuccessResult = Result.Ok();
        var expectedFailedResult = Result.Fail(error);

        var expectedValues = new List<string>();
        var successAction = (string x) =>
        {
            expectedValues.Add(x);
            return expectedSuccessResult;
        };
        var failedAction = (string x) =>
        {
            expectedValues.Add(x);
            return expectedFailedResult;
        };

        // Act
        var thenResultSuccess = result.Then(successAction);
        var thenResultFailed = result.Then(failedAction);

        // Assert
        thenResultSuccess.ShouldBeSuccess();
        thenResultSuccess.Should().Be(expectedSuccessResult);

        thenResultFailed.ShouldBeFailed(error);
        thenResultFailed.Should().Be(expectedFailedResult);

        expectedValues.Should().OnlyContain(x => ReferenceEquals(x, value));
    }

    [Fact]
    public void ThenExtension_WhenInvokeOnFailedTypedResultAndFuncWithResultReturn_ShouldNotBeInvokedAndReturnResult()
    {
        // Arrange
        var error = new Error("Deadlock");
        var result = Result.Fail<string>(error);

        var internalError = new Error("Not functional?!");

        var expectedSuccessResult = Result.Ok();
        var expectedFailedResult = Result.Fail(internalError);

        var expectedValues = new List<string>();
        var successAction = (string x) =>
        {
            expectedValues.Add(x);
            return expectedSuccessResult;
        };
        var failedAction = (string x) =>
        {
            expectedValues.Add(x);
            return expectedFailedResult;
        };

        // Act
        var thenResultSuccess = result.Then(successAction);
        var thenResultFailed = result.Then(failedAction);

        // Assert
        thenResultSuccess.ShouldBeFailed(error);
        thenResultFailed.ShouldBeFailed(error);

        expectedValues.Should().BeEmpty();
    }

    [Fact]
    public void ThenExtension_WhenInvokeOnSuccessTypedResultAndFuncWithResultReturn_ShouldBeInvokedAndReturnTypedResult()
    {
        // Arrange
        var value = "Good time need good result";
        var result = Result.Ok(value);

        var internalValue = "Hello there!";
        var error = new Error("Not functional?!");

        var expectedSuccessResult = Result.Ok(internalValue);
        var expectedFailedResult = Result.Fail<string>(error);

        var expectedValues = new List<string>();
        var successAction = (string x) =>
        {
            expectedValues.Add(x);
            return expectedSuccessResult;
        };
        var failedAction = (string x) =>
        {
            expectedValues.Add(x);
            return expectedFailedResult;
        };

        // Act
        var thenResultSuccess = result.Then(successAction);
        var thenResultFailed = result.Then(failedAction);

        // Assert
        thenResultSuccess.ShouldBeSuccessAndReferenceEqualsValue(internalValue);
        thenResultSuccess.Should().Be(expectedSuccessResult);

        thenResultFailed.ShouldBeFailed(error);
        thenResultFailed.Should().Be(expectedFailedResult);

        expectedValues.Should().OnlyContain(x => ReferenceEquals(x, value));
    }

    [Fact]
    public void ThenExtension_WhenInvokeOnFailedTypedResultAndFuncWithResultReturn_ShouldNotBeInvoked()
    {
        // Arrange
        var error = new Error("Deadlock");
        var result = Result.Fail<string>(error);

        var internalValue = "Hello there!";
        var internalError = new Error("Not functional?!");

        var expectedSuccessResult = Result.Ok(internalValue);
        var expectedFailedResult = Result.Fail<string>(internalError);

        var expectedValues = new List<string>();
        var successAction = (string x) =>
        {
            expectedValues.Add(x);
            return expectedSuccessResult;
        };
        var failedAction = (string x) =>
        {
            expectedValues.Add(x);
            return expectedFailedResult;
        };

        // Act
        var thenResultSuccess = result.Then(successAction);
        var thenResultFailed = result.Then(failedAction);

        // Assert
        thenResultSuccess.ShouldBeFailed(error);
        thenResultFailed.ShouldBeFailed(error);

        expectedValues.Should().BeEmpty();
    }

    [Fact]
    public void ThenOnFailExtension_WhenInvokeOnSuccessResultWithFuncFactory_ShouldNotBeInvoked()
    {
        // Arrange
        var value = "Good time need good result";
        var result = Result.Ok(value);

        var internalValue = "Hello there!";
        var isInvoked = false;
        var action = () =>
        {
            isInvoked = true;
            return internalValue;
        };

        var internalResult = Result.Ok(internalValue);
        var isInvokedResult = false;
        var actionResult = () =>
        {
            isInvokedResult = true;
            return internalResult;
        };

        // Act
        var firstResult = result.ThenOnFail(action);
        var secondResult = result.ThenOnFail(actionResult);

        // Assert
        firstResult.Should().Be(result);
        secondResult.Should().Be(result);
        isInvoked.Should().BeFalse();
        isInvokedResult.Should().BeFalse();
    }

    [Fact]
    public void ThenOnFailExtension_WhenInvokeOnFailedResultWithFuncFactory_ShouldBeInvokedAndReturnNewResult()
    {
        // Arrange
        var error = new Error("Deadlocker");
        var result = Result.Fail<string>(error);

        var internalValue = "Hello there!";
        var isInvoked = false;
        var action = () =>
        {
            isInvoked = true;
            return internalValue;
        };

        var internalResult = Result.Ok(internalValue);
        var isInvokedResult = false;
        var actionResult = () =>
        {
            isInvokedResult = true;
            return internalResult;
        };

        // Act
        var firstResult = result.ThenOnFail(action);
        var secondResult = result.ThenOnFail(actionResult);

        // Assert
        firstResult.Should().NotBe(result);
        secondResult.Should().NotBe(result);
        isInvoked.Should().BeTrue();
        isInvokedResult.Should().BeTrue();

        firstResult.ShouldBeSuccessAndReferenceEqualsValue(internalValue);
        secondResult.ShouldBeSuccessAndReferenceEqualsValue(internalValue);
    }

    [Fact]
    public void ThenOnFailExtension_WhenInvokeOnSuccessResultWithFuncByErrors_ShouldNotBeInvoked()
    {
        // Arrange
        var value = "Good time need good result";
        var result = Result.Ok(value);

        var internalValue = "Hello there!";
        var isInvoked = false;
        var action = (IReadOnlyCollection<IError> errors) =>
        {
            isInvoked = true;
            return internalValue;
        };

        var internalResult = Result.Ok(internalValue);
        var isInvokedResult = false;
        var actionResult = (IReadOnlyCollection<IError> errors) =>
        {
            isInvokedResult = true;
            return internalResult;
        };

        // Act
        var firstResult = result.ThenOnFail(action);
        var secondResult = result.ThenOnFail(actionResult);

        // Assert
        firstResult.Should().Be(result);
        secondResult.Should().Be(result);
        isInvoked.Should().BeFalse();
        isInvokedResult.Should().BeFalse();
    }

    [Fact]
    public void ThenOnFailExtension_WhenInvokeOnFailedResultWithFuncByErrors_ShouldBeInvokedAndReturnNewResult()
    {
        // Arrange
        var error = new Error("Deadlocker");
        var result = Result.Fail<string>(error);

        var internalValue = "Hello there!";
        var isInvoked = false;
        var action = (IReadOnlyCollection<IError> errors) =>
        {
            isInvoked = true;
            return internalValue;
        };

        var internalResult = Result.Ok(internalValue);
        var isInvokedResult = false;
        var actionResult = (IReadOnlyCollection<IError> errors) =>
        {
            isInvokedResult = true;
            return internalResult;
        };

        // Act
        var firstResult = result.ThenOnFail(action);
        var secondResult = result.ThenOnFail(actionResult);

        // Assert
        firstResult.Should().NotBe(result);
        secondResult.Should().NotBe(result);
        isInvoked.Should().BeTrue();
        isInvokedResult.Should().BeTrue();

        firstResult.ShouldBeSuccessAndReferenceEqualsValue(internalValue);
        secondResult.ShouldBeSuccessAndReferenceEqualsValue(internalValue);
    }
}