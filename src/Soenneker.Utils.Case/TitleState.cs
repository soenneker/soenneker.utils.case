using System;
using System.Globalization;

namespace Soenneker.Utils.Case;

internal readonly ref struct TitleState(ReadOnlySpan<char> source, CultureInfo culture)
{
    internal readonly ReadOnlySpan<char> Source = source;
    internal readonly CultureInfo Culture = culture;
}
