using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary;
using TrackerLibrary.Models;
using System.Security.Cryptography;

namespace TrackerLibrary.Data.Process
{
    public static class TournamentLogic
    {
        private static Random rng = new Random();
        
        
        
        

        
        // Create all tournament rounds
        public static void CreateRounds(TournamentModel model)
        {
            List<TeamModel> randomizedTeams = RandomizeTeamOrder(model.EnteredTeams);
            int rounds = FindNumberOfRounds(randomizedTeams.Count);
            int byes = NumberOfByes(rounds, randomizedTeams.Count);
            model.Rounds.Add(CreateFirstRound(byes, randomizedTeams));
            CreateOtherRounds(model, rounds);
        }

        //Order our list randomly of teams
        private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams)
        {
            return teams.OrderBy(a => rng.Next()).ToList();
        }

        //Check if list is big enough - if not, add in byes - 2*2*2*2 - 2^4
        private static int FindNumberOfRounds(int teamCount)
        {
            int output = 1;
            int value = 2;

            while (value < teamCount)
            {value *= 2; output++;}
            return output;
        }

        //Add byes if need to fill the tournament format
        private static int NumberOfByes(int rounds, int numberOfTeams)
        {
            int output = 0;
            int totalTeams = 1;

            for (int i = 1; i <= rounds; i++)
            {
                totalTeams *= 2;
            }


            output = totalTeams - numberOfTeams;
            return output;
        }

        //Create first round of matchups
        private static List<MatchupModel> CreateFirstRound( int byes, List<TeamModel> teams)
        {
            List<MatchupModel> output = new List<MatchupModel>();
            MatchupModel curr = new MatchupModel();

            foreach (TeamModel team in teams)
            {
                curr.Entries.Add(new MatchupEntryModel { TeamCompeting = team });

                if (byes > 0 || curr.Entries.Count > 1)
                {
                    curr.MatchupRound = 1;
                    if (byes > 0)
                    {
                        curr.Winner = team;
                        byes--;
                    }
                    output.Add(curr);
                    curr = new MatchupModel();
                }
            }
            return output;
        }


        //Create every round after that - matchups/2/2/2 >> 8 matchups - 4 matchups - 2 matchups - 1 matchup
        private static void CreateOtherRounds(TournamentModel model, int rounds)
        {
            int round = 2;
            List<MatchupModel> previousRound = model.Rounds[0];
            List<MatchupModel> currRound = new List<MatchupModel>();
            MatchupModel currMatchup = new MatchupModel();

            while (round <= rounds)
            {
                foreach (MatchupModel match in previousRound)
                {
                    if (match.Winner != null) 
                    { 
                        currMatchup.Entries.Add(new MatchupEntryModel {ParentMatchup = match, TeamCompeting = match.Winner}); 
                    }
                    else
                    {
                        currMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = match });
                    }
                    //currMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = match });
                    if (currMatchup.Entries.Count > 1)
                    {
                        currMatchup.MatchupRound = round;
                        currRound.Add(currMatchup);
                        currMatchup = new MatchupModel();
                    }
                    
                }

                model.Rounds.Add(currRound);
                previousRound = currRound;

                currRound = new List<MatchupModel>();
                round++;
            }
        }
    }
}
