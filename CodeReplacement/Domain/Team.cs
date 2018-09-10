using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Team
{
    private string TeamName;
    private string TeamURL;

    #region Constructors
    public Team()
    {
        TeamName = string.Empty;
        TeamURL = string.Empty;
    }

    public Team(string _TeamName, string _URL)
    {
        TeamName = _TeamName;
        TeamURL = _URL;
    }
    #endregion

    #region Properties
    public string Name
    {
        get { return TeamName; }
        set { TeamName = value; }
    }

    public string URL
    {
        get { return TeamURL; }
        set { TeamURL = value; }
    }
    #endregion
}
