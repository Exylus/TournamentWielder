using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Data.Access;

namespace TrackerLibrary.Models
{
    public class TeamModel
    {
        /// <summary>
        /// TeamId
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Represents the team name.
        /// </summary>
        public string TeamName { get; set; }
        /// <summary>
        /// Represents the list of members within the team.
        /// </summary>
        public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();
        
    }
}
