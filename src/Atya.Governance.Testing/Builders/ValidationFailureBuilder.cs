namespace Atya.Governance.Testing.Builders;

/// <summary>
/// Builds validation failure data for tests without depending on a specific validation framework.
/// </summary>
public sealed class ValidationFailureBuilder
{
    private string _propertyName = "Name";
    private string _errorMessage = "Validation failed.";
    private string? _errorCode;
    private object? _attemptedValue;
    private string? _severity;
    private object? _customState;

    /// <summary>
    /// Creates a validation failure builder.
    /// </summary>
    /// <returns>A new builder instance.</returns>
    public static ValidationFailureBuilder Create()
    {
        return new ValidationFailureBuilder();
    }

    /// <summary>
    /// Sets the property name.
    /// </summary>
    /// <param name="propertyName">Property name.</param>
    /// <returns>The current builder.</returns>
    public ValidationFailureBuilder WithPropertyName(string propertyName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        _propertyName = propertyName;
        return this;
    }

    /// <summary>
    /// Sets the error message.
    /// </summary>
    /// <param name="errorMessage">Error message.</param>
    /// <returns>The current builder.</returns>
    public ValidationFailureBuilder WithErrorMessage(string errorMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);

        _errorMessage = errorMessage;
        return this;
    }

    /// <summary>
    /// Sets the error code.
    /// </summary>
    /// <param name="errorCode">Error code.</param>
    /// <returns>The current builder.</returns>
    public ValidationFailureBuilder WithErrorCode(string? errorCode)
    {
        _errorCode = errorCode;
        return this;
    }

    /// <summary>
    /// Sets the attempted value.
    /// </summary>
    /// <param name="attemptedValue">Attempted value.</param>
    /// <returns>The current builder.</returns>
    public ValidationFailureBuilder WithAttemptedValue(object? attemptedValue)
    {
        _attemptedValue = attemptedValue;
        return this;
    }

    /// <summary>
    /// Sets the severity.
    /// </summary>
    /// <param name="severity">Severity.</param>
    /// <returns>The current builder.</returns>
    public ValidationFailureBuilder WithSeverity(string? severity)
    {
        _severity = severity;
        return this;
    }

    /// <summary>
    /// Sets custom state.
    /// </summary>
    /// <param name="customState">Custom state.</param>
    /// <returns>The current builder.</returns>
    public ValidationFailureBuilder WithCustomState(object? customState)
    {
        _customState = customState;
        return this;
    }

    /// <summary>
    /// Builds validation failure data.
    /// </summary>
    /// <returns>A validation failure value.</returns>
    public TestValidationFailure Build()
    {
        return new TestValidationFailure(
            _propertyName,
            _errorMessage,
            _errorCode,
            _attemptedValue,
            _severity,
            _customState);
    }
}

/// <summary>
/// Framework-neutral validation failure data.
/// </summary>
/// <param name="PropertyName">Property name.</param>
/// <param name="ErrorMessage">Error message.</param>
/// <param name="ErrorCode">Optional error code.</param>
/// <param name="AttemptedValue">Optional attempted value.</param>
/// <param name="Severity">Optional severity.</param>
/// <param name="CustomState">Optional custom state.</param>
public sealed record TestValidationFailure(
    string PropertyName,
    string ErrorMessage,
    string? ErrorCode,
    object? AttemptedValue,
    string? Severity,
    object? CustomState);
