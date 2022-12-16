using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Helpers;
public class LoginForm
{
    public string Host
    {
        get; set;
    }
    public string Port
    {
        get; set;
    }
    public string DbName
    {
        get; set;
    }

    public string Username
    {
        get; set;
    }
    public string Password
    {
        get; set;
    }

    public bool IsUseOtherDb
    {
        get; set;
    }
}
