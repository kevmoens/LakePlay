using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using LakePlay.Data;
using System;
using LakePlay.Data.Login;
using LakePlay.WebUtil;
using System.Collections.Concurrent;
using Microsoft.Azure.Cosmos;

namespace LakePlay.Pages
{
    public partial class Admin
    {
        [Inject]
        Blazored.LocalStorage.ILocalStorageService? LocalStorage { get; set; }
        [Inject]
        LakePlayContext? Repo { get; set; }
        [Inject]
        JsConsole? JsConsole { get; set; }
        [Inject]
        NavigationManager? NavManager { get; set; }
        [Inject]
        ConcurrentDictionary<Guid, UserLogin>? UserLogins { get; set; }
        [Inject]
        LoginVerification? LoginVerify { get; set; }
        [Inject]
        UserLoginRepo? UserLoginRepo { get; set; }
        private UserLogin? User { get; set; }

        private int _numberOfRounds = 1;
        public int NumberOfRounds { get { return _numberOfRounds; } set { _numberOfRounds = value; SaveRounds(); } }
        private int _currentRound = 1;
        public int CurrentRound { get { return _currentRound; } set { _currentRound = value; } }

        protected override async Task OnInitializedAsync()
        {
            var numOfRounds = await LocalStorage!.GetItemAsync<string>("NumberOfRounds");

            if (int.TryParse(numOfRounds, out int numberOfRounds))
            {
                _numberOfRounds = numberOfRounds;
            }
            var curRound = await LocalStorage.GetItemAsync<string>("CurrentRound");

            if (int.TryParse(curRound, out int currentRound))
            {
                _currentRound = currentRound;
            }

            StateHasChanged();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    User = await UserLoginRepo!.Load();
                    if (LoginVerify!.VerifyLogin(User, true) == false)
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
        async void SaveRounds()
        {
            await LocalStorage!.SetItemAsync("NumberOfRounds", NumberOfRounds.ToString());
        }
        async void OnResetRounds()
        {
            CurrentRound = 1;
            foreach (var question in Repo!.Questions!.Where(q => q.AskedThisRound == true))
            {
                question.AskedThisRound = false;
                Repo.Questions!.Update(question);
            }
            Repo.SaveChanges();

            await LocalStorage!.SetItemAsync("CurrentRound", CurrentRound.ToString());
            StateHasChanged();
        }


        void OnQuestions()
        {
            NavManager!.NavigateTo("/questions");
        }
    }
}
