using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace TriviaForCheeseHeads.Data.MRT
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public byte[]? Image { get; set; }
        public int Difficulty { get; set; } = 0;
        public bool Used { get; set; } = false;
        public bool AskedThisRound { get; set; } = false;

        [XmlIgnore]
        public virtual ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
        [NotMapped] //Used for XML Serializeation
        public List<QuestionOption> ListOptions { get; set; } = new List<QuestionOption>();
    }
}
