using Soenneker.Extensions.Char;
using Soenneker.Extensions.String;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Soenneker.Utils.Case;

public static partial class CaseUtil
{
    /// <summary>
    /// Converts any supported input casing to kebab-case.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToKebab(ReadOnlySpan<char> input)
    {
        return WriteKebabLower(input);
    }

    /// <summary>
    /// Converts any supported input casing to snake_case.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToSnake(ReadOnlySpan<char> input)
    {
        return WriteSnakeLower(input);
    }

    /// <summary>
    /// Converts any supported input casing to UPPER_SNAKE_CASE.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToUpperSnake(ReadOnlySpan<char> input)
    {
        return WriteUpperSnake(input);
    }

    /// <summary>
    /// Converts any supported input casing to dot.case.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToDot(ReadOnlySpan<char> input)
    {
        return WriteDotLower(input);
    }

    /// <summary>
    /// Converts any supported input casing to flatcase.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToFlat(ReadOnlySpan<char> input)
    {
        return WriteFlatLower(input);
    }

    /// <summary>
    /// Converts any supported input casing to path/case.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToPath(ReadOnlySpan<char> input)
    {
        return WritePathLower(input);
    }

    /// <summary>
    /// Converts any supported input casing to space case.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToSpace(ReadOnlySpan<char> input)
    {
        return WriteSpaceLower(input);
    }

    /// <summary>
    /// Converts any supported input casing to Train-Case.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToTrain(ReadOnlySpan<char> input)
    {
        return WriteTrain(input);
    }

    /// <summary>
    /// Converts any supported input casing to PascalCase.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToPascal(ReadOnlySpan<char> input)
    {
        return WritePascal(input);
    }

    /// <summary>
    /// Converts any supported input casing to camelCase.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToCamel(ReadOnlySpan<char> input)
    {
        return WriteCamel(input);
    }

    /// <summary>
    /// Converts any supported input casing to a title-cased phrase.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToTitle(ReadOnlySpan<char> input, CultureInfo? culture = null)
    {
        return WriteTitle(input, culture ?? CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts any supported input casing to a title-cased phrase.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToTitle(string? value, CultureInfo? culture = null)
    {
        if (value.IsNullOrWhiteSpace())
            return string.Empty;

        return WriteTitle(value.AsSpan(), culture ?? CultureInfo.InvariantCulture);
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

        var len = 0;
        var prevWasDash = true;

        for (var i = 0; i < input.Length; i++)
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

        if (len > 0 && prevWasDash)
            len--;

        if (len <= 0)
            return string.Empty;

        return string.Create(len, input, static (dest, src) =>
        {
            var di = 0;
            var prevWasDash = true;

            for (var i = 0; i < src.Length; i++)
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

            if (di > 0 && dest[di - 1] == '-')
                di--;
        });
    }
}
