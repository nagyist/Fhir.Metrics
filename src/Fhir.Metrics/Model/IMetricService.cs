using System.Diagnostics.CodeAnalysis;

namespace Fhir.Metrics;

public interface IMetricService
{
    public bool TryCanonicalize((string value, string unit, string codesystem) quantity, [NotNullWhen(true)] out (string value, string unit, string codesystem)? canonical);
    
    public bool TryDivide((string value, string unit, string codesystem) quantity1, (string value, string unit, string codesystem) quantity2, [NotNullWhen(true)] out (string value, string unit, string codesystem)? result);
    
    public bool TryMultiply((string value, string unit, string codesystem) quantity1, (string value, string unit, string codesystem) quantity2, [NotNullWhen(true)] out (string value, string unit, string codesystem)? result);
    
    public bool TryCompare((string value, string unit, string codesystem) quantity1, (string value, string unit, string codesystem) quantity2, [NotNullWhen(true)] out int? result);
    
    
    // from here on - not yet implemented in the old one
    
    public bool TryConvertTo((string value, string unit, string codesystem) quantity, string targetUnit, [NotNullWhen(true)] out (string value, string unit, string codesystem)? converted);
    
    public bool TrySubtract((string value, string unit, string codesystem) quantity1, (string value, string unit, string codesystem) quantity2, [NotNullWhen(true)] out (string value, string unit, string codesystem)? result);
    
    public bool TryAdd((string value, string unit, string codesystem) quantity1, (string value, string unit, string codesystem) quantity2, [NotNullWhen(true)] out (string value, string unit, string codesystem)? result);
}