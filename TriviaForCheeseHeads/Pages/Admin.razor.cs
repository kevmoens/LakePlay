using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TriviaForCheeseHeads.Data;
using System;
using TriviaForCheeseHeads.Data.Login;
using TriviaForCheeseHeads.WebUtil;
using System.Collections.Concurrent;
using Microsoft.Azure.Cosmos;
using PubSub;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace TriviaForCheeseHeads.Pages
{
    public partial class Admin
    {
        [Inject]
        Blazored.LocalStorage.ILocalStorageService? LocalStorage { get; set; }
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
        [Inject]
        Game? Game { get; set; }

        [Inject]
        List<TriviaQuestion>? GameQuestions { get; set; }

        [Inject]
        ITriviaForCheeseHeadsRepo<TriviaQuestion>? QuestionRepo { get; set; }

        private UserLogin? User { get; set; }

        private int _numberOfRounds = 1;
        public int NumberOfRounds { get { return _numberOfRounds; } set { _numberOfRounds = value; SaveRounds(); } }

        protected override async Task OnInitializedAsync()
        {
            Game!.SetQRCode(NavManager!.BaseUri);
            var numOfRounds = await LocalStorage!.GetItemAsync<string>("NumberOfRounds");

            if (int.TryParse(numOfRounds, out int numberOfRounds))
            {
                _numberOfRounds = numberOfRounds;
                Game!.NumberOfRounds = numberOfRounds;
            }


            var resultLengthstr = await LocalStorage!.GetItemAsync<string>("ResultLength");

            if (int.TryParse(resultLengthstr, out int resultLength))
            {
                Game!.ResultLength = resultLength;
            }

            var leaderboardLengthstr = await LocalStorage!.GetItemAsync<string>("LeaderboardLength");

            if (int.TryParse(leaderboardLengthstr, out int leaderboardLength))
            {
                Game!.LeaderboardLength = leaderboardLength;
            }


            var roundLengthstr = await LocalStorage!.GetItemAsync<string>("roundLength");

            if (int.TryParse(roundLengthstr, out int roundLength))
            {
                Game!.RoundLength = roundLength;
            }

            bool saveNeeded = false;
            if (Game!.ResultLength == 0)
            {
                Game!.ResultLength = 8;
                saveNeeded = true;
            }
            if (Game!.LeaderboardLength == 0)
            {
                Game!.LeaderboardLength = 10;
                saveNeeded = true;
            }
            if (Game!.RoundLength == 0)
            {
                Game!.RoundLength = 60;
                saveNeeded = true;
            }
            if (saveNeeded)
            {
                await LocalStorage!.SetItemAsync("Game", Game);
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

                    if (GameQuestions!.Count == 0 && QuestionRepo != null)
                    {
                        var questions = await QuestionRepo.LoadAllAsync();
                        GameQuestions.AddRange(questions);
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
            Game!.NumberOfRounds = NumberOfRounds;
            await LocalStorage!.SetItemAsync("NumberOfRounds", NumberOfRounds.ToString());
        }


        void OnQuestions()
        {
            NavManager!.NavigateTo("/questions");
        }
        void OnStartGame()
        {
            Game!.ChangeState(GameState.AboutToStart);
            NavManager!.NavigateTo("/adminintro");
        }
        async void OnResetGameStatus()
        {
            try
            {
                Game!.ChangeState(GameState.NotSet);

                Game.CurrentRound = 1;
                foreach (var question in await QuestionRepo!.Where(q => q.AskedThisRound == true || q.Used == true))
                {
                    question.AskedThisRound = false;
                    question.Used = false;
                    QuestionRepo!.Update(question);
                }

                await QuestionRepo.SaveAsync();

                StateHasChanged();
            }
            catch (Exception ex)
            {
                await JsConsole!.LogAsync(ex.Message);
            }
        }
        async void OnSave()
        {
            try
            {
                await LocalStorage!.SetItemAsync("RoundLength", Game!.RoundLength.ToString());
                await LocalStorage!.SetItemAsync("LeaderboardLength", Game!.LeaderboardLength.ToString());
                await LocalStorage!.SetItemAsync("ResultLength", Game!.ResultLength.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
