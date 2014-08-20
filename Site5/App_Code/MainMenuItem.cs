using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for MainMenuItem
/// </summary>
[Serializable]
public class MainMenuItem
{
	public MainMenuItem()
	{
        Children = new MainMenuItem[0];
	}

    // FromXml
    // ToXml

    public string ResourceKey { get; set; }
    public int UserLevel { get; set; }
    public string Permissions { get; set; }
    public string ImageUrl { get; set; }
    public string NavigateUrl { get; set; }

    // TODO: INSERTION POINT (for plugins)

    public MainMenuItem[] Children { get; set; }

}