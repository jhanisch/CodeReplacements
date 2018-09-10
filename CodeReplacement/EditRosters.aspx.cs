using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace CodeReplacement
{
    public partial class EditRosters : System.Web.UI.Page
    {
        private CodeReplacements cr;

        private string HomeTeam;
        private string HomeTeamURL;
        private string VisitingTeam;
        private string VisitingTeamURL;
        private string HomeTeamPrefix;
        private string VistingTeamPrefix;
        private string HomeTeamAltDesc;
        private string VistingTeamAltDesc;
        private string Sport;
        private string DuplicateText;
        private string HomeTeamCoach;
        private string VisitingTeamCoach;

        protected void Page_Load(object sender, EventArgs e)
        {
                HomeTeam = Request.QueryString["HomeTeam"];
                HomeTeamURL = Request.QueryString["HomeTeamURL"];
                VisitingTeam = Request.QueryString["VisitingTeam"];
                VisitingTeamURL = Request.QueryString["VisitingTeamURL"];
                HomeTeamPrefix = Request.QueryString["HomeTeamPrefix"];
                VistingTeamPrefix = Request.QueryString["VistingTeamPrefix"];
                HomeTeamAltDesc = Request.QueryString["HomeTeamAltDesc"];
                VistingTeamAltDesc = Request.QueryString["VistingTeamAltDesc"];
                Sport = Request.QueryString["Sport"];
                DuplicateText = Request.QueryString["DuplicateText"];
                HomeTeamCoach = Request.QueryString["HomeTeamCoach"];
                VisitingTeamCoach = Request.QueryString["VisitingTeamCoach"];

                lblTeams.Text = Server.HtmlEncode("Matchup: " + VisitingTeam + " v " + HomeTeam);

                cr = new CodeReplacements();
                if (!Page.IsPostBack)
                {
                    var customRoster = cr.CreatePlayerRosterText(HomeTeam, HomeTeamURL, VisitingTeam, VisitingTeamURL, HomeTeamPrefix,
                                          VistingTeamPrefix, HomeTeamAltDesc, VistingTeamAltDesc,
                                          Sport, DuplicateText, HomeTeamCoach, VisitingTeamCoach, true);

                    txtRosters.Text = Server.HtmlEncode(customRoster);
                }
        }

        protected void btnCreateCodeReplacementFile_Click(object sender, EventArgs e)
        {
            var customRosterInfo = Server.HtmlEncode(txtRosters.Text);
            string Filename;

            Filename = cr.CreateCustomFile(customRosterInfo, HomeTeam, HomeTeamURL, VisitingTeam, VisitingTeamURL,
                                HomeTeamPrefix, VistingTeamPrefix, HomeTeamAltDesc, VistingTeamAltDesc,
                                Sport, DuplicateText, HomeTeamCoach, VisitingTeamCoach,
                                true);

            DownloadFile(Filename);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="FullFilePath"></param>
        private void DownloadFile(string FullFilePath)
        {
            string FileName = cr.ExtractFileName(FullFilePath);

            // 10-10-10 Updated to include opening the file download window when file completed generating
            if (FileName.Trim() != string.Empty)
            {
                FileStream fs;
                fs = File.Open(FullFilePath, FileMode.Open);
                Byte[] ByteArray = new Byte[fs.Length];
                fs.Read(ByteArray, 0, Convert.ToInt32(fs.Length));
                fs.Close();

                Response.AddHeader("Content-disposition", "attachment; filename=" + FileName);
                Response.ContentType = "application/octet-stream";
                Response.BinaryWrite(ByteArray);
                Response.End();
            }
            else
            {
                lblTeams.Text = "This isn't good, there was an error creating your code replacement file!";
            }
        }

        protected void txtRosters_TextChanged(object sender, EventArgs e)
        {
            string s = String.Empty;

        }
    }
}