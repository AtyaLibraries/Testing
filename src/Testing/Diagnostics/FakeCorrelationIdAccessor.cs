namespace Atya.Governance.Testing.Diagnostics;

/// <summary>
/// Mutable test double for code that needs a correlation id.
/// </summary>
public sealed class FakeCorrelationIdAccessor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FakeCorrelationIdAccessor"/> class.
    /// </summary>
    public FakeCorrelationIdAccessor()
        : this("test-correlation-id")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeCorrelationIdAccessor"/> class.
    /// </summary>
    /// <param name="correlationId">Initial correlation id.</param>
    public FakeCorrelationIdAccessor(string? correlationId)
    {
        CorrelationId = correlationId;
    }

    /// <summary>
    /// Gets the current correlation id.
    /// </summary>
    public string? CorrelationId { get; private set; }

    /// <summary>
    /// Gets a value indicating whether a correlation id is available.
    /// </summary>
    public bool HasCorrelationId => !string.IsNullOrWhiteSpace(CorrelationId);

    /// <summary>
    /// Sets the current correlation id.
    /// </summary>
    /// <param name="correlationId">Correlation id to expose.</param>
    public void Set(string correlationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);

        CorrelationId = correlationId;
    }

    /// <summary>
    /// Clears the current correlation id.
    /// </summary>
    public void Clear()
    {
        CorrelationId = null;
    }
}
