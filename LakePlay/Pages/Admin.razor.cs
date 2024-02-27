﻿using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using LakePlay.Data;
using System;
using LakePlay.Data.Login;
using LakePlay.WebUtil;
using System.Collections.Concurrent;
using Microsoft.Azure.Cosmos;
using PubSub;
using Microsoft.EntityFrameworkCore;

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
        [Inject]
        Game? Game { get; set; }

        [Inject]
        List<TriviaQuestion>? GameQuestions { get; set; }
        [Inject]
        LakePlayContext? DbContext { get; set; }

        private UserLogin? User { get; set; }

        private int _numberOfRounds = 1;
        public int NumberOfRounds { get { return _numberOfRounds; } set { _numberOfRounds = value; SaveRounds(); } }

        protected override async Task OnInitializedAsync()
        {
            var numOfRounds = await LocalStorage!.GetItemAsync<string>("NumberOfRounds");

            if (int.TryParse(numOfRounds, out int numberOfRounds))
            {
                _numberOfRounds = numberOfRounds;
                Game!.NumberOfRounds = numberOfRounds;
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

                    if (GameQuestions!.Count == 0 && DbContext?.Questions != null)
                    {
                        var questions = await DbContext.Questions.ToListAsync();
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
        void OnResetGameStatus()
        {
            Game!.ChangeState(GameState.NotSet);

            Game.CurrentRound = 1;
            foreach (var question in Repo!.Questions!.Where(q => q.AskedThisRound == true))
            {
                question.AskedThisRound = false;
                Repo.Questions!.Update(question);
            }
            Repo.SaveChanges();

            StateHasChanged();
        }
    }
}
