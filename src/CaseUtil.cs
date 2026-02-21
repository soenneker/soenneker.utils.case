using Soenneker.Extensions.Char;
using System;
using System.Runtime.CompilerServices;

namespace Soenneker.Utils.Case;

/// <summary>
/// High performance case transformation utility methods
/// </summary>
public static partial class CaseUtil
{
    
    private static int ComputeOutputLength(ReadOnlySpan<char> src)
    {
        var len = 0;

        var prevOutWasSpace = true;
        var prevSig = '\0';
        var prevSigHad = false;

        for (var i = 0; i < src.Length; i++)
        {
            char c = src[i];

            if (c.IsTokenSeparator())
            {
                if (!prevOutWasSpace && len > 0)
                {
                    len++; // space
                    prevOutWasSpace = true;
                    prevSigHad = false;
                }
                continue;
            }

            bool isLetter = c.IsLetterFast();
            bool isDigit = !isLetter && c.IsDigitFast();
            if (!isLetter && !isDigit)
            {
                if (!prevOutWasSpace && len > 0)
                {
                    len++;
                    prevOutWasSpace = true;
                    prevSigHad = false;
                }
                continue;
            }

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

                if (currIsUpper && prevIsLower)
                    needSpace = true;
                else if (currIsUpper && prevIsUpper && nextIsLower)
                    needSpace = true;
                else if (currIsDigit && !prevIsDigit)
                    needSpace = true;
                else if (!currIsDigit && prevIsDigit)
                    needSpace = true;
            }

            if (needSpace)
            {
                len++; // space
                prevOutWasSpace = true;
                prevSigHad = false;
            }

            len++; // the character itself

            prevOutWasSpace = false;
            prevSig = c;
            prevSigHad = true;
        }

        // Trim trailing space if we ended with one
        if (len > 0)
        {
            // Only possible if last processed input was boundary/punct which we collapse into space,
            // but we don't emit trailing spaces unless len>0 and prevOutWasSpace false.
            // Kept for safety if rules change.
        }

        return len;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CountNonDash(ReadOnlySpan<char> input)
    {
        int count = 0;
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] != '-')
                count++;
        }
        return count;
    }
}
