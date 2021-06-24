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

namespace TrackerUI
{
    public partial class CreateTeamForm : Form
    {
        ITeamRequester callingForm;

        private List<PersonModel> availablePersonModels = GlobalConfig.Connection.GetPerson_ALL();
        //public List<TeamModel> createdTeams = GlobalConfig.Connection.GetTeam_ALL();
        private List<PersonModel> selectedPersonModels = new List<PersonModel>();
        public CreateTeamForm(ITeamRequester caller)
        {
            InitializeComponent();
            callingForm = caller;
            //CreateSampleData();
            //CleanUpLists();
            WireUpLists();
        }
        // TODO - Fix function to remove already in team users from being available again.
        /*
        private void CleanUpLists()
        {
            List<TeamModel> cleanUpPersonModels = createdTeams;
            foreach (PersonModel p in cleanUpPersonModels.TeamMembers)
            {
                availablePersonModels.Remove(p);
                selectedPersonModels.Remove(p);
            }
        }
        */

        private void WireUpLists()
        {
            selectTeamMemberDropDown.DataSource = null;
            teamMembersListBox.DataSource = null;

            selectTeamMemberDropDown.DataSource = availablePersonModels;
            selectTeamMemberDropDown.DisplayMember = "FullName";

            teamMembersListBox.DataSource = selectedPersonModels;
            teamMembersListBox.DisplayMember = "FullName";

        }

        private void CreateSampleData()
        {
            availablePersonModels.Add(new PersonModel { FirstName = "Abby", LastName = "Turner" });
            availablePersonModels.Add(new PersonModel { FirstName = "John", LastName = "Smith" });

            selectedPersonModels.Add(new PersonModel { FirstName = "Carter", LastName = "Simpson" });
            selectedPersonModels.Add(new PersonModel { FirstName = "Randy", LastName = "Jackson" });
        }


        private void createTeamButton_Click(object sender, EventArgs e)
        {
            //Creates team & Clears fields
            // TODO - Add Option to close the form instead of when team is created.
            if (ValidateTeam())
            {
                TeamModel t = new TeamModel();
                t.TeamName = teamNameValue.Text;
                t.TeamMembers = selectedPersonModels;

                GlobalConfig.Connection.CreateTeam(t);

                callingForm.TeamComplete(t);
                this.Close();
                /*
                selectedPersonModels.Clear();
                teamNameValue.Text = "";
                WireUpLists();
                */
            }

            else
            {
                MessageBox.Show("You need a team name and 5 players\nto create a team.");
            }
            

        }
        //Check if team creation for is properly filled.
        private bool ValidateTeam()
        {
            if (teamNameValue.Text == "")
            {
                return false;
            }
            if (selectedPersonModels.Count != 5)
            {
                return false;
            }
            return true;
        }


        private void createMemberButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PersonModel p = new PersonModel();

                p.FirstName = firstNameValue.Text;
                p.LastName = lastNameValue.Text;
                p.EmailAddress = emailValue.Text; 
                p.CellphoneNumber = cellphoneValue.Text;

                p = GlobalConfig.Connection.CreatePerson(p);

                firstNameValue.Text = "";
                lastNameValue.Text = "";
                emailValue.Text = "";
                cellphoneValue.Text = "";

                selectedPersonModels.Add(p);
                WireUpLists();
            }
            else
            {
                MessageBox.Show("You need to fill in all of the fields.");
            }
            
        }

        private bool ValidateForm()
        {
            if (firstNameValue.Text.Length == 0)
            {
                return false;
            }
            if (lastNameValue.Text.Length == 0)
            {
                return false;
            }
            if (emailValue.Text.Length == 0)
            {
                return false;
            }
            if (cellphoneValue.Text.Length == 0)
            {
                return false;
            }

            return true;

        }

        private void addMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)selectTeamMemberDropDown.SelectedItem;
            if (p != null)
            {
                availablePersonModels.Remove(p);
                selectedPersonModels.Add(p);
            
                WireUpLists();
            }
        }

        private void removeSelectedMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)teamMembersListBox.SelectedItem;

            if (p != null)
            {
                selectedPersonModels.Remove(p);
                availablePersonModels.Add(p);

                WireUpLists();
            }
            
        }

        private void removeSelectedMemberButton_MouseHover(object sender, EventArgs e)
        {

        }

        private void selectTeamMembersDropDown_SelectedValueChanged(object sender, EventArgs e)
        {
            
        }
        private void teamMembersListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (teamMembersListBox.SelectedItem != null)
            {
                removeSelectedMemberButton.Enabled = true;
            }
            else
            {
                removeSelectedMemberButton.Enabled = false;
            }
        }
    }
}
