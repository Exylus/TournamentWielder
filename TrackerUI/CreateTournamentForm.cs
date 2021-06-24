using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;
using TrackerLibrary.Data.Process;

namespace TrackerUI
{
    public partial class CreateTournamentForm : Form, IPrizeRequester, ITeamRequester
    {
        List<TeamModel> availableTeams = GlobalConfig.Connection.GetTeam_ALL();
        List<TeamModel> selectedTeams = new List<TeamModel>();
        List<PrizeModel> selectedPrizes = new List<PrizeModel>();
        public CreateTournamentForm()
        {
            InitializeComponent();
            WireUpLists();
        }

        // Refresh all lists
        // TODO - find better way to refresh without using NULL
        private void WireUpLists()
        {
            selectTeamDropDown.DataSource = null;
            tournamentTeamsListBox.DataSource = null;
            prizesListBox.DataSource = null;

            selectTeamDropDown.DataSource = availableTeams;
            selectTeamDropDown.DisplayMember = "TeamName";

            tournamentTeamsListBox.DataSource = selectedTeams;
            tournamentTeamsListBox.DisplayMember = "TeamName";

            prizesListBox.DataSource = selectedPrizes;
            prizesListBox.DisplayMember = "PlaceName";
        }

        // Switch TeamModel from available teams to selected team.
        private void addTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)selectTeamDropDown.SelectedItem;
            if (t != null)
            {
                availableTeams.Remove(t);
                selectedTeams.Add(t);

                WireUpLists();
            }
        }
        // Call CreatePrizeForm.
        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            CreatePrizeForm frm = new CreatePrizeForm(this);
            frm.Show();
        }

        // Call CreateTeamForm.
        private void createNewTeamLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateTeamForm frm = new CreateTeamForm(this);
            frm.Show();
        }

        // Take the PrizeModel and put it into our list of selected Prizes.
        public void PrizeComplete(PrizeModel model)
        {
            selectedPrizes.Add(model);
            WireUpLists();
        }

        // Take TeamModel and put it into list of selected teams.
        public void TeamComplete(TeamModel model)
        {
            selectedTeams.Add(model);
            WireUpLists();
        }

        private void removeSelectedTeamsButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)tournamentTeamsListBox.SelectedItem;

            if (t != null)
            {
                selectedTeams.Remove(t);
                availableTeams.Add(t);

                WireUpLists();
            }
        }

        private void removeSelectedPrizeButton_Click(object sender, EventArgs e)
        {
            PrizeModel p = (PrizeModel)prizesListBox.SelectedItem;

            if (p != null)
            {
                selectedPrizes.Remove(p);

                WireUpLists();
            }
        }
        // TODO - Finish the create tournament function on button click
        // SQL [*Tournament, *TournamentPrizes, *TournamentEntries, Matchups]
        // TextFile [*Tournament, *TournamentPrizes, *TournamentEntries, Matchups]
        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            // Validate data
            if (ValidateForm())
            {
                // Create tournament model
                TournamentModel tm = new TournamentModel();

                tm.TournamentName = tournamentNameValue.Text;
                tm.EntryFee = decimal.Parse(entryFeeValue.Text);
                tm.Prizes = selectedPrizes;
                tm.EnteredTeams = selectedTeams;

                // Wire matchups
                TournamentLogic.CreateRounds(tm);

                // Create tournament, prizes & teams entries
                GlobalConfig.Connection.CreateTournament(tm);

                
            }
        }

        // Create Tournament Data validation method
        private bool ValidateForm()
        {
            bool feeAcceptable = decimal.TryParse(entryFeeValue.Text, out decimal fee);

            if (!feeAcceptable)
            {
                MessageBox.Show("The entry fee can only have numbers.\nError: #Invalid.Fee",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            if (selectedTeams.Count < 2)
            {
                MessageBox.Show("The tournament needs at least 2 teams to be created.\nError: #Invalid.Team.Parameters",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            if (tournamentNameValue.Text == "")
            {
                MessageBox.Show("Cannot create tournament without a name.\nError: #Invalid.Tournament.Name",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
    }
}
