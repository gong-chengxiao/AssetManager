namespace AssetManager.Models;
public class SchoolAsset
{
    public int AssetID
    {
        get; set;
    }

    public string AssetName
    {
        get; set;
    }

    public string AssetSpecification
    {
        get; set;
    }

    public string AssetType
    {
        get; set;
    }

    public string AssetSerialNumber
    {
        get; set;
    }

    public string AssetPurchaseDate
    {
        get; set;
    }

    public double AssetPurchasePrice
    {
        get; set;
    }

    public string AssetPurchaseOrderNumber
    {
        get; set;
    }

    public string AssetVendorName
    {
        get; set;
    }

    // 领用人ID
    public string GetterName
    {
        get; set;
    }

    public string UserName
    {
        get; set;
    }
    
    public string DepartmentName
    {
        get; set;
    }

    public string ShortDescription => $"{AssetID}: {AssetName}";
}
