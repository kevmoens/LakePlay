using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace TriviaForCheeseHeads.Data.MRT
{
    public class Import
    {
        public Import(IJSRuntime jSRuntime, ITriviaForCheeseHeadsRepo<TriviaQuestion> repo)
        {
            JSRuntime = jSRuntime;
            this.Repo = repo;

        }
        private IJSRuntime JSRuntime { get; set; }
        private ITriviaForCheeseHeadsRepo<TriviaQuestion> Repo { get; set; }
        public async Task ImportFile(InputFileChangeEventArgs e)
        {
            //Load XML File
            MemoryStream ms = new MemoryStream();
            try
            {
                await e.File.OpenReadStream(long.MaxValue).CopyToAsync(ms);
            }

            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message); // Alert
            }

            ms.Position = 0;
            StreamReader reader = new StreamReader(ms);
            string text = await reader.ReadToEndAsync();

            //Deserialize XML
            List<Question>? questions = DeserializeXml(text);

            if (questions == null || questions.Count == 0)
            {
                await JSRuntime.InvokeVoidAsync("alert", "Failed to load questions!"); // Alert
                return;
            }

            bool anyNewQuestionsPresent = await FindNewQuestions(questions);

            if (!anyNewQuestionsPresent)
                await JSRuntime.InvokeVoidAsync("alert", "Didn't Find any new questions to load!"); // Alert

            //UpdateSearchAndList();
        }

        private List<Question>? DeserializeXml(string text)
        {
            Type tyListQuestion = typeof(List<Question>);
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(tyListQuestion);
            List<Question>? questions = new List<Question>();

            using (StringReader stringReader = new StringReader(text))
            {
                questions = xmlSerializer.Deserialize(stringReader) as List<Question>;
            }

            return questions;
        }

        private async Task<bool> FindNewQuestions(List<Question> questions)
        {
            int newItems = 0;
            foreach (var question in questions)
            {
                if (question.Image != null && question.Image.Length > 0)
                {
                    continue;
                }
                try
                {
                    TriviaQuestion? existQuestion = await GetExistingQuestion(question);

                    if (existQuestion == null)
                    {
                        newItems++;
                        Guid Id = Guid.NewGuid();
                        TriviaQuestion newQuestion = new TriviaQuestion
                        {
                            Text = question.Text,
                            Difficulty = question.Difficulty,
                            Used = question.Used,
                            AskedThisRound = question.AskedThisRound,
                            Id = Id.ToString()
                        };
                        newQuestion.ListOptions = new List<TriviaQuestionOption>();
                        int idx = 0;
                        foreach (var option in question.ListOptions)
                        {
                            if (option.Image != null && option.Image.Length > 0)
                            {
                                continue;
                            }
                            idx++;
                            newQuestion.ListOptions.Add(new TriviaQuestionOption
                            {
                                Text = option.Text,
                                IsAnswer = option.IsAnswer,
                                Id = idx.ToString(),
                                QuestionId = Id.ToString()
                            });
                        }
                        if (newQuestion.ListOptions.Count > 0)
                        {
                            await SaveNewQuestion(newQuestion);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await JSRuntime.InvokeVoidAsync("alert", ex.Message); // Alert
                }
            }

            if (newItems > 0)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Loaded {newItems} new questions!"); // Alert
            }

            return newItems != 0;
        }

        private async Task<TriviaQuestion?> GetExistingQuestion(Question? question)
        {
            var existQuestions = (await Repo.LoadAllAsync())
                .Where(q => (!string.IsNullOrEmpty(q.Text) && !string.IsNullOrEmpty(question.Text) && q.Text.ToLower() == question.Text.ToLower()))
                .ToList();

            TriviaQuestion? existQuestion = null;

            if (existQuestions.Count > 1)
            {
                existQuestion = GetMatchingQuestion(question, existQuestions);
            }
            else
            {
                existQuestion = existQuestions.FirstOrDefault();
            }

            return existQuestion;
        }

        private TriviaQuestion? GetMatchingQuestion(Question question, List<TriviaQuestion> existQuestions)
        {
            TriviaQuestion? matchingQuestion = null;

            foreach (var eQ in existQuestions)
            {
                //var matchOptionCount = GetMatchingOptionCount(question, eQ);

                if (question.Options.Count == eQ.ListOptions.Count)
                {
                    matchingQuestion = eQ;
                    break;
                }
            }

            return matchingQuestion;
        }

        private async Task SaveNewQuestion(TriviaQuestion question)
        {
            //question.Id = 0;
            question.Used = false;

            Repo.Add(question);
            await Repo.SaveAsync();

            //var optionsList = question.ListOptions;
            //foreach (var option in optionsList)
            //{
            //    option.QuestionId = question.Id;
            //}
            //Repo.QuestionOptions.AddRange(optionsList);
            //Repo.SaveChanges()
        }

    }
}
