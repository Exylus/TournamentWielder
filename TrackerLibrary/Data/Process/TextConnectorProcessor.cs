﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Data.Access;
using TrackerLibrary.Models;



namespace TrackerLibrary.Data.Process
{
    public static class Text
    {
        //returns full file path from a file name
        public static string FullFilePath(this string fileName)
        {
            return $"{ConfigurationManager.AppSettings["filepath"]}\\{fileName}";
        }

        //Load or create specific file from submitted Path
        public static List<string> LoadFile(this string file)
        {
            if (!File.Exists(file))
            {
                return new List<string>();
            }
            return File.ReadAllLines(file).ToList();
        }

        //Convert list of PersonModels to string - PersonId,FirstName,LastName,EmailAddress,CellphoneNumber
        public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
        {
            List<PersonModel> output = new List<PersonModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                PersonModel p = new PersonModel();
                p.Id = int.Parse(cols[0]);
                p.FirstName = cols[1];
                p.LastName = cols[2];
                p.EmailAddress = cols[3];
                p.CellphoneNumber = cols[4];
                output.Add(p);
            }
            return output;
        }

        //Convert list of PrizeModels to string - PrizeId,PlaceNumber,PlaceName,PrizeAmount,PrizePercentage
        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                PrizeModel p = new PrizeModel();
                p.Id = int.Parse(cols[0]);
                p.PlaceNumber = int.Parse(cols[1]);
                p.PlaceName = cols[2];
                p.PrizeAmount = decimal.Parse(cols[3]);
                p.PrizePercentage = double.Parse(cols[4]);
                output.Add(p);
            }
            return output;
        }

        // Convert list of TeamModels - TeamId,TeamName,PersonId|PersonId|PersonId|PersonId|PersonId
        public static List<TeamModel> ConvertToTeamModels(this List<string> lines, string peopleFileName)
        {
            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = peopleFileName.FullFilePath().LoadFile().ConvertToPersonModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                TeamModel t = new TeamModel();
                t.Id = int.Parse(cols[0]);
                t.TeamName = cols[1];

                string[] personIds = cols[2].Split('|');

                foreach (string id in personIds)
                {
                    t.TeamMembers.Add(people.Where(x => x.Id == int.Parse(id)).First());
                }
                output.Add(t);
            }
            return output;
        }

        // Convert List of teams, people, prizes and rounds to a tournament data string
        // TournamendId,TournamentName,EntryFee,Entered Teams(id|id|id),Prizes(id|id|id),Rounds(id-id-id-id-id-id-id-id|id-id-id-id|id-id)
        public static List<TournamentModel> ConvertToTournamentModels(this List<string> lines, string teamFileName, string peopleFileName, string prizeFileName)
        {
            List<TournamentModel> output = new List<TournamentModel>();
            List<TeamModel> teams = teamFileName.FullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);
            List<PrizeModel> prizes = prizeFileName.FullFilePath().LoadFile().ConvertToPrizeModels();
            List<MatchupModel> matchups = GlobalConfig.MatchupsFile.FullFilePath().LoadFile().ConvertToMatchupModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                TournamentModel tm = new TournamentModel();
                tm.Id = int.Parse(cols[0]);
                tm.TournamentName = cols[1];
                tm.EntryFee = decimal.Parse(cols[2]);

                string[] teamIds = cols[3].Split('|');

                foreach (string id in teamIds)
                {
                    tm.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(id)).First());
                }

                string[] prizeIds = cols[4].Split('-');

                foreach (string id in prizeIds)
                {
                    tm.Prizes.Add(prizes.Where(x => x.Id == int.Parse(id)).First());
                }

                string[] rounds = cols[5].Split('|');

                List<MatchupModel> ms = new List<MatchupModel>();

                foreach (string round in rounds)
                {
                    string[] m = round.Split('-');
                    foreach(string matchupModelTextId in m)
                    {
                        ms.Add(matchups.Where(x => x.Id == int.Parse(matchupModelTextId)).First());
                    }

                    tm.Rounds.Add(ms);
                }

                output.Add(tm);
            }

            return output;
        }

        // Convert list of people to string PersonId|PersonId|PersonId
        public static string ConvertPeopleListToString(List<PersonModel> people)
        {
            string output = "";

            foreach (PersonModel p in people)
            {
                output += $"{p.Id}|";
            }

            output = output.Substring(0, output.Length - 1);
            return output;
        }

        // Convert list of teams to string - TeamId|TeamdId|TeamId
        private static string ConvertTeamListToString(List<TeamModel> teams)
        {
            string output = "";

            foreach (TeamModel t in teams)
            {
                output += $"{t.Id}|";
            }

            output = output.Substring(0, output.Length - 1);
            return output;
        }

        // Convert list of prizes to string - PrizeIdPrizeId|PrizeId
        private static string ConvertPrizeListToString(List<PrizeModel> prizes)
        {
            string output = "";

            foreach (PrizeModel pz in prizes)
            {
                output += $"{pz.Id}|";
            }

            output = output.Substring(0, output.Length - 1);
            return output;
        }

        // Convert list of rounds  with incremented matchups to string - id-id-id-id-id-id-id-id|id-id-id-id|id-id
        private static string ConvertRoundListToString(List<List<MatchupModel>> rounds)
        {
            string output = "";

            foreach (List<MatchupModel> r in rounds)
            {
                output += $"{ConvertMatchupListToString(r)}|";
            }

            output = output.Substring(0, output.Length - 1);
            return output;
        }

        // Convert list of matchup to string - id-id-id
        private static string ConvertMatchupListToString(this List<MatchupModel> matchups)
        {
            string output = "";

            foreach (MatchupModel m in matchups)
            {
                output += $"{m.Id}-";
            }

            output = output.Substring(0, output.Length - 1);
            return output;
        }

        // Convert list on matchup entries to string - id|id|id
        private static string ConvertMatchupEntryListToString(List<MatchupEntryModel> entries)
        {
            string output = "";

            foreach (MatchupEntryModel me in entries)
            {
                output += $"{me.Id}|";
            }

            output = output.Substring(0, output.Length - 1);
            return output;
        }

        // Save Prize to prize file - id|PlaceNumber|PlaceName|PrizeAmount|PrizePercentage
        public static void SaveToPrizeFile(this List<PrizeModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (PrizeModel p in models)
            {
                lines.Add($"{p.Id},{p.PlaceNumber},{p.PlaceName},{p.PrizeAmount},{p.PrizePercentage}");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        // Save person to people file - id|FirstName|LastName|EmailAddress|CellphoneNumber
        public static void SaveToPeopleFile(this List<PersonModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (PersonModel p in models)
            {
                lines.Add($"{p.Id},{p.FirstName},{p.LastName},{p.EmailAddress},{p.CellphoneNumber}");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        // Save team to teams file.
        public static void SaveToTeamsFile(this List<TeamModel> models, string fileName)
        {
            List<string> lines = new List<string>();
            foreach(TeamModel t in models)
            {
                lines.Add($"{t.Id},{t.TeamName},{ConvertPeopleListToString(t.TeamMembers)}");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        // Save Tournament to tournaments file.
        public static void SaveToTournamentFile(this List<TournamentModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (TournamentModel tm in models)
            {
                lines.Add($"{tm.Id},{tm.TournamentName},{tm.EntryFee},{ConvertTeamListToString(tm.EnteredTeams)},{ConvertPrizeListToString(tm.Prizes)},{ConvertRoundListToString(tm.Rounds)}");
            }
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static void SaveRoundsToFile(this TournamentModel model, string matchupFile, string matchupEntryFile)
        {
            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    matchup.SaveMatchupToFile(matchupFile, matchupEntryFile);
                }
            }
        }

        public static List<MatchupEntryModel> ConvertToMatchupEntryModels(this List<string> lines)
        {   // id = 0, TeamCompeting = 1, Score = 2, ParentMatchup = 3
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split('|');

                MatchupEntryModel me = new MatchupEntryModel();
                me.Id = int.Parse(cols[0]);
                me.TeamCompeting = LookupTeamById(int.Parse(cols[1]));
                me.Score = double.Parse(cols[2]);

                int parentId = 0;
                if(int.TryParse(cols[3], out parentId))
                {
                    me.ParentMatchup = LookupMatchupById(parentId);
                }
                else
                {
                    me.ParentMatchup = null;
                }
                
                output.Add(me);
            }
            return output;
        }

        private static List<MatchupEntryModel> ConvertStringToMatchupEntryModels(string input)
        {
            string[] ids = input.Split('|');
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();

            foreach (string id in ids)
            {
                output.Add(entries.Where(x => x.Id == int.Parse(id)).First());
            }
            return output;
        }

        static List<MatchupModel> ConvertToMatchupModels(this List<string> lines)
        {
            //id,entries, winner, matchupRound
           List<MatchupModel> output = new List<MatchupModel>();

           foreach (string line in lines)
           {
               string[] cols = line.Split(',');

               MatchupModel p = new MatchupModel();
               p.Id = int.Parse(cols[0]);
               p.Entries = ConvertStringToMatchupEntryModels(cols[1]);
               p.Winner = LookupTeamById(int.Parse(cols[2]));
               p.MatchupRound = int.Parse(cols[3]);
               output.Add(p);
           }
           return output;
        }

        public static void SaveMatchupToFile(this MatchupModel matchup, string MatchupsFile, string matchupEntryFile)
        {

            List<MatchupModel> matchups = GlobalConfig.MatchupsFile.FullFilePath().LoadFile().ConvertToMatchupModels();

            int currentId = 1;

            if (matchups.Count > 0)
            {
                currentId = matchups.OrderByDescending(x => x.Id).First().Id + 1;
            }
            matchup.Id = currentId;
            matchups.Add(matchup);

            // save to file
            List<string> lines = new List<string>();
            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null)
                {
                    winner = m.Winner.Id.ToString();
                }
                lines.Add($"{m.Id},{""},{winner},{m.MatchupRound}");
            }
            File.WriteAllLines(GlobalConfig.MatchupsFile.FullFilePath(), lines);

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.SaveEntryToFile(matchupEntryFile);
            }

            // save to file
            lines = new List<string>();
            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null)
                {
                    winner = m.Winner.Id.ToString();
                }
                lines.Add($"{m.Id},{ConvertMatchupEntryListToString(m.Entries)},{winner},{m.MatchupRound}");
            }
            File.WriteAllLines(GlobalConfig.MatchupsFile.FullFilePath(), lines);
        }

        public static void SaveEntryToFile (this MatchupEntryModel entry, string matchupEntryFile)
        {
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();

            int currentId = 1;

            if (entries.Count > 0)
            {
                currentId = entries.OrderByDescending(x => x.Id).First().Id + 1;
            }
            entry.Id = currentId;

            List<string> lines = new List<string>();
            foreach(MatchupEntryModel e in entries)
            {
                string parent = "";
                if (e.ParentMatchup != null)
                {
                    parent = e.ParentMatchup.Id.ToString();
                }
                string teamCompeting = "";
                if (e.TeamCompeting != null)
                {
                    teamCompeting = e.TeamCompeting.Id.ToString();
                }
                lines.Add($"{e.Id},{teamCompeting},{e.Score},{parent}");
            }
            File.WriteAllLines(GlobalConfig.MatchupEntriesFile.FullFilePath(), lines);
        }

        private static TeamModel LookupTeamById(int id)
        {
            List<TeamModel> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels(GlobalConfig.PeopleFile);

            return teams.Where(x => x.Id == id).First();
        }

        private static MatchupModel LookupMatchupById(int id)
        {
            List<MatchupModel> matchups = GlobalConfig.MatchupsFile.FullFilePath().LoadFile().ConvertToMatchupModels();

            return matchups.Where(x => x.Id == id).First();
        }
    }
}
