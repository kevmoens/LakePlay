using PubSub;
using System.Collections.Concurrent;

namespace LakePlay.Data
{
    public class Game
    {
        private readonly Hub _hub;
        private readonly List<TriviaQuestion> _triviaQuestions;

        public Game(Hub hub, List<TriviaQuestion> triviaQuestions)
        {
            _hub = hub;
            _triviaQuestions = triviaQuestions;
        }
        private GameState _State = GameState.NotSet;
        public GameState State { get { return _State; } }
        public int CurrentRound { get; set; }
        public int NumberOfRounds { get; set; }
        public string GameName { get; set; } = string.Empty;
        public TriviaQuestion? CurrentQuestion { get; set; }
        public ConcurrentDictionary<string, PlayerStats> Players { get; set; } = new ConcurrentDictionary<string, PlayerStats>(StringComparer.InvariantCultureIgnoreCase);

        public void ChangeState(GameState newState)
        {
            _State = newState;
            _hub.Publish<GameStateChanged>(new GameStateChanged { NewState = newState });
        }

        public string CheckForNavigateLocation(bool isAdmin, GameState currentLocation)
        {
            if (isAdmin)
            {
                return CheckForNavigateLocationAdmin(currentLocation);
            }
            return CheckForNavigateLocationUser(currentLocation);
        }

        private string CheckForNavigateLocationUser(GameState currentLocation)
        {
            if (currentLocation == State)
            {
                return ""; //Do Nothing and stay on the same page
            }
            return State switch
            {
                GameState.NotSet => "",
                GameState.AboutToStart => "/waiting",
                GameState.InRound => "/round",
                GameState.Meme => "/meme",
                GameState.Leaderboard => "/leaderboard",
                GameState.GameOver => "/gameover",
                _ => "",
            };
        }

        private string CheckForNavigateLocationAdmin(GameState currentLocation)
        {
            if (currentLocation == State)
            {
                return ""; //Do Nothing and stay on the same page
            }
            return currentLocation switch
            {
                GameState.NotSet => "/admin",
                GameState.AboutToStart => "/adminintro",
                GameState.InRound => "/adminround",
                GameState.Meme => "/adminresult",
                GameState.Leaderboard => "/adminleaderboard",
                GameState.GameOver => "/admingameover",
                _ => "",
            };
        }

        public bool PickNextQuestion()
        {
            var questions = _triviaQuestions.Where(q => q.Used == false && q.AskedThisRound == false).ToList();
            if (questions.Count == 0)
            {
                return false;
            }
            if (CurrentRound > NumberOfRounds)
            {
                return false;
            }
            var rnd = new Random();
            var question = questions[rnd.Next(questions.Count)];
            question.AskedThisRound = true;
            CurrentQuestion = question;
            CurrentRound++;
            return true;
        }
        public void Reset()
        {
            CurrentRound = 1;
            Players.Clear();
        }
    }
}
