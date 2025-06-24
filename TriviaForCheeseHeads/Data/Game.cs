using PubSub;
using QRCoder;
using System.Collections.Concurrent;
using TriviaForCheeseHeads.WebUtil;

namespace TriviaForCheeseHeads.Data
{
    public class Game
    {
        private readonly Hub _hub;
        private readonly List<TriviaQuestion> _triviaQuestions;
        private readonly JsConsole _jsConsole;

        public Game(Hub hub, List<TriviaQuestion> triviaQuestions, JsConsole jsConsole)
        {
            _hub = hub;
            _triviaQuestions = triviaQuestions;
            _jsConsole = jsConsole;
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

        public async Task<bool> PickNextQuestion()
        {
            await _jsConsole.LogAsync("Pick Next Question START");
            if (_triviaQuestions == null)
            {
				await _jsConsole.LogAsync("Questions NULL");
                return false;
			}
			var questions = _triviaQuestions.Where(q => q.Used == false && q.AskedThisRound == false).ToList();
            if (questions.Count == 0)
			{
				await _jsConsole.LogAsync("Question Count = 0");
				return false;
            }
            if (CurrentRound > NumberOfRounds)
			{
				await _jsConsole.LogAsync("Current Round > Number of Rounds");
				return false;
            }
            var rnd = new Random();
            var idx = rnd.Next(questions.Count);
			await _jsConsole.LogAsync($"Index {idx}");
			var question = questions[idx];
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
