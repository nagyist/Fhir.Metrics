namespace Fhir.Metrics;

public interface IMetricService
{
    public bool TryCanonicalize((string value, string unit, string codesystem) quantity, out (string value, string unit, string codesystem, bool isUcum) canonical);
    
    public bool TryCompare((string value, string unit, string codesystem) quantity1, (string value, string unit, string codesystem) quantity2, out int result);
    
    public bool TryConvertTo((string value, string unit, string codesystem) quantity, string targetUnit, out (string value, string unit, string codesystem) converted);
    
    public bool TryDivide((string value1, string unit1, string codesystem1) quantity1, (string value2, string unit2, string codesystem2) quantity2, out (string value, string unit, string codesystem) result);
    
    public bool TryMultiply((string value1, string unit1, string codesystem1) quantity1, (string value2, string unit2, string codesystem2) quantity2, out (string value, string unit, string codesystem) result);
    
    public bool TrySubtract((string value1, string unit1, string codesystem1) quantity1, (string value2, string unit2, string codesystem2) quantity2, out (string value, string unit, string codesystem) result);
    
    public bool TryAdd((string value1, string unit1, string codesystem1) quantity1, (string value2, string unit2, string codesystem2) quantity2, out (string value, string unit, string codesystem) result);
}