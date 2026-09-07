using AwesomeAssertions;

namespace Soenneker.Utils.Case.Tests;

public sealed class NormalizeKebabRegressionTests
{
    [Test]
    public void Edge_dashes_are_trimmed_without_writing_beyond_the_result()
    {
        CaseUtil.NormalizeKebab("hello-").Should().Be("hello");
        CaseUtil.NormalizeKebab("--hello--world---").Should().Be("hello-world");
        CaseUtil.NormalizeKebab("---").Should().BeEmpty();
        CaseUtil.NormalizeKebab("hello-world").Should().Be("hello-world");
    }
}
