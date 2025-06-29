﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TriviaForCheeseHeads.Data
{
    public class TriviaQuestionOption
    {
        public string QuestionId { get; set; }  = string.Empty;
        public string Id { get; set; } = string.Empty;
        public bool IsAnswer { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
