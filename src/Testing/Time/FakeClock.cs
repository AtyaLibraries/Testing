namespace Atya.Governance.Testing.Time;

/// <summary>
/// Test clock with manually controlled time.
/// </summary>
public sealed class FakeClock
{
    private DateTimeOffset _utcNow;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeClock"/> class at the Unix epoch.
    /// </summary>
    public FakeClock()
        : this(DateTimeOffset.UnixEpoch)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeClock"/> class.
    /// </summary>
    /// <param name="utcNow">Initial UTC time.</param>
    public FakeClock(DateTimeOffset utcNow)
    {
        SetUtcNow(utcNow);
    }

    /// <summary>
    /// Gets the current fake UTC time.
    /// </summary>
    public DateTimeOffset UtcNow => _utcNow;

    /// <summary>
    /// Gets the current fake local time.
    /// </summary>
    public DateTimeOffset Now => _utcNow.ToLocalTime();

    /// <summary>
    /// Gets the current fake UTC date.
    /// </summary>
    public DateOnly Today => DateOnly.FromDateTime(_utcNow.UtcDateTime);

    /// <summary>
    /// Sets the current fake UTC time.
    /// </summary>
    /// <param name="utcNow">The new current time.</param>
    public void SetUtcNow(DateTimeOffset utcNow)
    {
        _utcNow = utcNow.ToUniversalTime();
    }

    /// <summary>
    /// Advances the current fake time.
    /// </summary>
    /// <param name="amount">Amount to advance.</param>
    public void Advance(TimeSpan amount)
    {
        if (amount < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), amount, "Advance amount must not be negative.");
        }

        _utcNow = _utcNow.Add(amount);
    }

    /// <summary>
    /// Rewinds the current fake time.
    /// </summary>
    /// <param name="amount">Amount to rewind.</param>
    public void Rewind(TimeSpan amount)
    {
        if (amount < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), amount, "Rewind amount must not be negative.");
        }

        _utcNow = _utcNow.Subtract(amount);
    }
}
