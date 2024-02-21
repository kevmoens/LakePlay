using LakePlay.Data;
using LakePlay.Data.Login;
using LakePlay.WebUtil;
using Microsoft.AspNetCore.Components;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Collections.Concurrent;

namespace LakePlay.Pages
{
    public partial class Questions
    {

        [Inject]
        NavigationManager? NavManager { get; set; }
        protected string SearchString { get; set; } = string.Empty;
        [Inject]
        public IJSRuntime? JSRuntime { get; set;  }

        [Inject]
        public LakePlayContext? Context { get; set; }

        [Inject]
        LoginVerification? LoginVerify { get; set; }
        [Inject]
        UserLoginRepo? UserLoginRepo { get; set; }
        [Inject]
        JsConsole? JsConsole { get; set; }
        [Inject]
        ConcurrentDictionary<Guid, UserLogin>? UserLogins { get; set; }



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
                    await JsConsole!.LogAsync(ex.Message);
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
                await JsConsole!.LogAsync(ex.Message);
            }
            NavManager!.NavigateTo("/");
        }
        void OnBack()
        {
            NavManager!.NavigateTo("/admin");
        }

        protected override async Task OnInitializedAsync()
        {
            searchQuestionData = await Context!.Questions!.ToListAsync();
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
            Context!.Questions!.Update(question);
            await Context!.SaveChangesAsync();
            StateHasChanged();
        }
    }
}
