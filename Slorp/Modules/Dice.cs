using DSharpPlus.CommandsNext;
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

        public DiscordEmbed DRoll(string input, CommandContext ctx)
        {
            _input = input;
            ValidateInput();
            Run();

            return EmbedBuilder(ctx);
        }

        private void ValidateInput()
        {
            string[] sets = _input.Split(';');
            foreach (string set in sets)
            {
                int dNum, dType, mod;
                Match RegexMatch = Regex.Match(set, @"^(\d+)d(\d+)(-?\+?\d+)?$");

                dNum = Int32.Parse(RegexMatch.Groups[1].Value);
                dType = Int32.Parse(RegexMatch.Groups[2].Value);
                mod = string.IsNullOrEmpty(RegexMatch.Groups[3].Value) ? 0 : Int32.Parse(RegexMatch.Groups[3].Value);

                _dSets.Add(new DSet(dNum, dType, mod));
            }
        }

        private void Run()
        {
            int[] setTotal = new int[_dSets.Count];

            foreach (var d in _dSets) d.Roll();

            for (int i = 0; i < _dSets.Count; i++)
                results.Add(_dSets[i].dResult.Total.ToString());
        }

        private DiscordEmbed EmbedBuilder(CommandContext ctx)
        {
            var messageBuilder = new StringBuilder();

            for (int i = 0; i < results.Count; i++)
            {
                string mod = string.Empty;
                int moddedTotal = _dSets[i].dResult.Total + _dSets[i].Modifier;

                // Sets mod string if mod != 0
                if (_dSets[i].Modifier == 0) mod = "";
                else if (_dSets[i].Modifier > 0) mod = $"+{_dSets[i].Modifier.ToString()}";
                else mod = _dSets[i].Modifier.ToString();

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
                {
                    messageBuilder.Append("  Critical success!");
                }
                else if (_dSets[i].DiceNum == 1 && _dSets[i].DiceType == 20 && _dSets[i].dResult.Total == 1)
                {
                    messageBuilder.Append("  Critical failure!");
                }

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
        private int _diceNum;
        private int _diceType;
        private int _modifier;

        public Results dResult = new Results();

        public int DiceNum { get => _diceNum; }
        public int DiceType { get => _diceType; }
        public int Modifier { get => _modifier; }

        private static readonly Random random = new Random();

        public DSet(int diceNum, int diceType, int modifier)
        {
            _diceNum = diceNum;
            _diceType = diceType;
            _modifier = modifier;
        }

        public void Roll()
        {
            int nRnd = 0;
            // Loops for each die
            for (int i = 0; i < _diceNum; i++)
            {
                // Generates a random number between 1 and the number of sides of the die
                nRnd = random.Next(1, _diceType + 1);
                // Adds the random number to the result list
                dResult.results.Add(new Tuple<int, int>(_diceType, nRnd));
            }
        }
    }

    class Results
    {
        public List<Tuple<int, int>> results = new List<Tuple<int, int>>();
        public int Total => results.Sum(x => x.Item2);
    }
}
