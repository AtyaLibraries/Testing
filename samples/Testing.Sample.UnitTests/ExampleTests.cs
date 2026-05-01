using Atya.Governance.Testing.Assertions;
using Atya.Governance.Testing.Builders;
using Atya.Governance.Testing.Diagnostics;
using Atya.Governance.Testing.Json;
using Atya.Governance.Testing.Security;
using Atya.Governance.Testing.Time;
using Xunit;

namespace Testing.Sample.UnitTests;

public sealed class ExampleTests
{
    [Fact]
    public void FakeClockCanAdvanceTime()
    {
        FakeClock clock = new(new DateTimeOffset(2026, 4, 18, 12, 0, 0, TimeSpan.Zero));

        clock.Advance(TimeSpan.FromMinutes(15));

        Assert.Equal(new DateTimeOffset(2026, 4, 18, 12, 15, 0, TimeSpan.Zero), clock.UtcNow);
    }

    [Fact]
    public void JsonAssertComparesJsonByStructure()
    {
        const string Expected = """{"id":1,"name":"Ada"}""";
        const string Actual = """
            {
                "name": "Ada",
                "id": 1
            }
            """;

        JsonAssert.Equal(Expected, Actual);
    }

    [Fact]
    public void FakeCorrelationIdAccessorCanBeChanged()
    {
        FakeCorrelationIdAccessor accessor = new();

        accessor.Set("order-123");

        Assert.True(accessor.HasCorrelationId);
        Assert.Equal("order-123", accessor.CorrelationId);
    }

    [Fact]
    public void FakeCurrentUserCanRepresentAuthenticatedUser()
    {
        FakeCurrentUser currentUser = new();

        currentUser.SetAuthenticated("user-1", "Ada", ["admin"]);

        Assert.True(currentUser.IsAuthenticated);
        Assert.True(currentUser.IsInRole("admin"));
        Assert.Equal("Ada", currentUser.UserName);
    }

    [Fact]
    public void ValidationFailureBuilderCreatesFailureData()
    {
        TestValidationFailure failure = ValidationFailureBuilder
            .Create()
            .WithPropertyName("Email")
            .WithErrorMessage("Email is required.")
            .WithErrorCode("Required")
            .Build();

        Assert.Equal("Email", failure.PropertyName);
        Assert.Equal("Required", failure.ErrorCode);
    }

    [Fact]
    public void ResultAssertionsSupportCommonResultShapes()
    {
        SampleResult result = new(false, new SampleError("Name.Required"));

        ResultAssertions.ShouldBeFailure(result);
        ResultAssertions.ShouldHaveErrorCode(result, "Name.Required");
    }

    private sealed record SampleResult(bool IsSuccess, SampleError Error);

    private sealed record SampleError(string Code);
}
