namespace LakePlay.Data
{
    public class TriviaQuestionOption
    {
        public string QuestionId { get; set; }  = string.Empty;
        public string Id { get; set; } = string.Empty;
        public bool IsAnswer { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
