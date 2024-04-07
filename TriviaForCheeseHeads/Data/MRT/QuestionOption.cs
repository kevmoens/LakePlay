namespace TriviaForCheeseHeads.Data.MRT
{
    public class QuestionOption
    {
        public int QuestionId { get; set; }
        public int Id { get; set; } = 0;
        public bool IsAnswer { get; set; }
        public string Text { get; set; } = string.Empty;
        public byte[]? Image { get; set; }
    }
}
