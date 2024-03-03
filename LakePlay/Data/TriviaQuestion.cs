using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace LakePlay.Data
{
    public class TriviaQuestion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int Difficulty { get; set; } = 0;
        public bool Used { get; set; } = false;
        public bool AskedThisRound { get; set; } = false;
        public List<TriviaQuestionOption> ListOptions { get; set; } = new List<TriviaQuestionOption>();
    }
}
