using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LakePlay.Data
{
    public class TriviaQuestionOption
    {
        //[ForeignKey("TriviaQuestion")]
        public string QuestionId { get; set; }  = string.Empty;
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = string.Empty;
        public bool IsAnswer { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
