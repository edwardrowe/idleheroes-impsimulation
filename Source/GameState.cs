namespace EdwardRowe.IdleHeroes.ImpSimulation
{
    public class GameState
    {
        public GameBoard GameBoard { get; private set; }

        public PlayerState PlayerState { get; private set; }

        public int PlayerPosition { get; private set; }

        public GameResults Results { get; private set; }

        public bool PlayerHasMovesLeft
        {
            get
            {
                var choice = this.PlayerState.Strategy.MakeChoice(this);
                return choice.Action != Strategy.Choice.Actions.DoNothing &&
                    ((choice.Action == Strategy.Choice.Actions.Roll && this.PlayerState.NumDie > 0) ||
                    (choice.Action == Strategy.Choice.Actions.RollLucky && this.PlayerState.NumLuckyDie > 0));
            }
        }

        public GameState(GameBoard board, PlayerState startingPlayerState)
        {
            this.GameBoard = board;
            this.PlayerPosition = 0;
            this.PlayerState = startingPlayerState;
            this.Results = new GameResults();
        }

        public void PlayTurn()
        {
            var choice = this.PlayerState.Strategy.MakeChoice(this);
            if (choice.Action == Strategy.Choice.Actions.RollLucky)
            {
                this.MovePlayer(choice.LuckyDieValue);
                this.PlayerState.NumLuckyDie--;
                Results.NumLuckyDieUsed += 1;
                Results.NumSpacesGainedFromLuckyDie += choice.LuckyDieValue;
            }
            else if (choice.Action == Strategy.Choice.Actions.Roll)
            {
                var random = new System.Random();
                this.MovePlayer(random.Next(1, 7));
                this.PlayerState.NumDie--;
                Results.NumRegularDieUsed++;
            }
        }

        public void MovePlayer(int spaces)
        {
            if (this.GameBoard.NumSpaces <= 0)
            {
                this.PlayerPosition = 0;
                return;
            }

            this.PlayerPosition += spaces;
            Results.TotalNumberOfSpacesMoved += spaces;
            this.PlayerPosition %= this.GameBoard.NumSpaces;

            this.AwardPoints();
        }

        private void AwardPoints()
        {
            if (this.PlayerPosition == this.GameBoard.LuckySpace)
            {
                this.PlayerState.NumLuckyDie++;
            }
        }
    }
}