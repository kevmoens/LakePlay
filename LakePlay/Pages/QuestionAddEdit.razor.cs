using LakePlay.Data;
using LakePlay.Data.Login;
using LakePlay.WebUtil;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Collections.Concurrent;

namespace LakePlay.Pages
{
    public partial class QuestionAddEdit
    {

        [Inject]
        NavigationManager? NavManager { get; set; }
        protected string SearchString { get; set; } = string.Empty;
        [Inject]
        public IJSRuntime? JSRuntime { get; set; }

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



        [Parameter]
        public string? QuestionId { get; set; }

        private UserLogin? User { get; set; }

        protected string Title = "Add";
        protected TriviaQuestion question = new();


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


        protected override async Task OnParametersSetAsync()
        {
            if (string.IsNullOrEmpty(QuestionId) == false)
            {
                Title = "Edit";
                var qs = await Context!.Questions!.Where(q => q.Id == QuestionId).ToListAsync();
                if (qs.Count > 0)
                {
                    question = qs[0];
                }
            }
            else
            {
                question.ListOptions.Add(new TriviaQuestionOption());
                question.ListOptions.Add(new TriviaQuestionOption());
                question.ListOptions.Add(new TriviaQuestionOption());
                question.ListOptions.Add(new TriviaQuestionOption());
            }
        }

        private void OnAddOption()
        {
            question.ListOptions.Add(new TriviaQuestionOption());
        }
        private void OnDeleteOption(TriviaQuestionOption option)
        {
            question.ListOptions.Remove(option);
            StateHasChanged();
        }
        protected async Task SaveQuestion()
        {
            if (string.IsNullOrEmpty(QuestionId) == false)
            {
                Context!.Questions!.Update(question);              
            }
            else
            {
                Context!.Questions!.Add(question);
            }
            await Context!.SaveChangesAsync();
            Cancel();
        }

        public void Cancel()
        {
            NavManager!.NavigateTo("/questions");
        }
    }
}
