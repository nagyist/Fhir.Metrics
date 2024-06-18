using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Fhir.Metrics;

/// <summary>
/// Service interface for canonicalization, comparison and arithmetic operations on UCUM quantities. The service uses tuples (string, string, string) to represent quantities.
/// </summary>
public interface IMetricService
{
    /// <summary>
    /// Tries to convert a given quantity to its canonical form.
    /// </summary>
    /// <param name="quantity">the quantity to convert</param>
    /// <param name="canonical">the canonicalized quantity, or null if the operation failed</param>
    /// <returns>true if the canonicalization succeeded, false otherwise</returns>
    public bool TryCanonicalize((string value, string unit, string codesystem) quantity, [NotNullWhen(true)] out (string value, string unit, string codesystem)? canonical);
    
    /// <summary>
    /// Tries to divide quantity 1 by quantity 2. This will also perform a division of the units. 
    /// </summary>
    /// <param name="quantity1">the quantity to perform the operation on</param>
    /// <param name="quantity2">the quantity to divide the first quantity by</param>
    /// <param name="result">the canonical form of the result of the division, or null if the operation failed</param>
    /// <returns>true if the operation succeeded, false otherwise</returns>
    public bool TryDivide((string value, string unit, string codesystem) quantity1, (string value, string unit, string codesystem) quantity2, [NotNullWhen(true)] out (string value, string unit, string codesystem)? result);
    
    /// <summary>
    /// Tries to multiply quantity 1 with quantity 2. This will also perform a multiplication of the units
    /// </summary>
    /// <param name="quantity1">the quantity to perform the operation on</param>
    /// <param name="quantity2">the quantity to multiply the first quantity by</param>
    /// <param name="result">the result of the multiplication, or null if the operation failed</param>
    /// <returns>true if the operation succeeded, false otherwise</returns>
    public bool TryMultiply((string value, string unit, string codesystem) quantity1, (string value, string unit, string codesystem) quantity2, [NotNullWhen(true)] out (string value, string unit, string codesystem)? result);
    
    /// <summary>
    /// Tries to compare quantity 1 with quantity 2. Will fail if the units have different dimensions.
    /// </summary>
    /// <param name="quantity1"></param>
    /// <param name="quantity2"></param>
    /// <param name="result">&lt;0 if quantity1 &lt; quantity2, 0 if quantity1 == quantity2 (within error margin), &gt;0 if quantity1 &gt; quantity2, or null if the operands could not be compared</param>
    /// <returns>true if the operation succeeded, false otherwise</returns>
    public bool TryCompare((string value, string unit, string codesystem) quantity1, (string value, string unit, string codesystem) quantity2, [NotNullWhen(true)] out int? result);
    
    
    // from here on - not yet implemented in the old one
    
    /// <summary>
    /// Tries to convert quantity 1 to the target unit. Will fail if the units have different dimensions, or if the target unit could not be found.
    /// </summary>
    /// <param name="quantity">the quantity to convert</param>
    /// <param name="targetUnit">the target unit</param>
    /// <param name="converted">the result if the conversion, or null if the operation failed</param>
    /// <returns>true if the conversion succeeded, false otherwise</returns>
    public bool TryConvertTo((string value, string unit, string codesystem) quantity, string targetUnit, [NotNullWhen(true)] out (string value, string unit, string codesystem)? converted);
    
    /// <summary>
    /// Tries to subtract quantity 2 from quantity 1. Will fail if the units have different dimensions.
    /// </summary>
    /// <param name="quantity1">the quantity to subtract from</param>
    /// <param name="quantity2">the quantity to subtract from the first operand</param>
    /// <param name="result">the result of the subtraction in canonical form, or null if the operation failed</param>
    /// <returns>true if the operation succeeded, false otherwise</returns>
    public bool TrySubtract((string value, string unit, string codesystem) quantity1, (string value, string unit, string codesystem) quantity2, [NotNullWhen(true)] out (string value, string unit, string codesystem)? result);
    
    /// <summary>
    /// Tries to add quantity 2 to quantity 1. Will fail if the units have different dimensions.
    /// </summary>
    /// <param name="quantity1">the quantity to add to</param>
    /// <param name="quantity2">the quantity to add to the first operand</param>
    /// <param name="result">the result of the addition in canonical form, or null if the operation failed</param>
    /// <returns>true if the operation succeeded, false otherwise</returns>
    public bool TryAdd((string value, string unit, string codesystem) quantity1, (string value, string unit, string codesystem) quantity2, [NotNullWhen(true)] out (string value, string unit, string codesystem)? result);
}

public static class MetricServiceExtensions
{
    // same methods, but with decimal overload
    
