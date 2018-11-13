using System;
using System.IO;
using System.Net;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using CodeReplacement.Constants;
using log4net;
using HtmlAgilityPack;
using System.Security.Permissions;
using System.Linq;

/// <summary>
/// Summary description for CodeReplacements
/// </summary>
public class CodeReplacements
{
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	public CodeReplacements()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string CreatePlayerRosterText(string SelectedHomeTeamName, string HomeTeamURL, string SelectedVisitingTeamName, string VisitingTeamURL, string HomeTeamPrefix, string VisitingTeamPrefix, string HomeTeamAlternateDescription, string VisitingTeamAlternateDescription, string SelectedSport, string DuplicateTextDescription, string HomeTeamCoach, string VisitingTeamCoach, bool IncludeHelpfulCodes)
    {
        string returnValue = String.Empty;

        var HomeTeamName = DetermineTeamName(SelectedHomeTeamName/*.SelectedItem.Text.ToString()*/, HomeTeamAlternateDescription);
        var VisitingTeamName = DetermineTeamName(SelectedVisitingTeamName/*.SelectedItem.Text.ToString()*/, VisitingTeamAlternateDescription);
        var PlayerSport = GetEnumForSport(SelectedSport);

        IList<Player> bothteams = GetPlayers(HomeTeamURL, VisitingTeamURL, HomeTeamPrefix, VisitingTeamPrefix, HomeTeamName, VisitingTeamName, PlayerSport, HomeTeamCoach, VisitingTeamCoach);
        for (int Index = 0; Index < bothteams.Count; Index++)
        {
            Player p = bothteams[Index];
            returnValue += p.Prefix + '\t' + p.PlayerNumber + '\t' + p.PlayerName + '\t' + p.PlayerPosition + "\r\n";
        }

        return returnValue;
    }


    public string CreateCustomFile(string customRoster, string SelectedHomeTeamName, string HomeTeamURL, string SelectedVisitingTeamName, string VisitingTeamURL, string HomeTeamPrefix, string VisitingTeamPrefix, string HomeTeamAlternateDescription, string VisitingTeamAlternateDescription, string SelectedSport, string DuplicateTextDescription, string HomeTeamCoach, string VisitingTeamCoach, bool IncludeHelpfulCodes)
    {
        var HomeTeamName = DetermineTeamName(SelectedHomeTeamName/*.SelectedItem.Text.ToString()*/, HomeTeamAlternateDescription);
        var VisitingTeamName = DetermineTeamName(SelectedVisitingTeamName/*.SelectedItem.Text.ToString()*/, VisitingTeamAlternateDescription);
        Constants.Sports PlayerSport = GetEnumForSport(SelectedSport);

        // Parse Custom Roster into IList of Players
        IList<Player> bothteams = new List<Player>();
        CreatePlayerListFromCustomRosterText(customRoster, bothteams, HomeTeamPrefix, HomeTeamName, VisitingTeamPrefix, VisitingTeamName, PlayerSport);

        var FileName = CreateFileFromPlayerList(bothteams, HomeTeamName, HomeTeamURL, VisitingTeamName, VisitingTeamURL, HomeTeamPrefix, VisitingTeamPrefix, HomeTeamAlternateDescription, VisitingTeamAlternateDescription, PlayerSport, DuplicateTextDescription, HomeTeamCoach, VisitingTeamCoach, IncludeHelpfulCodes);

        return FileName;
    }

    private static void CreatePlayerListFromCustomRosterText(string customRoster, IList<Player> bothteams, string HomeTeamPrefix, string HomeTeamName, string VisitingTeamPrefix, string VisitingTeamName, Constants.Sports PlayerSport)
    {
        var customPlayers = customRoster.Split('\n');
        foreach (string customLine in customPlayers)
        {
            var p = new Player();
            var customPlayer = customLine.Split('\t');
            if (customPlayer.Count() == 4)
            {
                if ((customPlayer[0] == HomeTeamPrefix) || (customPlayer[0] == VisitingTeamPrefix))
                {
                    p.Prefix = customPlayer[0];
                    p.PlayerNumber = customPlayer[1];
                    p.PlayerName = customPlayer[2];
                    p.PlayerPosition = customPlayer[3].TrimEnd('\r');
                    p.Team = (p.Prefix == HomeTeamPrefix ? HomeTeamName : VisitingTeamName);
                    p.PlayersSport = PlayerSport;

                    bothteams.Add(p);
                }
            }
        }

    }

