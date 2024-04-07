using TriviaForCheeseHeads.Data;
using TriviaForCheeseHeads.Data.Login;
using TriviaForCheeseHeads.WebUtil;
using Microsoft.AspNetCore.Components;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Collections.Concurrent;
using TriviaForCheeseHeads.Data.MRT;
using Microsoft.AspNetCore.Components.Forms;

namespace TriviaForCheeseHeads.Pages
{
    public partial class Questions
    {

        [Inject]
        NavigationManager? NavManager { get; set; }
        protected string SearchString { get; set; } = string.Empty;
        [Inject]
        public IJSRuntime? JSRuntime { get; set;  }

        [Inject]
        ITriviaForCheeseHeadsRepo<TriviaQuestion>? QuestionRepo { get; set; }

        [Inject]
        LoginVerification? LoginVerify { get; set; }
        [Inject]
        UserLoginRepo? UserLoginRepo { get; set; }
        //[Inject]
        //JsConsole? JsConsole { get; set; }
        [Inject]
        ConcurrentDictionary<Guid, UserLogin>? UserLogins { get; set; }
        [Inject]
        Import? Import { get; set; } 



        private UserLogin? User { get; set; }

        protected List<TriviaQuestion>? questionList = null;
        protected List<TriviaQuestion> searchQuestionData = new();
        protected TriviaQuestion? question = new();
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    User = await UserLoginRepo!.Load();
                    if (LoginVerify!.VerifyLogin(User, false) == false)
                    {
                        NavManager!.NavigateTo("/");
                        return;
                    }
                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    //await JsConsole!.LogAsync(ex.Message);
                }
            }
        }
        async void OnLogout()
        {

            UserLogins!.TryRemove(User!.UserId, out _);

            try
            {
                await UserLoginRepo!.Remove();
            }
            catch (Exception ex)
            {
                //await JsConsole!.LogAsync(ex.Message);
            }
            NavManager!.NavigateTo("/");
        }
        void OnBack()
        {
            NavManager!.NavigateTo("/admin");
        }

        protected override async Task OnInitializedAsync()
        {
            searchQuestionData = await QuestionRepo!.LoadAllAsync();
            questionList = searchQuestionData;
        }

        protected void FilterQuestion()
        {
            if (!string.IsNullOrEmpty(SearchString))
            {
                questionList = searchQuestionData
                    .Where(x => x.Text.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }
            else
            {
                questionList = searchQuestionData;
            }
            StateHasChanged();
        }

        public void ResetSearch()
        {
            SearchString = string.Empty;
            questionList = searchQuestionData;
            StateHasChanged();
        }

        async Task OnUnused(TriviaQuestion question)
        {
            question.Used = false;
            QuestionRepo!.Update(question);
            await QuestionRepo!.SaveAsync();
            StateHasChanged();
        }
        async Task OnDelete(string Id)
        {
            var question = await QuestionRepo!.Where(q => q.Id == Id);
            if (question.Count > 0)
            {
                QuestionRepo!.Remove(question[0]);
                await QuestionRepo!.SaveAsync();
                searchQuestionData = await QuestionRepo!.LoadAllAsync();
                questionList = searchQuestionData;
                StateHasChanged();
            }
        }

        async Task OnImport(InputFileChangeEventArgs e)
        {
            await Import!.ImportFile(e);
            NavManager!.NavigateTo(NavManager!.Uri, forceLoad: true);
        }
    }
}
