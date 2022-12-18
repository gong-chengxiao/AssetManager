
namespace AssetManager.Helpers;

public static class Strings
{
    private static readonly string _appDisplayName = "AppDisplayName".GetLocalized();

    private static readonly string _assetTypeText = "AssetTypeText".GetLocalized();
    private static readonly string _assetType_Big = "AssetType_Big".GetLocalized();
    private static readonly string _assetType_Mid = "AssetType_Mid".GetLocalized();
    private static readonly string _assetType_Small = "AssetType_Small".GetLocalized();

    private static readonly string _schoolOfComputerName            = "School of Computer".GetLocalized();
    private static readonly string _schoolOfElectricalName          = "School of Electrical".GetLocalized();
    private static readonly string _schoolOfAeronauticsName         = "School of Aeronautics".GetLocalized();
    private static readonly string _departmentOfPublicityName       = "Department of Publicity".GetLocalized();
    private static readonly string _generalOfficeOfSchoolName       = "General Office of School".GetLocalized();
    private static readonly string _engineeringTrainingCenterName   = "Engineering Training Center".GetLocalized();
    private static readonly string _libraryName                     = "Library".GetLocalized();
    private static readonly string _academicAffairsOfficeName       = "Academic Affairs Office".GetLocalized();

    private static readonly string _userNameText = "Username".GetLocalized();
    private static readonly string _passwordText = "Password".GetLocalized();
    
    private static readonly string _loginText = "Log in".GetLocalized();
    private static readonly string _useOtherDbText = "Use Other Database".GetLocalized();

    private static readonly string _hostAddressText = "Host".GetLocalized();
    private static readonly string _portText = "Port".GetLocalized();
    private static readonly string _dbNameText = "Database Name".GetLocalized();

    // 用户表
    private static readonly string _userIdText = "ID".GetLocalized();
    private static readonly string _nameText = "PersonName".GetLocalized();
    private static readonly string _DepartmentText = "Department".GetLocalized();
    private static readonly string _PhonenumberText = "Phonenumber".GetLocalized();
    public static string UserIdText => _userIdText;
    public static string NameText => _nameText;
    public static string DepartmentText => _DepartmentText;
    public static string PhonenumberText => _PhonenumberText;


    public static string AppDisplayName => _appDisplayName;

    public static string AssetTypeText => _assetTypeText;
    public static string AssetType_Big => _assetType_Big;
    public static string AssetType_Mid => _assetType_Mid;
    public static string AssetType_Small => _assetType_Small;

    public static string SchoolOfComputerName => _schoolOfComputerName;
    public static string SchoolOfElectricalName => _schoolOfElectricalName;
    public static string SchoolOfAeronauticsName => _schoolOfAeronauticsName;
    public static string DepartmentOfPublicityName => _departmentOfPublicityName;
    public static string GeneralOfficeOfSchoolName => _generalOfficeOfSchoolName;
    public static string EngineeringTrainingCenterName => _engineeringTrainingCenterName;
    public static string LibraryName => _libraryName;
    public static string AcademicAffairsOfficeName => _academicAffairsOfficeName;

    public static string UserNameText => _userNameText;
    public static string PasswordText => _passwordText;

    public static string LoginText => _loginText;

    public static string UseOtherDbText => _useOtherDbText;

    public static string HostAddressText => _hostAddressText;
    public static string PortText => _portText;
    public static string DbNameText => _dbNameText;
    

}
