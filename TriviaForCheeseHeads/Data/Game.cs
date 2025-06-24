using PubSub;
using QRCoder;
using System.Collections.Concurrent;
using System.Text;
using TriviaForCheeseHeads.WebUtil;

namespace TriviaForCheeseHeads.Data
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
        public int RoundLength { get; set; }
        public int LeaderboardLength { get; set; }
        public int ResultLength { get; set; }
        public string QrCode { get; set; }
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

        public (bool Success, string message) PickNextQuestion()
        {
            StringBuilder sb = new StringBuilder();
			sb.AppendLine("Pick Next Question START");
            if (_triviaQuestions == null)
            {
				sb.AppendLine("Questions NULL");
                return (false, sb.ToString());
			}
			var questions = _triviaQuestions.Where(q => q.Used == false && q.AskedThisRound == false).ToList();
            if (questions.Count == 0)
			{
				sb.AppendLine("Question Count = 0");
				return (false, sb.ToString());
            }
            if (CurrentRound > NumberOfRounds)
			{
				sb.AppendLine("Current Round > Number of Rounds");
				return (false, sb.ToString());
            }
            var rnd = new Random();
            var idx = rnd.Next(questions.Count);
			sb.AppendLine($"Index {idx}");
			var question = questions[idx];
            question.AskedThisRound = true;
            CurrentQuestion = question;
            CurrentRound++;
            return (true, sb.ToString());
        }
        public void Reset()
        {
            CurrentRound = 1;
            Players.Clear();
        }
        public void SetQRCode(string qrCode)
        {
            QrCode = GenerateQRCodeBase64(qrCode);
        }

        private string GenerateQRCodeBase64(string text)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);

            var qrCodeImage = qrCode.GetGraphic(10);

            return Convert.ToBase64String(qrCodeImage);
        }
    }
}
