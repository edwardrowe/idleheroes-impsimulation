namespace EdwardRowe.IdleHeroes.ImpSimulation
{
    public abstract class Strategy
    {
        public abstract Choice MakeChoice(GameState state);

        public struct Choice
        {
            public Actions Action { get; set; }

            public int LuckyDieValue { get; set; }

            public enum Actions
            {
                DoNothing,
                Roll,
                RollLucky
            }
        }
    }
}