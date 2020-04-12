namespace EdwardRowe.IdleHeroes.ImpSimulation
{
    public class NeverUseLuckyDie : Strategy
    {
        public override Choice MakeChoice(GameState state)
        {
            var choice = new Choice();
            choice.Action = state.PlayerState.NumDie > 0 ? Choice.Actions.Roll : Choice.Actions.DoNothing;
            return choice;
        }
    }
}