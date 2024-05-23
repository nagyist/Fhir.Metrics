using System;
using FluentAssertions;
using Xunit;

namespace Fhir.Metrics.Tests.Tests;

public class FhirMetricServiceTests
{
    private static readonly IMetricService _service = FhirMetricService.Instance.Value;

    [Theory]
    [InlineData("1", "m", "http://unitsofmeasure.org", "1", "m", "http://unitsofmeasure.org")]
    [InlineData("1", "km", "http://unitsofmeasure.org", "1000", "m", "http://unitsofmeasure.org")]
    [InlineData("1", "[in_i]", "http://unitsofmeasure.org", "0.025400", "m", "http://unitsofmeasure.org")]
    [InlineData("-80", "cm", "http://unitsofmeasure.org", "-0.800", "m", "http://unitsofmeasure.org")]
    public void MetricService_CanonicalReduction_ShouldReturnCorrectReduction(string value1, string unit1, string codesystem1,
        string value2, string unit2, string codesystem2)
    {
        var quantity1 = (value1, unit1, codesystem1);

        _ = _service.TryCanonicalize(quantity1, out var canonical);

        canonical.Should().NotBeNull();
        canonical!.Value.value.Should().Be(value2);
        canonical.Value.unit.Should().Be(unit2);
        canonical.Value.codesystem.Should().Be(codesystem2);
    }

    [Fact]
    public void MetricService_CanonicalReduction_ShouldReturnNullForInvalidInput()
    {
        var quantity1 = ("1", "blub", "http://unitsofmeasure.org");

        var _ = _service.TryCanonicalize(quantity1, out var canonical);

        canonical.Should().BeNull();
    }

    [Theory]
    [InlineData("1", "m", "http://unitsofmeasure.org", "1", "m", "http://unitsofmeasure.org", "1", "", "http://unitsofmeasure.org")]
    [InlineData("1", "m2", "http://unitsofmeasure.org", "1", "m", "http://unitsofmeasure.org", "1", "m", "http://unitsofmeasure.org")]
    [InlineData("1", "[in_i]", "http://unitsofmeasure.org", "1", "m", "http://unitsofmeasure.org", "0.025400", "", "http://unitsofmeasure.org")]
    [InlineData("6", "m", "http://unitsofmeasure.org", "2", "m", "http://unitsofmeasure.org", "3", "", "http://unitsofmeasure.org")]
    public void MetricService_Divide_ShouldReturnCorrectDivision(string value1, string unit1, string codesystem1,
        string value2, string unit2, string codesystem2, string value3, string unit3, string codesystem3)
    {
        var quantity1 = (value1, unit1, codesystem1);
        var quantity2 = (value2, unit2, codesystem2);

        var _ = _service.TryDivide(quantity1, quantity2, out var result);

        result.Should().NotBeNull();
        result!.Value.value.Should().Be(value3);
        result.Value.unit.Should().Be(unit3);
        result.Value.codesystem.Should().Be(codesystem3);
    }

    [Fact]
    public void MetricService_Divide_ShouldReturnNullForInvalidInput()
    {
        var quantity1 = ("1", "blub", "http://unitsofmeasure.org");
        var quantity2 = ("1", "m", "http://unitsofmeasure.org");

        var _ = _service.TryDivide(quantity1, quantity2, out var result);

        result.Should().BeNull();
    }

    [Theory]
    [InlineData("1", "m", "http://unitsofmeasure.org", "1", "m", "http://unitsofmeasure.org", "1", "m2", "http://unitsofmeasure.org")]
    [InlineData("1", "m2", "http://unitsofmeasure.org", "1", "m", "http://unitsofmeasure.org", "1", "m3", "http://unitsofmeasure.org")]
    [InlineData("1", "[in_i]", "http://unitsofmeasure.org", "1", "m", "http://unitsofmeasure.org", "0.025400", "m2", "http://unitsofmeasure.org")]
    [InlineData("1000", "m", "http://unitsofmeasure.org", "1", "km", "http://unitsofmeasure.org", "1000000", "m2", "http://unitsofmeasure.org")]
    public void MetricService_Multiply_ShouldReturnCorrectMultiplication(string value1, string unit1, string codesystem1,
        string value2, string unit2, string codesystem2, string value3, string unit3, string codesystem3)
    {
        var quantity1 = (value1, unit1, codesystem1);
        var quantity2 = (value2, unit2, codesystem2);

        var _ = _service.TryMultiply(quantity1, quantity2, out var result);

        result.Should().NotBeNull();
        result!.Value.value.Should().Be(value3);
        result.Value.unit.Should().Be(unit3);
        result.Value.codesystem.Should().Be(codesystem3);
    }

    [Fact]
    public void MetricService_Multiply_ShouldReturnNullForInvalidInput()
    {
        var quantity1 = ("1", "blub", "http://unitsofmeasure.org");
        var quantity2 = ("1", "m", "http://unitsofmeasure.org");

        var _ = _service.TryMultiply(quantity1, quantity2, out var result);

        result.Should().BeNull();
    }
    
    [Theory]
    [InlineData("1", "m", "http://unitsofmeasure.org", "1", "m", "http://unitsofmeasure.org", 0)]
    [InlineData("1", "m", "http://unitsofmeasure.org", "2", "m", "http://unitsofmeasure.org", -1)]
    [InlineData("2", "m", "http://unitsofmeasure.org", "1", "m", "http://unitsofmeasure.org", 1)]
    [InlineData("1", "m", "http://unitsofmeasure.org", "1", "km", "http://unitsofmeasure.org", -1)]
    [InlineData("1", "m", "http://unitsofmeasure.org", "10", "[in_i]", "http://unitsofmeasure.org", 1)]
    public void MetricService_Compare_ShouldReturnCorrectComparison(string value1, string unit1, string codesystem1,
        string value2, string unit2, string codesystem2, int result)
    {
        var quantity1 = (value1, unit1, codesystem1);
        var quantity2 = (value2, unit2, codesystem2);

        _ = _service.TryCompare(quantity1, quantity2, out var comparison);

        comparison.Should().NotBeNull();

        _ = result switch
        {
            -1 => comparison.Should().BeLessThan(0),
            0 => comparison.Should().Be(0),
            1 => comparison.Should().BeGreaterThan(0),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    [Fact]
    public void MetricService_Compare_ShouldReturnNullForInvalidInput()
    {
        var quantity1 = ("1", "blub", "http://unitsofmeasure.org");
        var quantity2 = ("1", "m", "http://unitsofmeasure.org");

        var _ = _service.TryCompare(quantity1, quantity2, out var comparison);

        comparison.Should().BeNull();
    }
    
    // [Fact] This test should work in the new implementation, but not in the old one
    // public void MetricService_Compare_ShouldReturnCorrectForNonUcumCompare()
    // {
    //     var quantity1 = ("1", "blub", "vlorg.org");
    //     var quantity2 = ("1", "blub", "vlorg.org");
    //
    //     var _ = _service.TryCompare(quantity1, quantity2, out var comparison);
    //
    //     comparison.Should().NotBeNull();
    //     comparison.Should().Be(0);
    // }
}