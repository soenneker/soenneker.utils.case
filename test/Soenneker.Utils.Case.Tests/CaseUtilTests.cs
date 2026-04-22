using AwesomeAssertions;

namespace Soenneker.Utils.Case.Tests;

public sealed class CaseUtilTests
{
    [Test]
    public void KebabToCamel_EmptyString_ReturnsEmpty()
    {
        string result = CaseUtil.ToCamel("");

        result.Should().BeEmpty();
    }

    [Test]
    public void KebabToCamel_SingleWord_ReturnsSameWord()
    {
        string result = CaseUtil.ToCamel("hello");

        result.Should().Be("hello");
    }

    [Test]
    public void KebabToCamel_NoDashes_ReturnsSameString()
    {
        string result = CaseUtil.ToCamel("helloworld");

        result.Should().Be("helloworld");
    }

    [Test]
    public void KebabToCamel_TwoWords_ReturnsCamelCase()
    {
        string result = CaseUtil.ToCamel("hello-world");

        result.Should().Be("helloWorld");
    }

    [Test]
    public void KebabToCamel_MultipleWords_ReturnsCamelCase()
    {
        string result = CaseUtil.ToCamel("this-is-a-test");

        result.Should().Be("thisIsATest");
    }

    [Test]
    public void KebabToCamel_LeadingDash_CapitalizesFirstWord()
    {
        string result = CaseUtil.ToCamel("-hello");

        result.Should().Be("hello");
    }

    [Test]
    public void KebabToCamel_TrailingDash_IgnoresTrailingDash()
    {
        string result = CaseUtil.ToCamel("hello-");

        result.Should().Be("hello");
    }

    [Test]
    public void KebabToCamel_ConsecutiveDashes_HandlesCorrectly()
    {
        string result = CaseUtil.ToCamel("hello--world");

        result.Should().Be("helloWorld");
    }

    [Test]
    public void KebabToCamel_AlreadyUpperCase_PreservesCase()
    {
        string result = CaseUtil.ToCamel("HELLO-WORLD");

        result.Should().Be("helloWorld");
    }

    [Test]
    public void KebabToCamel_MixedCase_CapitalizesAfterDash()
    {
        string result = CaseUtil.ToCamel("Hello-World");

        result.Should().Be("helloWorld");
    }

    [Test]
    public void CamelToKebab_EmptyString_ReturnsEmpty()
    {
        string result = CaseUtil.ToKebab("");

        result.Should().BeEmpty();
    }

    [Test]
    public void CamelToKebab_SingleLowercaseWord_ReturnsSameWord()
    {
        string result = CaseUtil.ToKebab("hello");

        result.Should().Be("hello");
    }

    [Test]
    public void CamelToKebab_NoUppercase_ReturnsSameString()
    {
        string result = CaseUtil.ToKebab("helloworld");

        result.Should().Be("helloworld");
    }

    [Test]
    public void CamelToKebab_SimpleCamelCase_ReturnsKebabCase()
    {
        string result = CaseUtil.ToKebab("helloWorld");

        result.Should().Be("hello-world");
    }

    [Test]
    public void CamelToKebab_MultipleWords_ReturnsKebabCase()
    {
        string result = CaseUtil.ToKebab("thisIsATest");

        result.Should().Be("this-is-a-test");
    }

    [Test]
    public void CamelToKebab_PascalCase_ReturnsKebabCase()
    {
        string result = CaseUtil.ToKebab("HelloWorld");

        result.Should().Be("hello-world");
    }

    [Test]
    public void CamelToKebab_Acronym_HandlesCorrectly()
    {
        string result = CaseUtil.ToKebab("parseHTMLDocument");

        result.Should().Be("parse-html-document");
    }

    [Test]
    public void CamelToKebab_AcronymAtStart_HandlesCorrectly()
    {
        string result = CaseUtil.ToKebab("HTMLParser");

        result.Should().Be("html-parser");
    }

    [Test]
    public void CamelToKebab_AcronymAtEnd_HandlesCorrectly()
    {
        string result = CaseUtil.ToKebab("parseHTML");

        result.Should().Be("parse-html");
    }

    [Test]
    public void CamelToKebab_ConsecutiveUppercase_HandlesAcronyms()
    {
        string result = CaseUtil.ToKebab("getURLValue");

        result.Should().Be("get-url-value");
    }

    [Test]
    public void CamelToKebab_SingleUppercaseLetter_ReturnsLowercase()
    {
        string result = CaseUtil.ToKebab("A");

        result.Should().Be("a");
    }

    [Test]
    public void CamelToKebab_AllUppercase_ReturnsAllLowercase()
    {
        string result = CaseUtil.ToKebab("ABC");

        result.Should().Be("abc");
    }

    [Test]
    public void RoundTrip_KebabToCamelToKebab_ReturnsOriginal()
    {
        const string original = "hello-world-test";
        string camel = CaseUtil.ToCamel(original);
        string kebab = CaseUtil.ToKebab(camel);

        kebab.Should().Be(original);
    }

    [Test]
    public void RoundTrip_CamelToKebabToCamel_ReturnsOriginal()
    {
        const string original = "helloWorldTest";
        string kebab = CaseUtil.ToKebab(original);
        string camel = CaseUtil.ToCamel(kebab);

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
        string result = CaseUtil.ToCamel(input);

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
        string result = CaseUtil.ToKebab(input);

        result.Should().Be(expected);
    }
}