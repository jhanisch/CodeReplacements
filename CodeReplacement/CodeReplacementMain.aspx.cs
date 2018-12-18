﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CodeReplacement
{
    public partial class CodeReplacementNBA : System.Web.UI.Page
    {
        private CodeReplacements cr;
        private string Sport;

        protected void Page_Load(object sender, EventArgs e)
        {
            cr = new CodeReplacements();
            
            Sport = Request.QueryString["Sport"];

            lblSport.Text = cr.GetDescriptionForSport(Sport) + " Code Replacement";

            LoadTeamsForSport();
        }

        protected void btnCreateFile_Click(object sender, EventArgs e)
        {
            bool HelfulCodes = true;

            // contains the full absolute directory on the filesystem to the new file
            string FullFilePath = cr.CreateFile(HomeTeamDropDown.SelectedItem.Text.ToString(), 
                HomeTeamDropDown.SelectedValue.ToString(),
                VisitingTeamDropDown.SelectedItem.Text.ToString(),
                VisitingTeamDropDown.SelectedValue.ToString(),
                HomeTeamPrefix.Text, 
                VisitingTeamPrefix.Text, 
                HomeTeamAlternateDesc.Text.Trim(), 
                VisitingTeamAlternateDesc.Text.Trim(), 
                GetEnumForSport(), 
                txtDuplicate.Text, 
                HomeTeamCoach.Text, 
                VisitingTeamCoach.Text, 
                HelfulCodes);

            DownloadFile(FullFilePath);

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
                lblSaveDirection.Text = "This isn't good, there was an error creating your code replacement file!";
                lblSaveDirection.Visible = true;
            }
        }

        #region Methods
        private void LoadTeamsForSport()
        {
            IList<Team> TeamList = new List<Team>();

            // Constants must match those for Constants.Sports
            switch (Sport)
            {
                case "MLB":
                    TeamList = cr.GetTeams_ESPN(Constants.Constants.Sports.MLB);
                    break;
                case "NFL":
                    TeamList = cr.GetTeams_ESPN(Constants.Constants.Sports.NFL);
                    break;
                case "NCAAFootball":
                    TeamList = cr.GetTeams_ESPN(Constants.Constants.Sports.NCAAFootball);

                    lblDuplicateText.Visible = true;
                    txtDuplicate.Visible = true;
                    break;
                case "NCAAMensBasketball":
                    TeamList = cr.GetTeams_ESPN(Constants.Constants.Sports.NCAAMensBasketball);
                    break;
                case "NBA":
                    TeamList = cr.GetTeams_ESPN(Constants.Constants.Sports.NBA);
                    break;
                case "6":
                    break;
                case "7":
                    break;
            }

            LoadTeams(TeamList, HomeTeamDropDown);
            LoadTeams(TeamList, VisitingTeamDropDown);
        }


        /// <summary>
        /// Add the teams from the Team List into the drop down list
        /// </summary>
        /// <param name="TeamList"></param>
        /// <param name="ddl"></param>
        private void LoadTeams(IList<Team> TeamList, DropDownList ddl)
        {
            foreach (Team t in TeamList)
            {
                ListItem newItem = new ListItem(t.Name, t.URL);
                if (!ddl.Items.Contains(newItem))
                {
                    ddl.Items.Add(newItem);
                }
            }
        }



        private Constants.Constants.Sports GetEnumForSport()
        {

            foreach(var s in Enum.GetValues(typeof(Constants.Constants.Sports)))
            {
                if (s.ToString() == Sport)
                    return (Constants.Constants.Sports) s;
            }

            return Constants.Constants.Sports.MLB;
        }

        #endregion

        protected void btnEditPlayerList_Click(object sender, EventArgs e)
        {
            Response.Redirect("EditRosters.aspx?HomeTeam=" + HomeTeamDropDown.SelectedItem.Text + 
                               "&HomeTeamURL=" + HomeTeamDropDown.SelectedValue.ToString() +
                               "&VisitingTeam=" + VisitingTeamDropDown.SelectedItem.Text +
                               "&VisitingTeamURL=" + VisitingTeamDropDown.SelectedValue.ToString() +
                               "&HomeTeamPrefix=" + HomeTeamPrefix.Text +
                               "&VistingTeamPrefix=" + VisitingTeamPrefix.Text +
                               "&HomeTeamAltDesc=" + HomeTeamAlternateDesc.Text.Trim() +
                               "&VistingTeamAltDesc=" + VisitingTeamAlternateDesc.Text.Trim() +
                               "&Sport=" + GetEnumForSport() +
                               "&DuplicateText=" + txtDuplicate.Text +
                               "&HomeTeamCoach=" + HomeTeamCoach.Text +
                               "&VisitingTeamCoach=" + VisitingTeamCoach.Text );

        }

    }
}