    /// <summary>
    /// Create the file in the folder defined in the Web.config file
    /// </summary>
    /// <param name="HomeTeamDropDown"></param>
    /// <param name="VisitingTeamDropDown"></param>
    /// <param name="HomeTeamPrefix"></param>
    /// <param name="VisitingTeamPrefix"></param>
//    public string CreateFile(DropDownList HomeTeamDropDown, DropDownList VisitingTeamDropDown, string HomeTeamPrefix, string VisitingTeamPrefix, string HomeTeamAlternateDescription, string VisitingTeamAlternateDescription, Constants.Sports PlayerSport, string DuplicateTextDescription, string HomeTeamCoach, string VisitingTeamCoach, bool IncludeHelpfulCodes)
    public string CreateFile(string HomeTeam, string HomeTeamURL, string VisitingTeam, string VisitingTeamURL, string HomeTeamPrefix, string VisitingTeamPrefix, string HomeTeamAlternateDescription, string VisitingTeamAlternateDescription, Constants.Sports PlayerSport, string DuplicateTextDescription, string HomeTeamCoach, string VisitingTeamCoach, bool IncludeHelpfulCodes)
    {
        var HomeTeamName = DetermineTeamName(HomeTeam, HomeTeamAlternateDescription);
        var VisitingTeamName = DetermineTeamName(VisitingTeam, VisitingTeamAlternateDescription);

        IList<Player> bothteams = GetPlayers(HomeTeamURL, VisitingTeamURL, HomeTeamPrefix, VisitingTeamPrefix, HomeTeamName, VisitingTeamName, PlayerSport, HomeTeamCoach, VisitingTeamCoach);
        string FileName = CreateFileFromPlayerList(bothteams, HomeTeamName, HomeTeamURL, VisitingTeamName, VisitingTeamURL, HomeTeamPrefix, VisitingTeamPrefix, HomeTeamAlternateDescription, VisitingTeamAlternateDescription, PlayerSport, DuplicateTextDescription, HomeTeamCoach, VisitingTeamCoach, IncludeHelpfulCodes);

        return FileName;
    }

//    private string CreateFileFromPlayerList(IList<Player> bothteams, DropDownList HomeTeamDropDown, DropDownList VisitingTeamDropDown, string HomeTeamPrefix, string VisitingTeamPrefix, string HomeTeamAlternateDescription, string VisitingTeamAlternateDescription, Constants.Sports PlayerSport, string DuplicateTextDescription, string HomeTeamCoach, string VisitingTeamCoach, bool IncludeHelpfulCodes)
    private string CreateFileFromPlayerList(IList<Player> bothteams, string HomeTeamName, string HomeTeamURL, string VisitingTeamName, string VisitingTeamURL, string HomeTeamPrefix, string VisitingTeamPrefix, string HomeTeamAlternateDescription, string VisitingTeamAlternateDescription, Constants.Sports PlayerSport, string DuplicateTextDescription, string HomeTeamCoach, string VisitingTeamCoach, bool IncludeHelpfulCodes)
    {
        string FileName = string.Empty;
        string RootPath = string.Empty;

        try
        {
            DateTime x = new DateTime();
            x = DateTime.Now;

            if (log.IsDebugEnabled)
            {
                log.Debug(PlayerSport.ToString() + ", Visting: " + VisitingTeamName + " Home: " + HomeTeamName);
                log.Debug("Visting Alt Desc: " + VisitingTeamAlternateDescription + " Home Alt Desc: " + HomeTeamAlternateDescription);
            }

            RootPath = GetFileRootPath("FileLocation");

            FileName = RootPath + VisitingTeamName.Substring(0, 3) + "_v_" + HomeTeamName.Substring(0, 3) + '_' + x.Date.Month + x.Date.Day + x.Year + x.Hour.ToString() + x.Minute.ToString() + x.Second.ToString() + HomeTeamPrefix + VisitingTeamPrefix + ".txt";

            if (!File.Exists(FileName))
            {
                using (StreamWriter sw = File.CreateText(FileName))
                {
//                    var HomeTeamName = DetermineTeamName(HomeTeam, HomeTeamAlternateDescription);
//                    var VisitingTeamName = DetermineTeamName(VisitingTeamDropDown.SelectedItem.Text.ToString(), VisitingTeamAlternateDescription);

                    sw.WriteLine(HomeTeamPrefix + '\t' + HomeTeamName);
                    sw.WriteLine(VisitingTeamPrefix + '\t' + (VisitingTeamAlternateDescription.Trim().Length > 0 ? VisitingTeamAlternateDescription.Trim() : VisitingTeamName));
                    sw.WriteLine("dg" + HomeTeamPrefix + '\t' + "during the game against the " + HomeTeamName);
                    sw.WriteLine("dg" + VisitingTeamPrefix + '\t' + "during the game against the " + (VisitingTeamAlternateDescription.Trim().Length > 0 ? VisitingTeamAlternateDescription.Trim() : VisitingTeamName));
                    sw.WriteLine("a" + HomeTeamPrefix + '\t' + "against the " + HomeTeamName);
                    sw.WriteLine("a" + VisitingTeamPrefix + '\t' + "against the " + (VisitingTeamAlternateDescription.Trim().Length > 0 ? VisitingTeamAlternateDescription.Trim() : VisitingTeamName));

                    if (IncludeHelpfulCodes)
                    {
                        IncludeUsefullCodes(PlayerSport, sw);
                    }

                    for (int Index = 0; Index < bothteams.Count; Index++)
                    {
                        string FormattedNumber;

                        Player p = bothteams[Index];

                        bool DuplicateFound = false;
                        string DuplicateText = string.Empty;
                        // if we're not at the last record, there could be a duplicate player number in the next line
                        if (Index < bothteams.Count - 1)
                        {
                            if ((bothteams[Index + 1].Team == p.Team) && (bothteams[Index + 1].PlayerNumber == p.PlayerNumber) && (bothteams[Index + 1].PlayerName != p.PlayerName) && ((bothteams[Index + 1].OffenseDefenseFlag == p.OffenseDefenseFlag)))
                            {
                                DuplicateFound = true;
                                DuplicateText = (DuplicateTextDescription.Length == 0 ? "@@@@Duplicate " : DuplicateTextDescription);
                            }
                        }

                        FormattedNumber = (string.IsNullOrEmpty(p.PlayerNumber) || p.PlayerNumber == Constants.HEAD_COACH_JERSEY_NUMBER) ? string.Empty : " (" + p.PlayerNumber + ')';

                        sw.WriteLine(p.Prefix + p.PlayerNumber + p.OffenseDefenseFlag + '\t' + DuplicateText + p.PlayerName + (DuplicateFound ? " @@@@" + bothteams[Index + 1].PlayerName : string.Empty));                                      // g10  Matt Flynn
                        sw.WriteLine('t' + p.Prefix + p.PlayerNumber + p.OffenseDefenseFlag + '\t' + DuplicateText + p.Team + (p.Team.EndsWith("s") || p.Team.EndsWith("es") ? "' " : "'s ") + p.PlayerName + (DuplicateFound ? " @@@@" + bothteams[Index + 1].Team + (p.Team.EndsWith("s") || p.Team.EndsWith("es") ? "' " : "'s ") + bothteams[Index + 1].PlayerName : string.Empty));                // tg10     Green Bay Packers' Matt Flynn
                        sw.WriteLine('t' + p.Prefix + p.PlayerNumber + p.OffenseDefenseFlag + "n\t" + DuplicateText + p.Team + (p.Team.EndsWith("s") || p.Team.EndsWith("es") ? "' " : "'s ") + p.PlayerName + FormattedNumber + (DuplicateFound ? " @@@@" + bothteams[Index + 1].Team + (p.Team.EndsWith("s") || p.Team.EndsWith("es") ? "' " : "'s ") + bothteams[Index + 1].PlayerName + FormattedNumber : string.Empty));                // tg10n     Green Bay Packers' Matt Flynn (10)
                        sw.WriteLine(p.Prefix + p.PlayerNumber + 'n' + p.OffenseDefenseFlag + '\t' + DuplicateText + p.PlayerName + FormattedNumber + (DuplicateFound ? " @@@@" + bothteams[Index + 1].PlayerName + FormattedNumber : string.Empty)); // g10n  Matt Flynn (10)
                        sw.WriteLine("tp" + p.Prefix + p.PlayerNumber + p.OffenseDefenseFlag + '\t' + DuplicateText + p.Team + ' ' + p.PositionDescription + ' ' + p.PlayerName + (DuplicateFound ? " @@@@" + bothteams[Index + 1].Team + ' ' + bothteams[Index + 1].PositionDescription + ' ' + bothteams[Index + 1].PlayerName : string.Empty));
                        sw.WriteLine("p" + p.Prefix + p.PlayerNumber + p.OffenseDefenseFlag + '\t' + DuplicateText + p.PositionDescription + ' ' + p.PlayerName + (DuplicateFound ? " @@@@" + bothteams[Index + 1].PositionDescription + ' ' + bothteams[Index + 1].PlayerName : string.Empty));   // pg10
                        sw.WriteLine("tp" + p.Prefix + p.PlayerNumber + "n" + p.OffenseDefenseFlag + "\t" + DuplicateText + p.Team + ' ' + p.PositionDescription + ' ' + p.PlayerName + FormattedNumber + (DuplicateFound ? " @@@@" + bothteams[Index + 1].Team + ' ' + bothteams[Index + 1].PositionDescription + ' ' + bothteams[Index + 1].PlayerName + FormattedNumber : string.Empty));
                        sw.WriteLine("p" + p.Prefix + p.PlayerNumber + "n" + p.OffenseDefenseFlag + "\t" + DuplicateText + p.PositionDescription + ' ' + p.PlayerName + FormattedNumber + (DuplicateFound ? " @@@@" + bothteams[Index + 1].PositionDescription + ' ' + bothteams[Index + 1].PlayerName + FormattedNumber : string.Empty));   // pg10n

                        if (DuplicateFound)
                            Index++;
                    }

                    sw.Close();
                }

            }
            else
            {
                log.Debug("Duplicate File Name already exists: " + FileName);
                FileName = string.Empty;
            }

        }
        catch (Exception FileException)
        {
            log.Error(FileException.Message.ToString());
            FileName = string.Empty;
        }

        CleanupOldFiles();
        return FileName;
    }

