using PubSub;

namespace LakePlay.Data
{
    public class Game
    {
        private Hub _hub;
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
        public TriviaQuestion? CurrentQuestion { get; set; }

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
            switch (State)
            {
                case GameState.NotSet:
                    return "";
                case GameState.AboutToStart:
                    return "/waiting";
                case GameState.InRound:
                    return "/round";
                case GameState.Meme:
                    return "/meme";
                case GameState.Leaderboard:
                    return "/leaderboard";
                case GameState.GameOver:
                    return "/gameover";
                default:
                    return "";
            }
        }

        private string CheckForNavigateLocationAdmin(GameState currentLocation)
        {
            if (currentLocation == State)
            {
                return ""; //Do Nothing and stay on the same page
            }
            switch (currentLocation)
            {
                case GameState.NotSet:
                    return "/admin";
                case GameState.AboutToStart:
                    return "/adminintro";
                case GameState.InRound:
                    return "/adminround";
                case GameState.Meme:
                    return "/adminmeme";
                case GameState.Leaderboard:
                    return "/adminleaderboard";
                case GameState.GameOver:
                    return "/admingameover";
                default:
                    return "";
            }
        }

        public void PickNextQuestion()
        {
            var questions = _triviaQuestions.Where(q => q.Used == false && q.AskedThisRound == false).ToList();
            if (questions.Count == 0)
            {

                return;
            }
            var rnd = new Random();
            var question = questions[rnd.Next(questions.Count)];
            question.AskedThisRound = true;
            CurrentQuestion = question;
        }
    }
}
