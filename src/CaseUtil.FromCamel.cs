using Soenneker.Extensions.Char;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Soenneker.Utils.Case;

public static partial class CaseUtil
{
    /// <summary>
    /// Converts a camel case or Pascal case string to a human-readable title with spaces and proper casing.
    /// </summary>
    /// <remarks>Acronyms and digit boundaries are handled to produce readable titles. The method trims
    /// whitespace and collapses separators to single spaces. Suitable for formatting identifiers or property names for
    /// display.</remarks>
    /// <param name="value">The input string to convert. Can be null or contain leading/trailing whitespace.</param>
    /// <param name="culture">The culture to use for casing rules. If null, uses the invariant culture.</param>
    /// <returns>A title-cased string with spaces inserted between words. Returns an empty string if the input is null, empty, or
    /// contains only whitespace.</returns>
    public static string CamelToTitle(string? value, CultureInfo? culture = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        culture ??= CultureInfo.InvariantCulture;

        ReadOnlySpan<char> all = value.AsSpan();

        // Trim outer whitespace (fast)
        var start = 0;
        while (start < all.Length && all[start]
                   .IsWhiteSpaceFast())
            start++;

        int end = all.Length - 1;
        while (end >= start && all[end]
                   .IsWhiteSpaceFast())
            end--;

        if (end < start)
            return string.Empty;

        int inputLen = end - start + 1;

        // Prepass to compute exact output length (required for string.Create)
        int outLen = ComputeOutputLength(all.Slice(start, inputLen));
        if (outLen == 0)
            return string.Empty;

        // State cannot contain ReadOnlySpan<char> (ref struct), but can contain string + indices.
        var state = (Source: value, Start: start, Length: inputLen, Culture: culture);

        return string.Create(outLen, state, static (dest, st) =>
        {
            ReadOnlySpan<char> src = st.Source.AsSpan(st.Start, st.Length);
            CultureInfo culture = st.Culture;

            var w = 0;

            var atWordStart = true;
            var prevOutWasSpace = true;

            var prevSig = '\0';
            var prevSigHad = false;

            for (var i = 0; i < src.Length; i++)
            {
                char c = src[i];

                // Collapse boundaries to single spaces
                if (c.IsTokenSeparator())
                {
                    if (!prevOutWasSpace && w > 0)
                    {
                        dest[w++] = ' ';
                        prevOutWasSpace = true;
                        atWordStart = true;
                        prevSigHad = false;
                    }

                    continue;
                }

                bool isLetter = c.IsLetterFast(); // ASCII fast + Unicode fallback
                bool isDigit = !isLetter && c.IsDigitFast();
                if (!isLetter && !isDigit)
                {
                    // Treat other punctuation as boundary (skip it)
                    if (!prevOutWasSpace && w > 0)
                    {
                        dest[w++] = ' ';
                        prevOutWasSpace = true;
                        atWordStart = true;
                        prevSigHad = false;
                    }

                    continue;
                }

                // Lookahead for acronym boundary: "HTTPServer" => "HTTP Server"
                char next = (i + 1 < src.Length) ? src[i + 1] : '\0';
                bool nextIsLower = next != '\0' && !next.IsTokenSeparator() && next.IsLowerFast();

                var needSpace = false;

                if (!prevOutWasSpace && prevSigHad)
                {
                    bool currIsUpper = c.IsUpperFast();
                    bool prevIsLower = prevSig.IsLowerFast();
                    bool prevIsUpper = prevSig.IsUpperFast();
                    bool prevIsDigit = prevSig.IsDigitFast();
                    bool currIsDigit = isDigit;

                    // lower -> upper (firstName)
                    if (currIsUpper && prevIsLower)
                        needSpace = true;
                    // acronym -> word (HTTPServer): split before 'S'
                    else if (currIsUpper && prevIsUpper && nextIsLower)
                        needSpace = true;
                    // digit <-> letter boundaries (X509Certificate, version2Value)
                    else if (currIsDigit && !prevIsDigit)
                        needSpace = true;
                    else if (!currIsDigit && prevIsDigit)
                        needSpace = true;
                }

                if (needSpace)
                {
                    dest[w++] = ' ';
                    prevOutWasSpace = true;
                    atWordStart = true;
                    prevSigHad = false;
                }

                // Casing while writing (no second pass)
                char outChar = c;

                if (atWordStart)
                {
                    if (isLetter)
                        outChar = c <= 127 ? c.ToAsciiUpper() : char.ToUpper(c, culture);

                    atWordStart = false;
                }
                else if (isLetter)
                {
                    // Preserve acronym runs: keep uppercase when continuing an uppercase run.
                    // Otherwise, lower-case letters after the first letter of the word.
                    bool currIsUpper = c.IsUpperFast();
                    bool prevIsUpper = prevSigHad && prevSig.IsUpperFast();

                    if (!currIsUpper || !prevIsUpper)
                        outChar = c <= 127 ? c.ToAsciiLower() : char.ToLower(c, culture);
                }

                dest[w++] = outChar;

                prevOutWasSpace = false;
                prevSig = c;
                prevSigHad = true;
            }
        });
    }

    /// <summary>
    /// Converts a camelCase or PascalCase string to kebab-case using ASCII casing rules.
    /// </summary>
    /// <remarks>This method inserts hyphens before uppercase letters that mark word boundaries and converts
    /// all letters to lowercase. Consecutive uppercase sequences are treated as a single word. The conversion is
    /// performed using ASCII casing; non-ASCII characters are not affected.</remarks>
    /// <param name="input">The input span containing the camelCase or PascalCase string to convert. Must contain only ASCII letters;
    /// non-ASCII characters are not transformed.</param>
    /// <returns>A string in kebab-case format, where uppercase letters are replaced with a hyphen followed by their lowercase
    /// equivalent. Returns an empty string if the input is empty.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string CamelToKebab(ReadOnlySpan<char> input)
    {
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
    /// Converts a camelCase string to PascalCase format (ASCII rules).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string CamelToPascal(ReadOnlySpan<char> input)
    {
        if (input.Length == 0)
            return string.Empty;

        // Fast path: already starts with upper
        if (input[0].IsAsciiUpper())
            return new string(input);

        return string.Create(input.Length, input, static (dest, src) =>
        {
            dest[0] = src[0].ToAsciiUpper();
            src.Slice(1).CopyTo(dest.Slice(1));
        });
    }
}