    /// <summary>
    /// Build the team name, given the values selected from the screen
    /// </summary>
    /// <param name="SelectedTeamName">Team name selected from the drop down on the main screen</param>
    /// <param name="AlternateTeamName">Team Name entered by the user</param>
    /// <returns></returns>
    private string DetermineTeamName(string SelectedTeamName, string AlternateTeamName)
    {
        var TeamName = SelectedTeamName;
        if ((!string.IsNullOrEmpty(AlternateTeamName)) && (AlternateTeamName.Trim().Length > 0) && (!SelectedTeamName.Equals(AlternateTeamName)))
        {
            TeamName = AlternateTeamName;
        }

        return TeamName;
    }

    /// <summary>
    /// Cleanup all the text files that are more than X minutes old
    /// 
    /// Cleanup will default to clean up files more than 30 minutes old
    /// </summary>
    private void CleanupOldFiles()
    {
        string RootPath = GetFileRootPath("FileLocation");

        // Determine the time period for which we will keep old files around
        int FileCleanupDuration;
        try
        {
            string FileDuration = ConfigurationManager.AppSettings["FileCleanupDuration"];
            FileCleanupDuration = Convert.ToInt32(FileDuration);

            if (FileCleanupDuration > 0)
                FileCleanupDuration = -1 * FileCleanupDuration;
        }
        catch (Exception x)
        {
            log.Error(x.Message.ToString());
            // Default the cleanup time to 30 minutes
            FileCleanupDuration = -30;
        }

        // get all the text files already existing in our folder, and remove any of them
        // that are more than our configured timeframe
        string [] files = Directory.GetFiles(RootPath, "*.txt");
        foreach(string f in files)
        {
            var x = new FileInfo(f);
            if (x.CreationTime < DateTime.Now.AddMinutes(FileCleanupDuration))
            {
                File.Delete(f);
            }
        }
    }

