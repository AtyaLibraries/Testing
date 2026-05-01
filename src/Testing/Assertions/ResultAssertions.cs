using System.Collections;
using System.Reflection;

namespace Atya.Governance.Testing.Assertions;

/// <summary>
/// Reflection-based assertions for result-like objects.
/// </summary>
public static class ResultAssertions
{
    /// <summary>
    /// Asserts that a result-like object represents success.
    /// </summary>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <param name="result">Result instance.</param>
    /// <returns>The supplied result.</returns>
    public static TResult ShouldBeSuccess<TResult>(TResult result)
        where TResult : notnull
    {
        if (!TryReadSuccess(result, out bool isSuccess))
        {
            throw new InvalidOperationException("Result object must expose a boolean IsSuccess, Succeeded, IsFailure, or Failed property.");
        }

        if (!isSuccess)
        {
            throw new InvalidOperationException("Expected result to be successful.");
        }

        return result;
    }

    /// <summary>
    /// Asserts that a result-like object represents failure.
    /// </summary>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <param name="result">Result instance.</param>
    /// <returns>The supplied result.</returns>
    public static TResult ShouldBeFailure<TResult>(TResult result)
        where TResult : notnull
    {
        if (!TryReadSuccess(result, out bool isSuccess))
        {
            throw new InvalidOperationException("Result object must expose a boolean IsSuccess, Succeeded, IsFailure, or Failed property.");
        }

        if (isSuccess)
        {
            throw new InvalidOperationException("Expected result to be failed.");
        }

        return result;
    }

    /// <summary>
    /// Asserts that a result-like object exposes an error with the expected code.
    /// </summary>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <param name="result">Result instance.</param>
    /// <param name="expectedCode">Expected error code.</param>
    /// <returns>The supplied result.</returns>
    public static TResult ShouldHaveErrorCode<TResult>(TResult result, string expectedCode)
        where TResult : notnull
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expectedCode);

        List<string> errorCodes = ReadErrorCodes(result).ToList();

        if (!errorCodes.Contains(expectedCode, StringComparer.Ordinal))
        {
            string knownCodes = errorCodes.Count == 0 ? "<none>" : string.Join(", ", errorCodes);

            throw new InvalidOperationException($"Expected error code '{expectedCode}', but found {knownCodes}.");
        }

        return result;
    }

    private static bool TryReadSuccess(object result, out bool isSuccess)
    {
        if (TryReadBooleanProperty(result, "IsSuccess", out isSuccess) ||
            TryReadBooleanProperty(result, "Succeeded", out isSuccess))
        {
            return true;
        }

        if (TryReadBooleanProperty(result, "IsFailure", out bool isFailure) ||
            TryReadBooleanProperty(result, "Failed", out isFailure))
        {
            isSuccess = !isFailure;
            return true;
        }

        return false;
    }

    private static bool TryReadBooleanProperty(object source, string propertyName, out bool value)
    {
        PropertyInfo? property = source.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

        if (property is not null &&
            property.PropertyType == typeof(bool) &&
            property.GetValue(source) is bool propertyValue)
        {
            value = propertyValue;
            return true;
        }

        value = false;
        return false;
    }

    private static IEnumerable<string> ReadErrorCodes(object result)
    {
        foreach (string code in ReadErrorCodesFromProperty(result, "Error"))
        {
            yield return code;
        }

        foreach (string code in ReadErrorCodesFromProperty(result, "Errors"))
        {
            yield return code;
        }
    }

    private static IEnumerable<string> ReadErrorCodesFromProperty(object source, string propertyName)
    {
        PropertyInfo? property = source.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        object? value = property?.GetValue(source);

        if (value is null)
        {
            yield break;
        }

        if (value is string)
        {
            yield break;
        }

        if (TryReadStringProperty(value, "Code", out string? code) &&
            code is not null)
        {
            yield return code;
            yield break;
        }

        if (value is IEnumerable errors)
        {
            foreach (object? error in errors)
            {
                if (error is not null &&
                    TryReadStringProperty(error, "Code", out string? itemCode) &&
                    itemCode is not null)
                {
                    yield return itemCode;
                }
            }
        }
    }

    private static bool TryReadStringProperty(object source, string propertyName, out string? value)
    {
        PropertyInfo? property = source.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

        if (property is not null &&
            property.PropertyType == typeof(string) &&
            property.GetValue(source) is string propertyValue)
        {
            value = propertyValue;
            return true;
        }

        value = null;
        return false;
    }
}
