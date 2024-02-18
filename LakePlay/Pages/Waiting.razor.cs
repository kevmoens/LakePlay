using LakePlay.Data.Login;
using LakePlay.WebUtil;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Concurrent;

namespace LakePlay.Pages
{
    public partial class Waiting
    {
        [Inject]
        Blazored.LocalStorage.ILocalStorageService? LocalStorage { get; set; }
        [Inject]
        NavigationManager? NavManager {get;set;}
        [Inject]
        ConcurrentDictionary<Guid, UserLogin>? UserLogins { get; set; }
        [Inject]
        JsConsole? JsConsole { get; set; }

        private string UserName { get; set; } = string.Empty;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    UserName = await LocalStorage!.GetItemAsStringAsync("UserName");
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
            var userId = await LocalStorage!.GetItemAsync<string>("UserID");
            if (userId != null )
            {
                UserLogins!.TryRemove(Guid.Parse(userId), out _);
            }

            try
            {
                await LocalStorage.RemoveItemAsync("UserName");
                await LocalStorage.RemoveItemAsync("Email");
                await LocalStorage.RemoveItemAsync("UserID");
            } catch (Exception ex)
            {
                await JsConsole!.LogAsync(ex.Message);
            }
            NavManager!.NavigateTo("/");
        }
    }
}
