using Soenneker.Extensions.Char;
using System;
using System.Runtime.CompilerServices;

namespace Soenneker.Utils.Case;

public static partial class CaseUtil
{

    /// <summary>
    /// Converts kebab-case to a display title (e.g., "my-http-server" -> "My Http Server").
    /// ASCII casing only.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string KebabToTitle(ReadOnlySpan<char> input)
    {
        if (input.Length == 0)
            return string.Empty;

        int outLen = CountNonDash(input);
        if (outLen == 0)
            return string.Empty;

        // Spaces between words = number of word breaks we emit
        int spaces = 0;
        bool prevWasDash = true;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (c == '-')
            {
                prevWasDash = true;
                continue;
            }

            if (!prevWasDash)
            {
                // continue same word
            }
            else
            {
                if (i != 0) spaces++; // new word after a dash-run (except leading)
            }

            prevWasDash = false;
        }

        // If it started with non-dash, we counted a space for the first word; remove it.
        // Example: "a-b" => spaces computed as 2, but should be 1.
        if (input.Length > 0 && input[0] != '-' && spaces > 0)
            spaces--;

        int finalLen = outLen + spaces;
        if (finalLen <= 0)
            return string.Empty;

        return string.Create(finalLen, input, static (dest, src) =>
        {
            int di = 0;
            bool atWordStart = true;

            for (int i = 0; i < src.Length; i++)
            {
                char c = src[i];

                if (c == '-')
                {
                    atWordStart = true;
                    continue;
                }

                if (atWordStart)
                {
                    if (di != 0)
                        dest[di++] = ' ';

                    dest[di++] = c.ToAsciiUpper();
                    atWordStart = false;
                }
                else
                {
                    dest[di++] = c.ToAsciiLower();
                }
            }
        });
    }

    /// <summary>
    /// Converts a kebab-case string to PascalCase format.
    /// </summary>
    /// <remarks>This method capitalizes the first letter of each word separated by dashes and removes all
    /// dashes. For example, 'my-variable-name' becomes 'MyVariableName'. The conversion is performed using ASCII
    /// casing; non-ASCII characters are not affected.</remarks>
    /// <param name="input">A read-only span of characters containing the kebab-case input to convert. Dashes ('-') are treated as word
    /// separators.</param>
    /// <returns>A string in PascalCase format, where each word is capitalized and dashes are removed. Returns an empty string if
    /// the input is empty.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string KebabToPascal(ReadOnlySpan<char> input)
    {
        if (input.Length == 0)
            return string.Empty;

        var dashCount = 0;

        foreach (char c in input)
        {
            if (c == '-')
                dashCount++;
        }

        if (dashCount == 0)
        {
            // No separators: just Pascalize first char if needed
            return string.Create(input.Length, input, static (dest, src) =>
            {
                dest[0] = src[0].ToAsciiUpper();
                src.Slice(1).CopyTo(dest.Slice(1));
            });
        }

        return string.Create(input.Length - dashCount, input, static (dest, src) =>
        {
            var di = 0;
            var upperNext = true; // Pascal: first letter is upper

            for (var i = 0; i < src.Length; i++)
            {
                char c = src[i];

                if (c == '-')
                {
                    upperNext = true;
                    continue;
                }

                if (upperNext)
                {
                    dest[di++] = c.ToAsciiUpper();
                    upperNext = false;
                }
                else
                {
                    dest[di++] = c;
                }
            }
        });
    }

    /// <summary>
    /// Converts a kebab-case string to camelCase format.
    /// </summary>
    /// <remarks>Dashes ('-') in the input are removed, and the character immediately following each dash is
    /// converted to uppercase. If the input contains no dashes, the original string is returned unchanged.</remarks>
    /// <param name="input">A read-only span of characters representing the kebab-case input to convert. Cannot be empty.</param>
    /// <returns>A string containing the camelCase representation of the input. Returns an empty string if the input is empty.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string KebabToCamel(ReadOnlySpan<char> input)
    {
        if (input.Length == 0)
            return string.Empty;

        var dashCount = 0;

        foreach (char c in input)
        {
            if (c == '-')
                dashCount++;
        }

        if (dashCount == 0)
            return new string(input);

        return string.Create(input.Length - dashCount, input, static (dest, src) =>
        {
            var di = 0;
            var upperNext = false;

            for (var i = 0; i < src.Length; i++)
            {
                char c = src[i];

                if (c == '-')
                {
                    upperNext = true;
                    continue;
                }

                if (upperNext)
                {
                    dest[di++] = c.ToAsciiUpper();
                    upperNext = false;
                }
                else
                {
                    dest[di++] = c;
                }
            }
        });
    }

    /// <summary>
    /// Normalizes kebab-case by collapsing dash runs and trimming leading/trailing dashes.
    /// Keeps non-dash chars unchanged.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NormalizeKebab(ReadOnlySpan<char> input)
    {
        if (input.Length == 0)
            return string.Empty;

        // Compute exact output length
        int len = 0;
        bool prevWasDash = true; // trims leading dashes

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (c == '-')
            {
                if (!prevWasDash)
                {
                    len++;
                    prevWasDash = true;
                }
                continue;
            }

            len++;
            prevWasDash = false;
        }

        // Trim trailing dash if we ended after writing one
        if (len > 0 && prevWasDash)
            len--;

        if (len <= 0)
            return string.Empty;

        return string.Create(len, input, static (dest, src) =>
        {
            int di = 0;
            bool prevWasDash = true;

            for (int i = 0; i < src.Length; i++)
            {
                char c = src[i];

                if (c == '-')
                {
                    if (!prevWasDash)
                    {
                        dest[di++] = '-';
                        prevWasDash = true;
                    }
                    continue;
                }

                dest[di++] = c;
                prevWasDash = false;
            }

            // If last written was '-', drop it (should only happen if input ended with dash-run)
            if (di > 0 && dest[di - 1] == '-')
                di--;
        });
    }

}