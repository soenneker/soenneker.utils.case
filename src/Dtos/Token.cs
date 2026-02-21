using Soenneker.Utils.Case.Enums;

namespace Soenneker.Utils.Case.Dtos;

internal readonly struct Token(int start, int length, TokenKind kind)
{
    public int Start { get; } = start;
    public int Length { get; } = length;
    public TokenKind Kind { get; } = kind;
}