using System.Security.Claims;
using Atya.Governance.Testing.Assertions;
using Atya.Governance.Testing.Builders;
using Atya.Governance.Testing.Diagnostics;
using Atya.Governance.Testing.Json;
using Atya.Governance.Testing.Security;
using Atya.Governance.Testing.Time;
using Xunit;

namespace Atya.Governance.Testing.UnitTests;

public sealed class FakeClockTests
{
    [Fact]
    public void ConstructorNormalizesInitialValueToUtc()
    {
        FakeClock clock = new(new DateTimeOffset(2026, 4, 18, 16, 30, 0, TimeSpan.FromHours(4)));

        Assert.Equal(TimeSpan.Zero, clock.UtcNow.Offset);
        Assert.Equal(new DateTimeOffset(2026, 4, 18, 12, 30, 0, TimeSpan.Zero), clock.UtcNow);
    }

    [Fact]
    public void AdvanceAndRewindRejectNegativeAmounts()
    {
        FakeClock clock = new();

        Assert.Throws<ArgumentOutOfRangeException>(() => clock.Advance(TimeSpan.FromTicks(-1)));
        Assert.Throws<ArgumentOutOfRangeException>(() => clock.Rewind(TimeSpan.FromTicks(-1)));
    }
}

public sealed class JsonAssertTests
{
    [Fact]
    public void EqualIgnoresObjectPropertyOrderAndFormatting()
    {
        const string Expected = """{"id":1,"name":"Ada","roles":["admin","writer"]}""";
        const string Actual = """
            {
                "roles": [
                    "admin",
                    "writer"
                ],
                "name": "Ada",
                "id": 1
            }
            """;

        JsonAssert.Equal(Expected, Actual);
    }

    [Fact]
    public void EqualRejectsInvalidJsonWithParameterName()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => JsonAssert.Equal("{", "{}"));

        Assert.Equal("expectedJson", exception.ParamName);
    }
}

public sealed class FakeCorrelationIdAccessorTests
{
    [Fact]
    public void ClearRemovesCorrelationId()
    {
        FakeCorrelationIdAccessor accessor = new("correlation-1");

        accessor.Clear();

        Assert.False(accessor.HasCorrelationId);
        Assert.Null(accessor.CorrelationId);
    }

    [Fact]
    public void SetRejectsBlankCorrelationId()
    {
        FakeCorrelationIdAccessor accessor = new();

        Assert.Throws<ArgumentException>(() => accessor.Set(" "));
    }
}

public sealed class FakeCurrentUserTests
{
    [Fact]
    public void SetAuthenticatedBuildsPrincipalWithNameAndRoleClaims()
    {
        FakeCurrentUser currentUser = new();

        currentUser.SetAuthenticated("user-1", "Ada", ["admin", "admin"]);

        Assert.True(currentUser.IsAuthenticated);
        Assert.Single(currentUser.Roles);
        Assert.True(currentUser.IsInRole("admin"));
        Assert.Contains(currentUser.Principal.Claims, claim => claim.Type == ClaimTypes.NameIdentifier && claim.Value == "user-1");
        Assert.Contains(currentUser.Principal.Claims, claim => claim.Type == ClaimTypes.Name && claim.Value == "Ada");
        Assert.Contains(currentUser.Principal.Claims, claim => claim.Type == ClaimTypes.Role && claim.Value == "admin");
    }

    [Fact]
    public void SetAnonymousClearsUserState()
    {
        FakeCurrentUser currentUser = new();
        currentUser.SetAuthenticated("user-1", "Ada", ["admin"]);

        currentUser.SetAnonymous();

        Assert.False(currentUser.IsAuthenticated);
        Assert.Null(currentUser.UserId);
        Assert.Null(currentUser.UserName);
        Assert.Empty(currentUser.Roles);
        Assert.Empty(currentUser.Claims);
    }
}

public sealed class ResultAssertionsTests
{
    [Fact]
    public void ShouldBeSuccessSupportsSucceededProperty()
    {
        SucceededResult result = new(true);

        SucceededResult returned = ResultAssertions.ShouldBeSuccess(result);

        Assert.Same(result, returned);
    }

    [Fact]
    public void ShouldHaveErrorCodeSupportsErrorsCollection()
    {
        FailureResult result = new(true, [new ResultError("Name.Required")]);

        FailureResult returned = ResultAssertions.ShouldBeFailure(result);
        ResultAssertions.ShouldHaveErrorCode(returned, "Name.Required");

        Assert.Same(result, returned);
    }

    [Fact]
    public void ShouldHaveErrorCodeReportsKnownCodesWhenMissing()
    {
        FailureResult result = new(true, [new ResultError("Name.Required")]);

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            () => ResultAssertions.ShouldHaveErrorCode(result, "Email.Required"));

        Assert.Contains("Name.Required", exception.Message, StringComparison.Ordinal);
    }

    private sealed record SucceededResult(bool Succeeded);

    private sealed record FailureResult(bool IsFailure, IReadOnlyCollection<ResultError> Errors);

    private sealed record ResultError(string Code);
}

public sealed class ValidationFailureBuilderTests
{
    [Fact]
    public void BuildReturnsConfiguredValidationFailure()
    {
        object attemptedValue = new();
        object customState = new();

        TestValidationFailure failure = ValidationFailureBuilder
            .Create()
            .WithPropertyName("Email")
            .WithErrorMessage("Email is required.")
            .WithErrorCode("Required")
            .WithAttemptedValue(attemptedValue)
            .WithSeverity("Error")
            .WithCustomState(customState)
            .Build();

        Assert.Equal("Email", failure.PropertyName);
        Assert.Equal("Email is required.", failure.ErrorMessage);
        Assert.Equal("Required", failure.ErrorCode);
        Assert.Same(attemptedValue, failure.AttemptedValue);
        Assert.Equal("Error", failure.Severity);
        Assert.Same(customState, failure.CustomState);
    }
}
