using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodeReplacement.Constants;

/// <summary>
/// Summary description for Player
/// </summary>
public class Player : IEquatable<Player>
{
    private string Name;
    private string TeamName;
    private string Number;
    private string Position;
    private string TeamPrefix;
    Constants.Sports playerSport;

	public Player()
	{
        Name = string.Empty;
        TeamName = string.Empty;
        Number = string.Empty;
        Position = string.Empty;
        TeamPrefix = string.Empty;

    }

    public bool Equals(Player other)
    {
        if ((this.Name == other.Name) &&
            (this.TeamName == other.TeamName) &&
            (this.Number == other.Number) &&
            (this.Position == other.Position) &&
            (this.OffenseDefenseFlag == other.OffenseDefenseFlag))
        {
            return true; 
        }

        return false;
    }

    #region Properties
    public string PlayerName
    {
        get { return Name; }
        set { Name = value; }
    }

    public string Team
    {
        get { return TeamName; }
        set { TeamName = value; }
    }

    public string PlayerNumber
    {
        get { return (Number.Trim() == String.Empty ? "-" : Number.Trim()); }
        set { Number = value; }
    }

    public string PlayerPosition
    {
        get { return Position; }
        set { Position = value; }
    }

    public string Prefix
    {
        get { return TeamPrefix; }
        set { TeamPrefix = value; }
    }

    public Constants.Sports PlayersSport
    {
        get { return playerSport; }
        set { playerSport = value; }
    }

    public string OffenseDefenseFlag
    {
        get
        {
            if (playerSport.Equals(Constants.Sports.NCAAFootball))
            {
                switch (Position.ToUpper())
                {
                    case "S":
                    case "NT":
                    case "LB":
                    case "DT":
                    case "DE":
                    case "CB":
                    case "DL":
                    case "DB":
                        return "d";
                    case "HEAD COACH":
                        return string.Empty;
                    default:
                        return "o";
                }
            }
            return string.Empty;
        }
    }

    public string PositionDescription
    {
        get
        {
            string Desc = Position;

            switch (playerSport)
            {
                case Constants.Sports.MLB:
                    switch (Position.ToUpper())
                    {
                        case "1B": Desc = "first baseman";
                            break;
                        case "2B": Desc = "second baseman";
                            break;
                        case "3B": Desc = "third baseman";
                            break;
                        case "SS": Desc = "shortstop";
                            break;
                        case "SP":
                        case "RP":
                        case "P": Desc = "pitcher";
                            break;
                        case "C": Desc = "catcher";
                            break;
                        case "LF": Desc = "left fielder";
                            break;
                        case "CF": Desc = "center fielder";
                            break;
                        case "RF": Desc = "right fielder";
                            break;

                    }
                    break;
                case Constants.Sports.NFL:
                case Constants.Sports.NCAAFootball:
                    switch (Position.ToUpper())
                    {
                        case "WR": Desc = "wide receiver";
                            break;
                        case "TE": Desc = "tight end";
                            break;
                        case "T": Desc = "tackle";
                            break;
                        case "S":
                        case "FS":
                        case "SS": Desc = "safety";
                            break;
                        case "RB": Desc = "running back";
                            break;
                        case "QB": Desc = "quarterback";
                            break;
                        case "P": Desc = "punter";
                            break;
                        case "NT": Desc = "nose tackle";
                            break;
                        case "LB": Desc = "linebacker";
                            break;
                        case "K": 
                        case "PK": Desc = "kicker";
                            break;
                        case "OG":
                        case "G": Desc = "guard";
                            break;
                        case "OT": Desc = "offensive tackle";
                            break;
                        case "DT": Desc = "defensive tackle";
                            break;
                        case "DE": Desc = "defensive end";
                            break;
                        case "CB": Desc = "cornerback";
                            break;
                        case "C":
                        case "LS":      // long snapper
                                   Desc = "center";
                            break;
                        case "OL": Desc = "offensive lineman";
                            break;
                        case "DL": Desc = "defensive lineman";
                            break;
                        case "DB": Desc = "defensive back";
                            break;
                        case "FB": Desc = "fullback";
                            break;
                    }
                    break;
                case Constants.Sports.NBA:
                case Constants.Sports.NCAAMensBasketball:
                case Constants.Sports.NCAAWomensBasketball:
                    switch (Position.ToUpper())
                    {
                        case "F": Desc = "forward";
                            break;
                        case "PG":
                        case "SG":
                        case "G": Desc = "guard";
                            break;
                        case "C": Desc = "center";
                            break;
                        case "G-F":
                        case "F-G": Desc = "guard-forward";
                            break;
                        case "F-C":
                        case "C-F":
                        case "FC":
                                    Desc = "forward-center";
                            break;
                        case "PF": 
                        case "SF": 
                                    Desc = "forward";
                            break;
                        default:
                            Desc = Position;
                            break;
                    }

                    if (Position.ToUpper() == "SHOOTING")
                    {
                        Desc = "guard";
                    }

                    if (Position.ToUpper() == "POWER")
                    {
                        Desc = "forward";
                    }

                    if (Position.ToUpper() == "POINT")
                    {
                        Desc = "guard";
                    }

                    if (Position.ToUpper() == "SMALL")
                    {
                        Desc = "forward";
                    }
                    break;
            }

            return Desc.ToLower();
        }
    }
    #endregion
}