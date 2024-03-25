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
        ILakePlayRepo<TriviaQuestion>? QuestionRepo { get; set; }

        [Inject]
        LoginVerification? LoginVerify { get; set; }
        [Inject]
        UserLoginRepo? UserLoginRepo { get; set; }
        //[Inject]
        //JsConsole? JsConsole { get; set; }
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


        protected override async Task OnParametersSetAsync()
        {
            if (string.IsNullOrEmpty(QuestionId) == false)
            {
                Title = "Edit";
                var qs = await QuestionRepo!.Where(q => q.Id == QuestionId);
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
            try
            {

                // Validate that one of the ListOptions has IsAnswer set to true
                if (!question.ListOptions.Any(option => option.IsAnswer))
                {
                    await ShowMessageBox("At least one option must be marked as the answer.");
                    return;
                }
                if (question.ListOptions.Count(option => option.IsAnswer) != 1)
                {
                    await ShowMessageBox("Only one option must be marked as the answer.");
                    return;
                }

                // Loop through question.ListOptions and ensure unique IDs
                HashSet<string> optionIds = new();
                foreach (var option in question.ListOptions)
                {
                    if (string.IsNullOrEmpty(option.Id) || optionIds.Contains(option.Id))
                    {
                        option.Id = GenerateUniqueId(optionIds);
                    }
                    optionIds.Add(option.Id);
                }

                if (string.IsNullOrEmpty(QuestionId) == false)
                {
                    QuestionRepo!.Update(question);
                }
                else
                {
                    var saveOptions = question.ListOptions.ToList();
                    question.ListOptions.Clear();
                    QuestionRepo!.Add(question);
                    foreach (var option in saveOptions)
                    {
                        question.ListOptions.Add(option);
                    }
                }

                await QuestionRepo!.SaveAsync();
                Cancel();
            }
            catch (Exception ex)
            {
                //await JsConsole!.LogAsync(ex.Message);
            }
        }
        private async Task ShowMessageBox(string message)
        {
            await JSRuntime.InvokeVoidAsync("alert", message);
        }
        // Method to generate unique ID
        private string GenerateUniqueId(HashSet<string> existingIds)
        {
            int newId = 1;
            while (existingIds.Contains(newId.ToString()))
            {
                newId++;
            }
            return newId.ToString();
        }
        public void Cancel()
        {
            NavManager!.NavigateTo("/questions");
        }
    }
}