    public static bool TryCanonicalize(this IMetricService service, (decimal value, string unit, string codesystem) quantity, [NotNullWhen(true)] out (decimal value, string unit, string codesystem)? canonical)
    {
        if(!service.TryCanonicalize((quantity.value.ToString(CultureInfo.InvariantCulture), quantity.unit, quantity.codesystem), out var canonicalTuple))
        {
            canonical = null;
            return false;
        }

        try
        {
            canonical = (decimal.Parse(canonicalTuple.Value.value, CultureInfo.InvariantCulture), canonicalTuple.Value.unit, canonicalTuple.Value.codesystem);
        }
        catch
        {
            canonical = null;
            return false;
        }

        return true;
    }
    
    public static bool TryDivide(this IMetricService service, (decimal value, string unit, string codesystem) quantity1, (decimal value, string unit, string codesystem) quantity2, [NotNullWhen(true)] out (decimal value, string unit, string codesystem)? result)
    {
        if(!service.TryDivide((quantity1.value.ToString(CultureInfo.InvariantCulture), quantity1.unit, quantity1.codesystem), (quantity2.value.ToString(CultureInfo.InvariantCulture), quantity2.unit, quantity2.codesystem), out var resultTuple))
        {
            result = null;
            return false;
        }

        try
        {
            result = (decimal.Parse(resultTuple.Value.value, CultureInfo.InvariantCulture), resultTuple.Value.unit, resultTuple.Value.codesystem);
        }
        catch
        {
            result = null;
            return false;
        }

        return true;
    }
    
    public static bool TryMultiply(this IMetricService service, (decimal value, string unit, string codesystem) quantity1, (decimal value, string unit, string codesystem) quantity2, [NotNullWhen(true)] out (decimal value, string unit, string codesystem)? result)
    {
        if(!service.TryMultiply((quantity1.value.ToString(CultureInfo.InvariantCulture), quantity1.unit, quantity1.codesystem), (quantity2.value.ToString(CultureInfo.InvariantCulture), quantity2.unit, quantity2.codesystem), out var resultTuple))
        {
            result = null;
            return false;
        }

        try
        {
            result = (decimal.Parse(resultTuple.Value.value, CultureInfo.InvariantCulture), resultTuple.Value.unit, resultTuple.Value.codesystem);
        }
        catch
        {
            result = null;
            return false;
        }

        return true;
    }
    
    public static bool TryCompare(this IMetricService service, (decimal value, string unit, string codesystem) quantity1, (decimal value, string unit, string codesystem) quantity2, [NotNullWhen(true)] out int? result)
    {
        if(!service.TryCompare((quantity1.value.ToString(CultureInfo.InvariantCulture), quantity1.unit, quantity1.codesystem), (quantity2.value.ToString(CultureInfo.InvariantCulture), quantity2.unit, quantity2.codesystem), out var resultInt))
        {
            result = null;
            return false;
        }

        result = resultInt;
        return true;
    }
    
    public static bool TryConvertTo(this IMetricService service, (decimal value, string unit, string codesystem) quantity, string targetUnit, [NotNullWhen(true)] out (decimal value, string unit, string codesystem)? converted)
    {
        if(!service.TryConvertTo((quantity.value.ToString(CultureInfo.InvariantCulture), quantity.unit, quantity.codesystem), targetUnit, out var convertedTuple))
        {
            converted = null;
            return false;
        }

        try
        {
            converted = (decimal.Parse(convertedTuple.Value.value, CultureInfo.InvariantCulture), convertedTuple.Value.unit, convertedTuple.Value.codesystem);
        }
        catch
        {
            converted = null;
            return false;
        }

        return true;
    }
    
    public static bool TrySubtract(this IMetricService service, (decimal value, string unit, string codesystem) quantity1, (decimal value, string unit, string codesystem) quantity2, [NotNullWhen(true)] out (decimal value, string unit, string codesystem)? result)
    {
        if(!service.TrySubtract((quantity1.value.ToString(CultureInfo.InvariantCulture), quantity1.unit, quantity1.codesystem), (quantity2.value.ToString(CultureInfo.InvariantCulture), quantity2.unit, quantity2.codesystem), out var resultTuple))
        {
            result = null;
            return false;
        }

        try
        {
            result = (decimal.Parse(resultTuple.Value.value, CultureInfo.InvariantCulture), resultTuple.Value.unit, resultTuple.Value.codesystem);
        }
        catch
        {
            result = null;
            return false;
        }

        return true;
    }
    
    public static bool TryAdd(this IMetricService service, (decimal value, string unit, string codesystem) quantity1, (decimal value, string unit, string codesystem) quantity2, [NotNullWhen(true)] out (decimal value, string unit, string codesystem)? result)
    {
        if(!service.TryAdd((quantity1.value.ToString(CultureInfo.InvariantCulture), quantity1.unit, quantity1.codesystem), (quantity2.value.ToString(CultureInfo.InvariantCulture), quantity2.unit, quantity2.codesystem), out var resultTuple))
        {
            result = null;
            return false;
        }

        try
        {
            result = (decimal.Parse(resultTuple.Value.value, CultureInfo.InvariantCulture), resultTuple.Value.unit, resultTuple.Value.codesystem);
        }
        catch
        {
            result = null;
            return false;
        }

        return true;
    }
}