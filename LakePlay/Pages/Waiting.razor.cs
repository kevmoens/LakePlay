using LakePlay.Data;
using LakePlay.Data.Login;
using LakePlay.WebUtil;
using Microsoft.AspNetCore.Components;
using PubSub;
using System;
using System.Collections.Concurrent;

namespace LakePlay.Pages
{
    public partial class Waiting
    {
        [Inject]
        NavigationManager? NavManager {get;set;}
        [Inject]
        ConcurrentDictionary<Guid, UserLogin>? UserLogins { get; set; }
        //[Inject]
        //JsConsole? JsConsole { get; set; }
        [Inject]
        LoginVerification? LoginVerify { get; set; }
        [Inject]
        UserLoginRepo? UserLoginRepo { get; set; }
        [Inject] 
        Game? Game { get; set; }
        [Inject]
        Hub? Hub { get; set; }
        private UserLogin? User { get; set; }
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

                    var navigateLocation = Game!.CheckForNavigateLocation(false, GameState.AboutToStart);
                    if (navigateLocation != "")
                    {
                        NavManager!.NavigateTo(navigateLocation);
                        return;
                    }
                    Hub!.Subscribe<GameStateChanged>((message) =>
                    {
                        var navigateLocation = Game!.CheckForNavigateLocation(false, GameState.AboutToStart);
                        if (navigateLocation != "")
                        {
                            NavManager!.NavigateTo(navigateLocation);
                        }
                    });
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
            } catch (Exception ex)
            {
                //await JsConsole!.LogAsync(ex.Message);
            }
            NavManager!.NavigateTo("/");
        }
    }
}
