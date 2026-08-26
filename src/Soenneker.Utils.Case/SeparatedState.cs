using System;

namespace Soenneker.Utils.Case;

internal readonly ref struct SeparatedState(ReadOnlySpan<char> source, char separator)
{
    internal readonly ReadOnlySpan<char> Source = source;
    internal readonly char Separator = separator;
}
