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
    }
}
