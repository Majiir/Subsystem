namespace Subsystem.Patch
{
    public class PowerSystemAttributesPatch
    {
        PowerSystemType PowerSystemType { get; set; }
        int StartingPowerLevelIndex { get; set; }
        int StartingMaxPowerLevelIndex { get; set; }
        PowerLevelAttributes[] PowerLevels { get; set; }
        PowerSystemViewAttributes View { get; set; }
    }
}
