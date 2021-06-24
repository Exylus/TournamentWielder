using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Data.Access;

namespace TrackerLibrary.Models
{
    public class MatchupModel
    { 
        public int Id { get; set; }
        /// <summary>
        /// list of matchups within one round.
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();
        /// <summary>
        /// Represents the winner for a matchup.
        /// </summary>
        public TeamModel Winner { get; set; }
        /// <summary>
        /// Represents the round for the matchup.
        /// </summary>
        public int MatchupRound { get; set; }
        
    }
}
