using System.Text.Json;
using System.Text.Json.Nodes;

namespace Atya.Governance.Testing.Json;

/// <summary>
/// Assertions for JSON payloads.
/// </summary>
public static class JsonAssert
{
    private static readonly JsonDocumentOptions s_documentOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
    };

    private static readonly JsonSerializerOptions s_writerOptions = new()
    {
        WriteIndented = true,
    };

    /// <summary>
    /// Asserts that two JSON payloads are structurally equal.
    /// </summary>
    /// <param name="expectedJson">Expected JSON.</param>
    /// <param name="actualJson">Actual JSON.</param>
    public static void Equal(string expectedJson, string actualJson)
    {
        JsonNode? expected = Parse(expectedJson, nameof(expectedJson));
        JsonNode? actual = Parse(actualJson, nameof(actualJson));

        if (!JsonNode.DeepEquals(expected, actual))
        {
            throw new InvalidOperationException(
                "JSON payloads are not equal." + Environment.NewLine +
                "Expected:" + Environment.NewLine +
                Normalize(expected) + Environment.NewLine +
                "Actual:" + Environment.NewLine +
                Normalize(actual));
        }
    }

    /// <summary>
    /// Asserts that two JSON payloads are structurally different.
    /// </summary>
    /// <param name="expectedJson">Expected JSON.</param>
    /// <param name="actualJson">Actual JSON.</param>
    public static void NotEqual(string expectedJson, string actualJson)
    {
        JsonNode? expected = Parse(expectedJson, nameof(expectedJson));
        JsonNode? actual = Parse(actualJson, nameof(actualJson));

        if (JsonNode.DeepEquals(expected, actual))
        {
            throw new InvalidOperationException("JSON payloads are equal, but were expected to differ.");
        }
    }

    private static JsonNode? Parse(string json, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json, parameterName);

        try
        {
            return JsonNode.Parse(json, documentOptions: s_documentOptions);
        }
        catch (JsonException exception)
        {
            throw new ArgumentException("Value must be valid JSON.", parameterName, exception);
        }
    }

    private static string Normalize(JsonNode? node)
    {
        return node?.ToJsonString(s_writerOptions) ?? "null";
    }
}
