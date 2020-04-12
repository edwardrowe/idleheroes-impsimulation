namespace EdwardRowe.IdleHeroes.ImpSimulation
{
    public class IntelligentLucky : Strategy
    {
        public int MinimumLuckyRoll { get; set; }

        public override Choice MakeChoice(GameState state)
        {
            var choice = new Choice();
            var numSpacesAwayFromLucky = state.GameBoard.LuckySpace - state.PlayerPosition;
            if (state.PlayerState.NumLuckyDie > 0 &&
                numSpacesAwayFromLucky <= 6 && numSpacesAwayFromLucky >= MinimumLuckyRoll)
            {
                choice.Action = Choice.Actions.RollLucky;
                choice.LuckyDieValue = numSpacesAwayFromLucky;
            }
            else if (state.PlayerState.NumDie > 0)
            {
                choice.Action = Choice.Actions.Roll;
            }
            else
            {
                if (state.PlayerState.NumLuckyDie > 0)
                {
                    choice.Action = Choice.Actions.RollLucky;
                    choice.LuckyDieValue = 6;
                }
            }

            return choice;
        }
    }
}