    /// <summary>
    /// Gets the RootPath element from the web.config file if it exists.  Makes sure to always end the root path with a \,
    /// and defaults the root path to C:\Temp\ if one does not exist in the web.config file
    /// </summary>
    /// <returns></returns>
    private static string GetFileRootPath(string Setting)
    {
        string RootPath = ConfigurationManager.AppSettings[Setting];

        if ((RootPath == string.Empty) || (RootPath == null))
        {
            RootPath = "C:\\Temp\\";
        }

        if (!RootPath.EndsWith("\\"))
            RootPath += "\\";

        if (!RootPath.StartsWith("C:"))
        {
            // if we are not running on the local filesystem, get the full path of the current folder and append the root path from web.config to it
            RootPath = System.Web.HttpContext.Current.Server.MapPath("") + RootPath;
        }

        return RootPath;
    }

    /// <summary>
    /// For both teams selected in the dropdown lists, get the rosters from the defined site, then
    /// call the ParseRoster method to create the list of Players
    /// </summary>
    /// <param name="HomeTeamDropDown"></param>
    /// <param name="VisitingTeamDropDown"></param>
    /// <param name="HomeTeamPrefix"></param>
    /// <param name="VisitingTeamPrefix"></param>
    /// <returns></returns>
    private List<Player> GetPlayers(string HomeTeamDropDown, string VisitingTeamDropDown, string HomeTeamPrefix, string VisitingTeamPrefix, string HomeTeamName, string VisitingTeamName, Constants.Sports CurrentSport, string HomeTeamCoach, string VisitingTeamCoach)
    {

        List<Player> PlayerList = new List<Player>();

        WebClient GetFile = new WebClient();
        System.Text.Encoding enc = System.Text.Encoding.ASCII;

        try
        {
            // get the selected home teams roster from the configured location
            byte[] HomeTeam = GetFile.DownloadData(BuildTeamURL(HomeTeamDropDown, CurrentSport));
            string HomeTeamRoster = enc.GetString(HomeTeam);

            ParseRoster_ESPN(PlayerList, HomeTeamRoster, HomeTeamName, HomeTeamPrefix, CurrentSport, HomeTeamCoach);

        }
        catch (Exception ex)
        {
            string s = ex.Message.ToString();
        }

        try
        {
            // get the selected visiting teams roster from the configured location
            byte[] VisitingTeam = GetFile.DownloadData(BuildTeamURL(VisitingTeamDropDown, CurrentSport));
            string VisitingTeamRoster = enc.GetString(VisitingTeam);

            ParseRoster_ESPN(PlayerList, VisitingTeamRoster, VisitingTeamName, VisitingTeamPrefix, CurrentSport, VisitingTeamCoach);
        }
        catch (Exception ex)
        {
            string s = ex.Message.ToString();
        }

        PlayerList.Sort(PlayerCompare);

        return PlayerList;
    }


    /// <summary>
    ///  For each given sport, build the url for the team in the selected dropdown list
    /// </summary>
    /// <param name="TeamDropDown"></param>
    /// <param name="CurrentSport"></param>
    /// <returns></returns>
    private string BuildTeamURL(string TeamDropDown, Constants.Sports CurrentSport)
    {
        string URL = string.Empty;

        if (TeamDropDown.StartsWith("http:"))
        {
            URL = TeamDropDown;
        }
        else
        {
            switch (CurrentSport)
            {
                case Constants.Sports.NCAAFootball:
                case Constants.Sports.NCAAMensBasketball:
                case Constants.Sports.NBA:
                case Constants.Sports.NFL:
                case Constants.Sports.MLB:
                    URL = "http://espn.go.com" + TeamDropDown;
                    break;
                default:
                    string rosterurl = string.Empty;
                    string selectedteamurl = TeamDropDown;
                    int lastslashpos = selectedteamurl.LastIndexOf('/');
                    if (lastslashpos > 0)
                    {
                        rosterurl = selectedteamurl.Substring(0, lastslashpos) + "/roster/" + selectedteamurl.Substring(lastslashpos + 1);
                    }

                    URL = "http://msn.foxsports.com/" + rosterurl.Substring(1);

                    break;

            }
        }

        return URL;

    }


