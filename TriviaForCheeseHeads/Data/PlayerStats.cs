namespace TriviaForCheeseHeads.Data
{
    public class PlayerStats
    {
        public int Score { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Dictionary<int, int> RoundScores { get; set; } = new();

        public void AddScore(int round, int score)
        {
            if (RoundScores.ContainsKey(round))
            {
                return;
            }

            RoundScores.Add(round, score);
            
            Score += score;
        }
    }
}
