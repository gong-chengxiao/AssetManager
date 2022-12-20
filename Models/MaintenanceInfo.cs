namespace AssetManager.Models;
public class MaintenanceInfo
{
    public int MaintenanceID
    {
        get; set;
    }
    public int AssetId
    {
        get; set;
    }
    public string AssetName
    {
        get; set;
    }
    public int InChargePersonID
    {
        get; set;
    }
    public string InChargePersonName
    {
        get; set;
    }
    public string MaintenanceContent
    {
        get; set;
    } = "";
    public byte IsNormal
    {
        get; set;
    }
    public string MaintenanceDate
    {
        get; set;
    }
    public string NextMaintenanceDate
    {
        get; set;
    }
    public string Department
    {
        get; set;
    }
}
