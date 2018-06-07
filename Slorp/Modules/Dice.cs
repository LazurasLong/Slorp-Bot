using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Slorp.Modules
{
    public class Dice
    {
        private string _input;
        private List<DSet> _dSets = new List<DSet>();
        private List<string> results = new List<string>();

        public DiscordEmbed DRoll(string input)
        {
            _input = input;
            ValidateInput();
            Run();

            return EmbedBuilder();
        }

        private void ValidateInput()
        {
            string[] sets = _input.Split(';');
            foreach (string set in sets)
            {
                int[] diceSet = new int[3];
                Match RegexMatch = Regex.Match(set, @"^(\d+)d(\d+)(-?\+?\d+)?$");

                diceSet[0] = Int32.Parse(RegexMatch.Groups[1].Value);
                diceSet[1] = Int32.Parse(RegexMatch.Groups[2].Value);
                diceSet[2] = string.IsNullOrEmpty(RegexMatch.Groups[3].Value) ? 0 : Int32.Parse(RegexMatch.Groups[3].Value);

                _dSets.Add(new DSet(diceSet));
            }
        }

        private void Run()
        {
            foreach (var d in _dSets) d.Roll();

            for (int i = 0; i < _dSets.Count; i++)
                results.Add(_dSets[i].dResult.Total.ToString());
        }

        private DiscordEmbed EmbedBuilder()
        {
            var messageBuilder = new StringBuilder();

            for (int i = 0; i < results.Count; i++)
            {
                string mod = string.Empty;
                int moddedTotal = _dSets[i].dResult.Total + _dSets[i].Modifier;

                // Sets mod string if mod != 0
                if (_dSets[i].Modifier == 0)
                    mod = string.Empty;
                else if (_dSets[i].Modifier > 0)
                    mod = $"+{_dSets[i].Modifier.ToString()}";
                else
                    mod = _dSets[i].Modifier.ToString();

                messageBuilder.Append("("); // Start-of-set bracket

                // Adds each roll to messageBuilder, with a comma if theres another roll to add
                for (int j = 0; j < _dSets[i].dResult.results.Count; j++)
                {
                    if (j + 1 < _dSets[i].dResult.results.Count)
                    {
                        messageBuilder.Append(_dSets[i].dResult.results[j].Item2.ToString() + ", ");
                    }
                    else messageBuilder.Append(_dSets[i].dResult.results[j].Item2.ToString());
                }

                messageBuilder.Append(")"); // End-of-set bracket

                messageBuilder.Append(mod + $"  Total: {moddedTotal}");

                // if 1d20 is rolled, checks for critical success/failure.
                if (_dSets[i].DiceNum == 1 && _dSets[i].DiceType == 20 && _dSets[i].dResult.Total == 20)
                    messageBuilder.Append("  Critical success!");
                else if (_dSets[i].DiceNum == 1 && _dSets[i].DiceType == 20 && _dSets[i].dResult.Total == 1)
                    messageBuilder.Append("  Critical failure!");

                messageBuilder.Append("\n");
            }

            var embedBuilder = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#ea596e"),
                Title = results.Count == 1 ? $"Your roll :game_die:" : $"Your rolls :game_die:",
                Description = messageBuilder.ToString().Replace("\r\n", "\n"),
            };
            return embedBuilder.Build();
        }
    }

    class DSet
    {
        private int[] _rollSet = new int[3];

        public int DiceNum { get => _rollSet[0]; }
        public int DiceType { get => _rollSet[1]; }
        public int Modifier { get => _rollSet[2]; }

        public Results dResult = new Results();

        private static readonly Random random = new Random();

        public DSet(int[] diceSet)
        {
            _rollSet = diceSet;
        }

        public void Roll()
        {
            int nRnd = 0;
            // Loops for each die
            for (int i = 0; i < _rollSet[0]; i++)
            {
                // Generates a random number between 1 and the number of sides of the die
                nRnd = random.Next(1, _rollSet[1] + 1);
                // Adds the random number to the result list
                dResult.results.Add(new Tuple<int, int>(_rollSet[1], nRnd));
            }
        }
    }

    class Results
    {
        public List<Tuple<int, int>> results = new List<Tuple<int, int>>();
        public int Total => results.Sum(x => x.Item2);
    }
}
