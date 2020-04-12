using System;

namespace EdwardRowe.IdleHeroes.ImpSimulation
{
    public class ImpSimulation
    {
        public static void Main(string[] args)
        {
            var simulation = new ImpSimulation();
            simulation.AverageDieValue();
            simulation.FullStrategy_InRangeLucky();
            simulation.FullStrategy_IntelligentLuckyWithin2();
            simulation.FullStrategy_IntelligentLuckyWithin1();
        }
        
        public void AverageDieValue()
        {
            var total = 0;
            for (int i = 0; i < 1000; ++i)
            {
                var playerState = new PlayerState();
                playerState.NumDie = 1;
                playerState.Strategy = new NeverUseLuckyDie();

                var board = new GameBoard() { NumSpaces = 20, LuckySpace = 19 };
                var gameState = new GameState(board, playerState);
                while (gameState.PlayerHasMovesLeft)
                {
                    gameState.PlayTurn();
                }

                total += gameState.Results.TotalNumberOfSpacesMoved;
            }

            Console.WriteLine($"NumSpaces: {total}. Average: {total / 1000.0f}");
        }

        public void SingleStrategy_InRangeLucky()
        {
            var results = RunStandardGame(new LuckyWhenInRangeOfLucky());
            Console.WriteLine($"NumSpaces: {results.TotalNumberOfSpacesMoved} | D: {results.NumRegularDieUsed}, LD: {results.NumLuckyDieUsed}-{results.NumSpacesGainedFromLuckyDie}");
        }

        public void SingleStrategy_IntelligentLucky()
        {
            var results = RunStandardGame(new IntelligentLucky() { MinimumLuckyRoll = 3 });
            Console.WriteLine($"NumSpaces: {results.TotalNumberOfSpacesMoved} | D: {results.NumRegularDieUsed}, LD: {results.NumLuckyDieUsed}-{results.NumSpacesGainedFromLuckyDie}");
        }

        public void FullStrategy_InRangeLucky()
        {
            var totalResults = new GameResults();
            var numGames = 10000;
            for (int i = 0; i < numGames; ++i)
            {
                var results = RunStandardGame(new LuckyWhenInRangeOfLucky());
                totalResults.TotalNumberOfSpacesMoved += results.TotalNumberOfSpacesMoved;
                totalResults.NumLuckyDieUsed += results.NumLuckyDieUsed;
                totalResults.NumSpacesGainedFromLuckyDie += results.NumSpacesGainedFromLuckyDie;
                totalResults.NumRegularDieUsed += results.NumRegularDieUsed;
            }

            Console.WriteLine($"Always Lucky Averages || NumSpaces: " +
                $"{totalResults.TotalNumberOfSpacesMoved / (float)numGames} " +
                $"| D: {totalResults.NumRegularDieUsed / (float)numGames} " +
                $", LD: {totalResults.NumLuckyDieUsed / (float)numGames} " +
                $"- {totalResults.NumSpacesGainedFromLuckyDie / (float)numGames} ");
        }

        public void FullStrategy_IntelligentLuckyWithin2()
        {
            var totalResults = new GameResults();
            var numGames = 10000;
            for (int i = 0; i < numGames; ++i)
            {
                var results = RunStandardGame(new IntelligentLucky() { MinimumLuckyRoll = 3 });
                totalResults.TotalNumberOfSpacesMoved += results.TotalNumberOfSpacesMoved;
                totalResults.NumLuckyDieUsed += results.NumLuckyDieUsed;
                totalResults.NumSpacesGainedFromLuckyDie += results.NumSpacesGainedFromLuckyDie;
                totalResults.NumRegularDieUsed += results.NumRegularDieUsed;
            }

            Console.WriteLine($"Roll Lucky, Unless within 2|| NumSpaces: " +
                $"{totalResults.TotalNumberOfSpacesMoved / (float)numGames} " +
                $"| D: {totalResults.NumRegularDieUsed / (float)numGames} " +
                $", LD: {totalResults.NumLuckyDieUsed / (float)numGames} " +
                $"- {totalResults.NumSpacesGainedFromLuckyDie / (float)numGames} ");
        }

        public void FullStrategy_IntelligentLuckyWithin1()
        {
            var totalResults = new GameResults();
            var numGames = 10000;
            for (int i = 0; i < numGames; ++i)
            {
                var results = RunStandardGame(new IntelligentLucky() { MinimumLuckyRoll = 2 });
                totalResults.TotalNumberOfSpacesMoved += results.TotalNumberOfSpacesMoved;
                totalResults.NumLuckyDieUsed += results.NumLuckyDieUsed;
                totalResults.NumSpacesGainedFromLuckyDie += results.NumSpacesGainedFromLuckyDie;
                totalResults.NumRegularDieUsed += results.NumRegularDieUsed;
            }

            Console.WriteLine($"Roll Lucky, Unless within 1|| NumSpaces: " +
                $"{totalResults.TotalNumberOfSpacesMoved / (float)numGames} " +
                $"| D: {totalResults.NumRegularDieUsed / (float)numGames} " +
                $", LD: {totalResults.NumLuckyDieUsed / (float)numGames} " +
                $"- {totalResults.NumSpacesGainedFromLuckyDie / (float)numGames} ");
        }

        private GameResults RunStandardGame(Strategy strategy)
        {
            var playerState = new PlayerState();
            playerState.NumDie = 65;
            playerState.NumLuckyDie = 0;
            playerState.Strategy = strategy;

            var board = new GameBoard() { NumSpaces = 20, LuckySpace = 19 };
            var gameState = new GameState(board, playerState);
            while (gameState.PlayerHasMovesLeft)
            {
                gameState.PlayTurn();
            }

            return gameState.Results;
        }

        public class GameResults
        {
            public int TotalNumberOfSpacesMoved { get; set; }
            public int NumRegularDieUsed { get; set; }

            public int NumLuckyDieUsed { get; set; }

            public int NumSpacesGainedFromLuckyDie { get; set; }
        }

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

        public class PlayerState
        {
            public int NumDie { get; set; }
            public int NumLuckyDie { get; set; }
            public Strategy Strategy { get; set; }
        }

        public struct GameBoard
        {
            public int NumSpaces { get; set; }

            public int LuckySpace { get; set; }
        }

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

        public class NeverUseLuckyDie : Strategy
        {
            public override Choice MakeChoice(GameState state)
            {
                var choice = new Choice();
                choice.Action = state.PlayerState.NumDie > 0 ? Choice.Actions.Roll : Choice.Actions.DoNothing;
                return choice;
            }
        }

        public class LuckyWhenInRangeOfLucky : Strategy
        {
            public override Choice MakeChoice(GameState state)
            {
                var choice = new Choice();
                var numSpacesAwayFromLucky = state.GameBoard.LuckySpace - state.PlayerPosition;
                if (state.PlayerState.NumLuckyDie > 0 &&
                    numSpacesAwayFromLucky <= 6 && numSpacesAwayFromLucky > 0)
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
}
