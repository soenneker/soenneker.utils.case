using AwesomeAssertions;
using Xunit;

namespace Soenneker.Utils.Case.Tests;

public sealed class CaseUtilTests
{
    [Fact]
    public void KebabToCamel_EmptyString_ReturnsEmpty()
    {
        string result = CaseUtil.KebabToCamel("");

        result.Should().BeEmpty();
    }

    [Fact]
    public void KebabToCamel_SingleWord_ReturnsSameWord()
    {
        string result = CaseUtil.KebabToCamel("hello");

        result.Should().Be("hello");
    }

    [Fact]
    public void KebabToCamel_NoDashes_ReturnsSameString()
    {
        string result = CaseUtil.KebabToCamel("helloworld");

        result.Should().Be("helloworld");
    }

    [Fact]
    public void KebabToCamel_TwoWords_ReturnsCamelCase()
    {
        string result = CaseUtil.KebabToCamel("hello-world");

        result.Should().Be("helloWorld");
    }

    [Fact]
    public void KebabToCamel_MultipleWords_ReturnsCamelCase()
    {
        string result = CaseUtil.KebabToCamel("this-is-a-test");

        result.Should().Be("thisIsATest");
    }

    [Fact]
    public void KebabToCamel_LeadingDash_CapitalizesFirstWord()
    {
        string result = CaseUtil.KebabToCamel("-hello");

        result.Should().Be("Hello");
    }

    [Fact]
    public void KebabToCamel_TrailingDash_IgnoresTrailingDash()
    {
        string result = CaseUtil.KebabToCamel("hello-");

        result.Should().Be("hello");
    }

    [Fact]
    public void KebabToCamel_ConsecutiveDashes_HandlesCorrectly()
    {
        string result = CaseUtil.KebabToCamel("hello--world");

        result.Should().Be("helloWorld");
    }

    [Fact]
    public void KebabToCamel_AlreadyUpperCase_PreservesCase()
    {
        string result = CaseUtil.KebabToCamel("HELLO-WORLD");

        result.Should().Be("HELLOWORLD");
    }

    [Fact]
    public void KebabToCamel_MixedCase_CapitalizesAfterDash()
    {
        string result = CaseUtil.KebabToCamel("Hello-World");

        result.Should().Be("HelloWorld");
    }

    [Fact]
    public void CamelToKebab_EmptyString_ReturnsEmpty()
    {
        string result = CaseUtil.CamelToKebab("");

        result.Should().BeEmpty();
    }

    [Fact]
    public void CamelToKebab_SingleLowercaseWord_ReturnsSameWord()
    {
        string result = CaseUtil.CamelToKebab("hello");

        result.Should().Be("hello");
    }

    [Fact]
    public void CamelToKebab_NoUppercase_ReturnsSameString()
    {
        string result = CaseUtil.CamelToKebab("helloworld");

        result.Should().Be("helloworld");
    }

    [Fact]
    public void CamelToKebab_SimpleCamelCase_ReturnsKebabCase()
    {
        string result = CaseUtil.CamelToKebab("helloWorld");

        result.Should().Be("hello-world");
    }

    [Fact]
    public void CamelToKebab_MultipleWords_ReturnsKebabCase()
    {
        string result = CaseUtil.CamelToKebab("thisIsATest");

        result.Should().Be("this-is-a-test");
    }

    [Fact]
    public void CamelToKebab_PascalCase_ReturnsKebabCase()
    {
        string result = CaseUtil.CamelToKebab("HelloWorld");

        result.Should().Be("hello-world");
    }

    [Fact]
    public void CamelToKebab_Acronym_HandlesCorrectly()
    {
        string result = CaseUtil.CamelToKebab("parseHTMLDocument");

        result.Should().Be("parse-html-document");
    }

    [Fact]
    public void CamelToKebab_AcronymAtStart_HandlesCorrectly()
    {
        string result = CaseUtil.CamelToKebab("HTMLParser");

        result.Should().Be("html-parser");
    }

    [Fact]
    public void CamelToKebab_AcronymAtEnd_HandlesCorrectly()
    {
        string result = CaseUtil.CamelToKebab("parseHTML");

        result.Should().Be("parse-html");
    }

    [Fact]
    public void CamelToKebab_ConsecutiveUppercase_HandlesAcronyms()
    {
        string result = CaseUtil.CamelToKebab("getURLValue");

        result.Should().Be("get-url-value");
    }

    [Fact]
    public void CamelToKebab_SingleUppercaseLetter_ReturnsLowercase()
    {
        string result = CaseUtil.CamelToKebab("A");

        result.Should().Be("a");
    }

    [Fact]
    public void CamelToKebab_AllUppercase_ReturnsAllLowercase()
    {
        string result = CaseUtil.CamelToKebab("ABC");

        result.Should().Be("abc");
    }

    [Fact]
    public void RoundTrip_KebabToCamelToKebab_ReturnsOriginal()
    {
        const string original = "hello-world-test";
        string camel = CaseUtil.KebabToCamel(original);
        string kebab = CaseUtil.CamelToKebab(camel);

        kebab.Should().Be(original);
    }

    [Fact]
    public void RoundTrip_CamelToKebabToCamel_ReturnsOriginal()
    {
        const string original = "helloWorldTest";
        string kebab = CaseUtil.CamelToKebab(original);
        string camel = CaseUtil.KebabToCamel(kebab);

        camel.Should().Be(original);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("a", "a")]
    [InlineData("hello", "hello")]
    [InlineData("hello-world", "helloWorld")]
    [InlineData("one-two-three", "oneTwoThree")]
    public void KebabToCamel_Theory_ReturnsExpected(string input, string expected)
    {
        string result = CaseUtil.KebabToCamel(input);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("a", "a")]
    [InlineData("hello", "hello")]
    [InlineData("helloWorld", "hello-world")]
    [InlineData("oneTwoThree", "one-two-three")]
    public void CamelToKebab_Theory_ReturnsExpected(string input, string expected)
    {
        string result = CaseUtil.CamelToKebab(input);

        result.Should().Be(expected);
    }
}