    /// <summary>
    /// Parse the roster contained in the Roster string into a list of Player objects, returned in PlayerList
    /// 
    /// http://aspnetlibrary.com/articledetails.aspx?article=Convert-HTML-tables-to-a-dataset
    /// </summary>
    /// <param name="PlayerList">output, iList of player objects created from the Roster</param>
    /// <param name="Roster">input, string contining the roster data</param>
    /// <param name="TeamName">input, full text description of the Team Name to attach to players</param>
    /// <param name="TeamPrefix">input, prefix defined by the user to use for creating team codes</param>
    private void ParseRoster(List<Player> PlayerList, string Roster, string TeamName, string TeamPrefix, Constants.Sports CurrentSport)
    {
        
        DataTable  dt;
        DataRow dr;
//        DataColumn dc;
        string TableExpression = "<table[^>]*>(.*?)</table>";
        string HeaderExpression = "<th[^>]*>(.*?)</th>";
        string RowExpression = "<tr[^>]*>(.*?)</tr>";
        string ColumnExpression = "<td[^>]*>(.*?)</td>";
        bool HeadersExist = false;
        int iCurrentColumn = 0;  
        int iCurrentRow = 0;

        string s = Roster;

        // Get a match for all the tables in the HTML  
        MatchCollection Tables = Regex.Matches(s, TableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        string a = Tables[0].ToString();

        // we have nested tables, so remove the outer table header so we can get at the inner one
        int i = a.LastIndexOf("<table");
        a = a.Substring(i - 1);

        // foxsports puts a single row at the beginning of the table for the text ROSTER, strip that out so we can find the column headers
        int b = a.IndexOf("<tr");
        int c = a.IndexOf("/tr>");
        a = a.Substring(0, (b - 1)) + a.Substring(c + 1);

        MatchCollection nestedTables = Regex.Matches(a, TableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        //Loop through each table element 
        foreach (Match Table in nestedTables)
        {
            iCurrentRow = 0;
            HeadersExist = false;

            // Add a new table to the DataSet  
            dt = new DataTable();

            // Create the relevant amount of columns for this table (use the headers if they exist, otherwise use default names)  
            if (Table.Value.Contains("<th"))
            {
                HeadersExist = true;
                // Get a match for all the rows in the table
                MatchCollection Headers = Regex.Matches(Table.Value, HeaderExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                // Loop through each header element  
                foreach (Match Header in Headers)
                {
                    dt.Columns.Add(Header.Groups[1].ToString());
                }
            }
            else
            {
                for (int iColumns = 1; iColumns <= Regex.Matches(Regex.Matches(Regex.Matches(Table.Value, TableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase)[0].ToString(), RowExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase)[0].ToString(), ColumnExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase).Count; iColumns++)
                {
                    dt.Columns.Add("Column " + iColumns);
                }
            }


         // Get a match for all the rows in the table  
         MatchCollection Rows = Regex.Matches(Table.Value, RowExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);
   
         // Loop through each row element  
         foreach (Match Row in Rows)
         {
   
             // Only loop through the row if it isn't a header row  
             if (!((iCurrentRow == 0) && (HeadersExist == true)))
             {
   
                 // Create a new row and reset the current column counter  
                 dr = dt.NewRow();
                 iCurrentColumn = 0;
   
                 // Get a match for all the columns in the row  
                 MatchCollection Columns = Regex.Matches(Row.Value, ColumnExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);
   
                 // Loop through each column element  
                 foreach (Match Column in Columns)
                 {
   
                     // Add the value to the DataRow  
                     dr[iCurrentColumn] = Column.Groups[1].ToString();
   
                     // Increase the current column   
                     iCurrentColumn += 1;
                 }
   
                 // Add the DataRow to the DataTable  
                 dt.Rows.Add(dr);
   
             }  
   
             // Increase the current row counter  
             iCurrentRow += 1;
         }


         foreach (DataRow d in dt.Rows)
         {
             if (!(d[0].ToString().Contains("<a href=")))
             {
                 Player NewPlayer = new Player();
                 NewPlayer.PlayerNumber = RemoveFormatting(d[0].ToString()).Trim();
                 NewPlayer.PlayerName = RemoveFormatting(d[1].ToString()).Trim();
                 NewPlayer.PlayerPosition = RemoveFormatting(d[2].ToString()).Trim();
                 NewPlayer.Team = TeamName;
                 NewPlayer.Prefix = TeamPrefix;
                 NewPlayer.PlayersSport = CurrentSport;

                 PlayerList.Add(NewPlayer);
             }
         }

        }
    }



    /// <summary>
    /// Parse the roster contained in the Roster string into a list of Player objects, returned in PlayerList
    /// 
    /// </summary>
    /// <param name="PlayerList">output, iList of player objects created from the Roster</param>
    /// <param name="Roster">input, string contining the roster data</param>
    /// <param name="TeamName">input, full text description of the Team Name to attach to players</param>
    /// <param name="TeamPrefix">input, prefix defined by the user to use for creating team codes</param>
    private void ParseRoster_ESPN(List<Player> PlayerList, string Roster, string TeamName, string TeamPrefix, Constants.Sports CurrentSport, string CoachName)
    {

        try
        {
            WebClient GetFile = new WebClient();
            System.Text.Encoding enc = System.Text.Encoding.ASCII;

            var ListOfValues = new List<string>();
            var doc = new HtmlDocument();
            doc.LoadHtml(Roster);
            var TeamNames = new ArrayList();
            var TeamRosterURLs = new ArrayList();

            var theTables = doc.DocumentNode.SelectNodes("//tbody");

            foreach (var table in theTables)
            {
                 foreach (var link in table.ChildNodes)
                    {
                        var a = link.ToString();

                    try
                    {
                        if ((link.ChildNodes.Count >= 3) && (RemoveFormatting(link.ChildNodes[0].InnerText) != "NO.") && (RemoveFormatting(link.ChildNodes[0].InnerText) != "NO"))
                        {
                            Player NewPlayer = new Player();
                            NewPlayer.PlayerNumber = RemoveFormatting(link.ChildNodes[0].InnerText);
                            NewPlayer.PlayerName = RemoveFormatting(link.ChildNodes[1].InnerText);
                            NewPlayer.PlayerPosition = RemoveFormatting(link.ChildNodes[2].InnerText);
                            NewPlayer.Team = TeamName;
                            NewPlayer.Prefix = TeamPrefix;
                            NewPlayer.PlayersSport = CurrentSport;

                            if ((NewPlayer.PlayerNumber.Trim().Length > 0) &&
                                (NewPlayer.PlayerNumber.Trim() != "-") &&
                                (NewPlayer.PlayerNumber.Trim() != "--"))
                            {
                                if (!PlayerList.Contains(NewPlayer))
                                {
                                    PlayerList.Add(NewPlayer);
                                }
                            }
                        }
                    }
                    catch (Exception ESPNException)
                    {
                        log.Error(ESPNException.Message.ToString());
                    }
                }
            }

            if (CoachName.Trim().Length > 0)
            {
                Player Coach = new Player();

                Coach.PlayerPosition = HeadCoachTitle(CurrentSport);
                Coach.Team = TeamName;
                Coach.Prefix = TeamPrefix;
                Coach.PlayersSport = CurrentSport;
                var CoachJerseyNumber = string.Empty;
                if ((CoachName.Contains("(")) && (CurrentSport == Constants.Sports.MLB))
                {
                    Coach.PlayerName = CoachName.Substring(0, CoachName.IndexOf('(') - 1).Trim(); 
                    CoachJerseyNumber = CoachName.Substring(CoachName.IndexOf('(') + 1, (CoachName.IndexOf(')') - CoachName.IndexOf('(') - 1));
                }
                else
                {
                    Coach.PlayerName = CoachName.Trim();
                    CoachJerseyNumber = Constants.HEAD_COACH_JERSEY_NUMBER;
                }

                Coach.PlayerNumber = CoachJerseyNumber;

                PlayerList.Add(Coach);
            }
        }
        catch (Exception ESPNException)
        {
            log.Error(ESPNException.Message.ToString());
        }
    }


    /// <summary>
    /// Remove all the formatting found in the team names which will affect the text
    /// </summary>
    /// <param name="InputParm"></param>
    /// <returns></returns>
    private string RemoveFormatting(string InputParm)
    {
        string OutputParm = InputParm;

        OutputParm = Regex.Replace(OutputParm, "<[^>]*>", string.Empty);
        // remove other formatting characters
        OutputParm = OutputParm.Replace('\r', ' ');
        OutputParm = OutputParm.Replace('\n', ' ');
        OutputParm = OutputParm.Replace('\t', ' ');
        OutputParm = OutputParm.Replace("&nbsp;", "");

        OutputParm = HttpUtility.HtmlDecode(OutputParm);

        int TEAMTextStart = OutputParm.IndexOf("Team ");
        if (TEAMTextStart > 0)
        {
            OutputParm = OutputParm.Substring(0, TEAMTextStart);
        }

        return OutputParm.Trim();

    }


    /// <summary>
    /// Extract the file name from a full path string containing the file name at the end
    /// </summary>
    /// <param name="FullPath"></param>
    /// <returns></returns>
    public string ExtractFileName(string FullPath)
    {
        string FileName = string.Empty;

        int Pos = FullPath.LastIndexOf('\\');
        if (Pos > 0)
        {
            FileName = FullPath.Substring(Pos + 1);
        }

        return FileName;
    }

    /// <summary>
    /// Comparer used when sorting the list of players in ascending order
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int PlayerCompare(Player x, Player y)
    {
        if (x.Team == y.Team)
        {
            if (x.PlayerNumber == y.PlayerNumber)
            {
                if (x.OffenseDefenseFlag != y.OffenseDefenseFlag)
                    return String.Compare(x.OffenseDefenseFlag, y.OffenseDefenseFlag);
                else
                    return String.Compare(x.PlayerName, y.PlayerName);
            }
            else
                return String.Compare(x.PlayerNumber, y.PlayerNumber);
        }
        else
        {
            return String.Compare(x.Team, y.Team);
        }
    }


    /// <summary>
    /// Comparer used with sorting team names
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int TeamCompare(Team x, Team y)
    {
        return String.Compare(x.Name, y.Name);
    }

    /// <summary>
    ///  Attempt to dynamically determine the list of NCAA Football teams to add to a dropdown list rather than use the hard-coded lists
    /// </summary>
    /// <param name="Sport"></param>
    public List<Team> GetTeams_FOXSPORTS(Constants.Sports Sport)
    {
        string TeamsURL = string.Empty;
        List<Team> TeamsList = new List<Team>();

        switch (Sport)
        {
            case Constants.Sports.MLB:
                TeamsURL = "http://msn.foxsports.com/mlb/teams";
                break;
            case Constants.Sports.NBA:
                TeamsURL = "http://msn.foxsports.com/nba/teams";
                break;
            case Constants.Sports.NFL:
                TeamsURL = "http://msn.foxsports.com/nfl/teams";
                break;
            case Constants.Sports.NCAAFootball:
                TeamsURL = "http://msn.foxsports.com/collegefootball/teams";
                break;
            case Constants.Sports.NCAAMensBasketball:
                TeamsURL = "http://msn.foxsports.com/collegebasketball/teams";
                break;
            case Constants.Sports.NCAAWomensBasketball:
                TeamsURL = "";
                break;
        }

        try
        {
            WebClient GetFile = new WebClient();
            System.Text.Encoding enc = System.Text.Encoding.ASCII;

            // get the selected home teams roster from the configured location
            byte[] Teams = GetFile.DownloadData(TeamsURL);
            string x = enc.GetString(Teams);

            DataSet ds = new DataSet();
            DataTable dt;
            DataRow dr;
//            DataColumn dc;
            string TableExpression = "<table[^>]*>(.*?)</table>";
            string HeaderExpression = "<th[^>]*>(.*?)</th>";
            string RowExpression = "<tr[^>]*>(.*?)</tr>";
            string ColumnExpression = "<td[^>]*>(.*?)</td>";
            bool HeadersExist = false;
            int iCurrentColumn = 0;
            int iCurrentRow = 0;

            string s = x;

            // Get a match for all the tables in the HTML  
            MatchCollection Tables = Regex.Matches(s, TableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            string a = Tables[0].ToString();

            //Loop through each table element 
            foreach (Match Table in Tables)
            {
                iCurrentRow = 0;
                HeadersExist = false;

                // Add a new table to the DataSet  
                dt = new DataTable();

                // Create the relevant amount of columns for this table (use the headers if they exist, otherwise use default names)  
                if (Table.Value.Contains("<th"))
                {
                    HeadersExist = true;
                    // Get a match for all the rows in the table
                    MatchCollection Headers = Regex.Matches(Table.Value, HeaderExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                    // Loop through each header element  
                    foreach (Match Header in Headers)
                    {
                        dt.Columns.Add(Header.Groups[1].ToString());
                    }
                }
                else
                {
                    for (int iColumns = 1; iColumns <= Regex.Matches(Regex.Matches(Regex.Matches(Table.Value, TableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase)[0].ToString(), RowExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase)[0].ToString(), ColumnExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase).Count; iColumns++)
                    {
                        dt.Columns.Add("Column " + iColumns);
                    }
                }

                // Get a match for all the rows in the table  
                MatchCollection Rows = Regex.Matches(Table.Value, RowExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                // Loop through each row element  
                foreach (Match Row in Rows)
                {

                    // Only loop through the row if it isn't a header row  
                    if (!((iCurrentRow == 0) && (HeadersExist == true)))
                    {

                        // Create a new row and reset the current column counter  
                        dr = dt.NewRow();
                        iCurrentColumn = 0;

                        // Get a match for all the columns in the row  
                        MatchCollection Columns = Regex.Matches(Row.Value, ColumnExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                        // Loop through each column element  
                        foreach (Match Column in Columns)
                        {

                            // Add the value to the DataRow  
                            dr[iCurrentColumn] = Column.Groups[1].ToString();

                            // Increase the current column   
                            iCurrentColumn += 1;
                        }

                        // Add the DataRow to the DataTable  
                        dt.Rows.Add(dr);

                    }

                    // Increase the current row counter  
                    iCurrentRow += 1;
                }


                foreach (DataRow d in dt.Rows)
                {
                    if (!(d[0].ToString().Contains("fs-data-table-header2 CFB")))
                    {
                        Team NewTeam = new Team();
                        NewTeam.Name = RemoveFormatting(d[0].ToString().Trim());

                        int i = d[0].ToString().IndexOf("<a href");
                        if (i > 0)
                        {
                            int firstquote = d[0].ToString().IndexOf('"', i);
                            int lastquote = d[0].ToString().IndexOf('"', (firstquote + 1));

                            string teamURL = d[0].ToString().Substring((firstquote + 1), (lastquote - firstquote - 1));
                            NewTeam.URL = teamURL;

                            TeamsList.Add(NewTeam);
                        }


                    }
                }
            }
        }
        catch (Exception x)
        {
            log.Error(x.Message.ToString());
        }

        TeamsList.Sort(TeamCompare);
        return TeamsList;
    }


    /// <summary>
    ///  Attempt to dynamically determine the list of NCAA Football teams to add to a dropdown list rather than use the hard-coded lists
    /// </summary>
    /// <param name="Sport"></param>
    public List<Team> GetTeams_ESPN(Constants.Sports Sport)
    {
        string TeamsURL = string.Empty;
        var sportPrefix = string.Empty;
        var lookupURL = string.Empty;
        List<Team> TeamsList = new List<Team>();

        switch (Sport)
        {
            case Constants.Sports.MLB:
                TeamsURL = "http://espn.go.com/mlb/teams";
                sportPrefix = "mlb";
                break;
            case Constants.Sports.NBA:
                TeamsURL = "http://espn.go.com/nba/teams";
                sportPrefix = "nba";
                break;
            case Constants.Sports.NFL:
                TeamsURL = "http://espn.go.com/nfl/teams";
                sportPrefix = "nfl";
                break;
            case Constants.Sports.NCAAFootball:
                TeamsURL = "http://espn.go.com/college-football/teams";
                sportPrefix = "college-football";
                break;
            case Constants.Sports.NCAAMensBasketball:
                TeamsURL = "http://espn.go.com/mens-college-basketball/teams";
                sportPrefix = "mens-college-basketball";
                break;
            case Constants.Sports.NCAAWomensBasketball:
                TeamsURL = "http://espn.go.com/womens-college-basketball/teams";
                sportPrefix = "womens-college-basketball";
                break;
        }

        lookupURL = String.Format("{0}/{1}", sportPrefix, "team/roster");

        try
        {
            WebClient GetFile = new WebClient();
            System.Text.Encoding enc = System.Text.Encoding.ASCII;

            // get the selected home teams roster from the configured location
            byte[] Teams = GetFile.DownloadData(TeamsURL);
            string x = enc.GetString(Teams);
            string s = x;

            var ListOfValues = new List<string>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(s);
            ArrayList TeamNames = new ArrayList();
            ArrayList TeamRosterURLs = new ArrayList();

//            var spans = doc.DocumentNode.SelectNodes("//span//a");

            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//span//a"))
            {
                if (link.OuterHtml.Contains(lookupURL))
                {
                    TeamNames.Add(link.ParentNode.ParentNode.ParentNode.FirstChild.InnerText);
                    TeamRosterURLs.Add(link.Attributes[0].Value);
                }
            }

            int RowCount = (TeamNames.Count < TeamRosterURLs.Count ? TeamNames.Count : TeamRosterURLs.Count);
            for (int index = 0; index < RowCount; index++)
            {
                Team NewTeam = new Team(TeamNames[index].ToString() , TeamRosterURLs[index].ToString());
                TeamsList.Add(NewTeam);
            }

            TeamsList.Sort(TeamCompare);
        }
        catch (Exception ESPNException)
        {
            log.Error(ESPNException.Message.ToString());
        }
        return TeamsList;
    }
    
    /// <summary>
    ///  Under Construction!!!
    ///  Attempt to dynamically determine the list of NCAA Football teams to add to a dropdown list rather than use the hard-coded lists
    /// </summary>
    /// <param name="Sport"></param>
    public string GetDescriptionForSport(string Sport)
    {
        string Description = string.Empty;

        switch (Sport)
        {
            case "MLB":
                Description = "Major League Baseball";
                break;
            case "NBA":
                Description = "NBA";
                break;
            case "NFL":
                Description = "NFL";
                break;
            case "NCAAFootball":
                Description = "NCAA Football";
                break;
            case "NCAAMensBasketball":
                Description = "NCAA Mens Basketball";
                break;
            case "NCAAWomensBasketball":
                Description = "NCAA Womens Basketball";
                break;
        }

        return Description;
    }

    /// <summary>
    ///  Under Construction!!!
    ///  Attempt to dynamically determine the list of NCAA Football teams to add to a dropdown list rather than use the hard-coded lists
    /// </summary>
    /// <param name="Sport"></param>
    public Constants.Sports GetEnumForSport(string Sport)
    {
        string Description = string.Empty;

        switch (Sport)
        {
            case "MLB":
                return Constants.Sports.MLB;
                break;
            case "NBA":
                return Constants.Sports.NBA;
                break;
            case "NFL":
                return Constants.Sports.NFL;
                break;
            case "NCAAFootball":
                return Constants.Sports.NCAAFootball;
                break;
            case "NCAAMensBasketball":
                return Constants.Sports.NCAAMensBasketball;
                break;
            case "NCAAWomensBasketball":
                return Constants.Sports.NCAAWomensBasketball;
                break;
        }

        return Constants.Sports.MLB;
    }



    private string HeadCoachTitle(Constants.Sports Sport)
    {
        string title = string.Empty;
        
        switch (Sport)
        {
            case Constants.Sports.MLB:
                return "manager";
            default:
                return "head coach";
        }
    }

    private void IncludeUsefullCodes(Constants.Sports Sport, StreamWriter sw)
    {
        try
        {
            log.Debug("Including Usefull codes for " + Sport.ToString());

            string UsefullCodeConfig = Sport.ToString() + "UsefullCodes";
            string UsefullCodeFile = ConfigurationManager.AppSettings[UsefullCodeConfig];

            string path = GetFileRootPath("UsefullCodes") + UsefullCodeFile;

            using (StreamReader sr = File.OpenText(path))
            {
                string CodeLine;
                while ((CodeLine = sr.ReadLine()) != null)
                {
                    sw.WriteLine(CodeLine);
                }
                sr.Close();
            }
        }
        catch (Exception UsefullCodeException)
        {
            log.Error(UsefullCodeException.Message.ToString());
        }

    }

}