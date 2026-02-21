using Soenneker.Extensions.Char;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Soenneker.Utils.Case.Dtos;
using Soenneker.Utils.Case.Enums;

namespace Soenneker.Utils.Case;

/// <summary>
/// High performance case transformation utility methods
/// </summary>
public static partial class CaseUtil
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsTokenChar(char c)
    {
        return c.IsLetterFast() || c.IsDigitFast();
    }

    private static bool TryReadToken(ReadOnlySpan<char> input, ref int index, out Token token)
    {
        int len = input.Length;

        while (index < len)
        {
            char c = input[index];

            if (IsTokenChar(c))
                break;

            index++;
        }

        if (index >= len)
        {
            token = default;
            return false;
        }

        int start = index;
        char first = input[index];

        // Keep common version markers together: v2, v10, V3.
        // Other letter/digit boundaries still split (e.g., X + 509).
        if ((first == 'v' || first == 'V') && start + 1 < len && input[start + 1].IsDigitFast())
        {
            index += 2;
            while (index < len && input[index].IsDigitFast())
            {
                index++;
            }

            token = new Token(start, index - start, TokenKind.Word);
            return true;
        }

        if (first.IsDigitFast())
        {
            index++;
            while (index < len && input[index].IsDigitFast())
            {
                index++;
            }

            token = new Token(start, index - start, TokenKind.Number);
            return true;
        }

        if (first.IsUpperFast())
        {
            index++;

            while (index < len && input[index].IsUpperFast())
            {
                index++;
            }

            int upperRunLength = index - start;

            if (upperRunLength > 1 && index < len && input[index].IsLowerFast())
            {
                index--;
                token = new Token(start, index - start, TokenKind.Acronym);
                return true;
            }

            if (upperRunLength > 1)
            {
                token = new Token(start, upperRunLength, TokenKind.Acronym);
                return true;
            }

            while (index < len && input[index].IsLowerFast())
            {
                index++;
            }

            token = new Token(start, index - start, TokenKind.Word);
            return true;
        }

        index++;
        while (index < len && input[index].IsLowerFast())
        {
            index++;
        }

        token = new Token(start, index - start, TokenKind.Word);
        return true;
    }

    private static void ComputeTokenStats(ReadOnlySpan<char> input, out int tokenCount, out int charCount)
    {
        tokenCount = 0;
        charCount = 0;

        var idx = 0;
        while (TryReadToken(input, ref idx, out Token token))
        {
            tokenCount++;
            charCount += token.Length;
        }
    }

    private static string WriteSeparatedLower(ReadOnlySpan<char> input, char separator)
    {
        ComputeTokenStats(input, out int tokenCount, out int charCount);
        if (tokenCount == 0)
            return string.Empty;

        int outputLength = charCount + tokenCount - 1;
        var state = (Source: input.ToString(), Separator: separator);

        return string.Create(outputLength, state, static (dest, st) =>
        {
            ReadOnlySpan<char> src = st.Source.AsSpan();
            var di = 0;
            var idx = 0;
            var wroteToken = false;

            while (TryReadToken(src, ref idx, out Token token))
            {
                if (wroteToken)
                    dest[di++] = st.Separator;

                int end = token.Start + token.Length;
                for (int i = token.Start; i < end; i++)
                {
                    dest[di++] = src[i].ToAsciiLower();
                }

                wroteToken = true;
            }
        });
    }

    private static string WriteSeparatedUpper(ReadOnlySpan<char> input, char separator)
    {
        ComputeTokenStats(input, out int tokenCount, out int charCount);
        if (tokenCount == 0)
            return string.Empty;

        int outputLength = charCount + tokenCount - 1;
        var state = (Source: input.ToString(), Separator: separator);

        return string.Create(outputLength, state, static (dest, st) =>
        {
            ReadOnlySpan<char> src = st.Source.AsSpan();
            var di = 0;
            var idx = 0;
            var wroteToken = false;

            while (TryReadToken(src, ref idx, out Token token))
            {
                if (wroteToken)
                    dest[di++] = st.Separator;

                int end = token.Start + token.Length;
                for (int i = token.Start; i < end; i++)
                {
                    dest[di++] = src[i].ToAsciiUpper();
                }

                wroteToken = true;
            }
        });
    }

    private static string WriteFlatLower(ReadOnlySpan<char> input)
    {
        ComputeTokenStats(input, out _, out int charCount);
        if (charCount == 0)
            return string.Empty;

        return string.Create(charCount, input.ToString(), static (dest, source) =>
        {
            ReadOnlySpan<char> src = source.AsSpan();
            var di = 0;
            var idx = 0;

            while (TryReadToken(src, ref idx, out Token token))
            {
                int end = token.Start + token.Length;
                for (int i = token.Start; i < end; i++)
                {
                    dest[di++] = src[i].ToAsciiLower();
                }
            }
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string WriteKebabLower(ReadOnlySpan<char> input)
    {
        return WriteSeparatedLower(input, '-');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string WriteSnakeLower(ReadOnlySpan<char> input)
    {
        return WriteSeparatedLower(input, '_');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string WriteUpperSnake(ReadOnlySpan<char> input)
    {
        return WriteSeparatedUpper(input, '_');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string WriteDotLower(ReadOnlySpan<char> input)
    {
        return WriteSeparatedLower(input, '.');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string WritePathLower(ReadOnlySpan<char> input)
    {
        return WriteSeparatedLower(input, '/');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string WriteSpaceLower(ReadOnlySpan<char> input)
    {
        return WriteSeparatedLower(input, ' ');
    }

    private static string WriteTitle(ReadOnlySpan<char> input, CultureInfo culture)
    {
        ComputeTokenStats(input, out int tokenCount, out int charCount);
        if (tokenCount == 0)
            return string.Empty;

        int outputLength = charCount + tokenCount - 1;

        (string Source, CultureInfo Culture) state = (Source: input.ToString(), Culture: culture);

        return string.Create(outputLength, state, static (dest, st) =>
        {
            ReadOnlySpan<char> src = st.Source.AsSpan();
            CultureInfo ci = st.Culture;

            var di = 0;
            var idx = 0;
            var wroteToken = false;

            while (TryReadToken(src, ref idx, out Token token))
            {
                if (wroteToken)
                    dest[di++] = ' ';

                int end = token.Start + token.Length;

                if (token.Kind == TokenKind.Acronym)
                {
                    for (int i = token.Start; i < end; i++)
                    {
                        char c = src[i];
                        dest[di++] = c <= 127 ? c.ToAsciiUpper() : char.ToUpper(c, ci);
                    }
                }
                else if (token.Kind == TokenKind.Number)
                {
                    for (int i = token.Start; i < end; i++)
                    {
                        dest[di++] = src[i];
                    }
                }
                else
                {
                    char first = src[token.Start];
                    dest[di++] = first <= 127 ? first.ToAsciiUpper() : char.ToUpper(first, ci);

                    for (int i = token.Start + 1; i < end; i++)
                    {
                        char c = src[i];
                        dest[di++] = c <= 127 ? c.ToAsciiLower() : char.ToLower(c, ci);
                    }
                }

                wroteToken = true;
            }
        });
    }

    private static string WritePascal(ReadOnlySpan<char> input)
    {
        ComputeTokenStats(input, out int tokenCount, out int charCount);
        if (tokenCount == 0)
            return string.Empty;

        return string.Create(charCount, input.ToString(), static (dest, source) =>
        {
            ReadOnlySpan<char> src = source.AsSpan();
            var di = 0;
            var idx = 0;

            while (TryReadToken(src, ref idx, out Token token))
            {
                int end = token.Start + token.Length;

                if (token.Kind == TokenKind.Acronym)
                {
                    for (int i = token.Start; i < end; i++)
                    {
                        dest[di++] = src[i].ToAsciiUpper();
                    }
                }
                else if (token.Kind == TokenKind.Number)
                {
                    for (int i = token.Start; i < end; i++)
                    {
                        dest[di++] = src[i];
                    }
                }
                else
                {
                    dest[di++] = src[token.Start].ToAsciiUpper();

                    for (int i = token.Start + 1; i < end; i++)
                    {
                        dest[di++] = src[i].ToAsciiLower();
                    }
                }
            }
        });
    }

    private static string WriteCamel(ReadOnlySpan<char> input)
    {
        ComputeTokenStats(input, out int tokenCount, out int charCount);
        if (tokenCount == 0)
            return string.Empty;

        return string.Create(charCount, input.ToString(), static (dest, source) =>
        {
            ReadOnlySpan<char> src = source.AsSpan();
            var di = 0;
            var idx = 0;
            var tokenIndex = 0;

            while (TryReadToken(src, ref idx, out Token token))
            {
                int end = token.Start + token.Length;

                if (tokenIndex == 0)
                {
                    for (int i = token.Start; i < end; i++)
                    {
                        dest[di++] = src[i].ToAsciiLower();
                    }
                }
                else if (token.Kind == TokenKind.Number)
                {
                    for (int i = token.Start; i < end; i++)
                    {
                        dest[di++] = src[i];
                    }
                }
                else
                {
                    dest[di++] = src[token.Start].ToAsciiUpper();

                    for (int i = token.Start + 1; i < end; i++)
                    {
                        dest[di++] = src[i].ToAsciiLower();
                    }
                }

                tokenIndex++;
            }
        });
    }

    private static string WriteTrain(ReadOnlySpan<char> input)
    {
        ComputeTokenStats(input, out int tokenCount, out int charCount);
        if (tokenCount == 0)
            return string.Empty;

        int outputLength = charCount + tokenCount - 1;

        return string.Create(outputLength, input.ToString(), static (dest, source) =>
        {
            ReadOnlySpan<char> src = source.AsSpan();
            var di = 0;
            var idx = 0;
            var wroteToken = false;

            while (TryReadToken(src, ref idx, out Token token))
            {
                if (wroteToken)
                    dest[di++] = '-';

                int end = token.Start + token.Length;

                if (token.Kind == TokenKind.Acronym)
                {
                    for (int i = token.Start; i < end; i++)
                    {
                        dest[di++] = src[i].ToAsciiUpper();
                    }
                }
                else if (token.Kind == TokenKind.Number)
                {
                    for (int i = token.Start; i < end; i++)
                    {
                        dest[di++] = src[i];
                    }
                }
                else
                {
                    dest[di++] = src[token.Start].ToAsciiUpper();

                    for (int i = token.Start + 1; i < end; i++)
                    {
                        dest[di++] = src[i].ToAsciiLower();
                    }
                }

                wroteToken = true;
            }
        });
    }

}
