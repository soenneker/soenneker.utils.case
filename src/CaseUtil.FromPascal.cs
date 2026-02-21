using Soenneker.Extensions.Char;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Soenneker.Utils.Case;

public static partial class CaseUtil
{
    /// <summary>
    /// Converts a PascalCase string to its kebab-case equivalent.
    /// </summary>
    /// <remarks>Word boundaries are detected at transitions from lowercase to uppercase characters and at
    /// acronym boundaries. The conversion is case-insensitive and inserts hyphens between words as needed.</remarks>
    /// <param name="input">The input span containing the PascalCase text to convert.</param>
    /// <returns>A string in kebab-case format. Returns an empty string if the input is empty.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string PascalToKebab(ReadOnlySpan<char> input)
    {
        // PascalCase -> kebab-case (same rules as CamelToKebab)
        if (input.Length == 0)
            return string.Empty;

        // First pass: calculate final length
        var extra = 0;

        for (var i = 1; i < input.Length; i++)
        {
            char c = input[i];
            char prev = input[i - 1];

            if (c.IsAsciiUpper())
            {
                // Insert '-' at word boundaries:
                // - lower -> upper (FirstName)
                // - acronym boundary (HTTPServer -> http-server; insert before 'S')
                if (!prev.IsAsciiUpper() ||
                    (i + 1 < input.Length && !input[i + 1].IsAsciiUpper()))
                {
                    extra++;
                }
            }
        }

        return string.Create(input.Length + extra, input, static (dest, src) =>
        {
            var di = 0;

            for (var i = 0; i < src.Length; i++)
            {
                char c = src[i];

                if (i > 0 && c.IsAsciiUpper())
                {
                    char prev = src[i - 1];

                    if (!prev.IsAsciiUpper() ||
                        (i + 1 < src.Length && !src[i + 1].IsAsciiUpper()))
                    {
                        dest[di++] = '-';
                    }
                }

                dest[di++] = c.ToAsciiLower();
            }
        });
    }

    /// <summary>
    /// Converts a PascalCase string to a human-readable title with spaces and proper casing.
    /// </summary>
    /// <remarks>
    /// Uses the same boundary and acronym logic as <see cref="CamelToTitle"/>.
    /// For example:
    /// HTTPServer -> HTTP Server
    /// MyXMLParser -> My XML Parser
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string PascalToTitle(string? value, CultureInfo? culture = null)
    {
        return CamelToTitle(value, culture);
    }

    /// <summary>
    /// Converts a PascalCase string to camelCase format (ASCII rules).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string PascalToCamel(ReadOnlySpan<char> input)
    {
        if (input.Length == 0)
            return string.Empty;

        // Fast path: already starts with lower
        if (!input[0].IsAsciiUpper())
            return new string(input);

        return string.Create(input.Length, input, static (dest, src) =>
        {
            dest[0] = src[0].ToAsciiLower();
            src.Slice(1).CopyTo(dest.Slice(1));
        });
    }
}