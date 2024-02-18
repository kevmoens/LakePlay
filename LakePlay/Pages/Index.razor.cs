using LakePlay.Data.Login;
using Microsoft.AspNetCore.Components;
using LakePlay.WebUtil;
using System.Collections.Concurrent;

namespace LakePlay.Pages
{
    public partial class Index
    {

        [Inject]
        ConcurrentDictionary<Guid, UserLogin>? UserLogins { get; set; }
        [Inject]
        Blazored.LocalStorage.ILocalStorageService? LocalStorage { get; set; }
        [Inject]
        JsConsole? JsConsole { get; set; }
        [Inject]
        NavigationManager? NavManager { get; set; }


        private readonly UserLogin userLogin = new();
        private string _validationMessage = string.Empty;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender == false)
            {
                return;
            }

            try
            {
                userLogin.UserName = await LocalStorage!.GetItemAsStringAsync("UserName");
                userLogin.Email = await LocalStorage.GetItemAsStringAsync("Email");
                var userId = await LocalStorage.GetItemAsStringAsync("UserId");
                if (!string.IsNullOrEmpty(userId))
                {
                    userLogin.UserId = Guid.Parse(userId);
                }

                if (!string.IsNullOrEmpty(userLogin.UserName) && !string.IsNullOrEmpty(userLogin.Email) && userLogin.UserId != Guid.Empty)
                {
                    if (UsePreviousLogin())
                    {
                        return;
                    }
                }

                userLogin.UserId = Guid.Empty;

                StateHasChanged();
            }
            catch (Exception ex)
            {
                await JsConsole!.LogAsync(ex.Message);
            }
            
        }

        private bool UsePreviousLogin()
        {
            if (UserLogins!.TryGetValue(userLogin.UserId, out UserLogin? existUserLogin))
            {

                if (existUserLogin == userLogin)
                {
                    Navigate();
                    return true;
                }
            }

            if (UserLogins!.Values.Any(u => u.UserName.Equals(userLogin.UserName, StringComparison.OrdinalIgnoreCase)))
            {
                //doesn't exit, let user login
                return false;   
            }

            if (UserLogins!.TryAdd(userLogin.UserId, userLogin))
            {
                Navigate();
                return true;
            }

            return false;
        }

        private bool ValidateForm()
        {
            if (UserLogins!.ContainsKey(userLogin.UserId))
            {
                _validationMessage = "User already exists";
                return false;
            }
            // Perform custom validation logic here
            //Azure AI Content Safety avoid bad words
            // Return true if the form is valid, false otherwise
            return true;
        }

        private async void OnStart()
        {
            try {                 

                if (!ValidateForm())
                {
                    return;
                }

                if (userLogin.UserId == Guid.Empty)
                {
                    userLogin.UserId = Guid.NewGuid();
                }

                // Perform actions when the form is valid
                await LocalStorage!.SetItemAsStringAsync("UserName", userLogin.UserName);
                await LocalStorage.SetItemAsStringAsync("Email", userLogin.Email);
                await LocalStorage.SetItemAsStringAsync("UserId", userLogin.UserId.ToString());

                if (UserLogins!.Values.Any(u => u.UserName.Equals(userLogin.UserName, StringComparison.OrdinalIgnoreCase)))
                {
                    _validationMessage = "User already exists";
                    return;
                }

                Navigate();
            }
            catch (Exception ex)
            {
                await JsConsole!.LogAsync(ex.Message);
            }
        }

        private void Navigate()
        {
            if (userLogin.UserName.Equals("MRamage338", StringComparison.OrdinalIgnoreCase) 
                && userLogin.Email.Equals("matt@triviaforcheeseheads.com", StringComparison.OrdinalIgnoreCase))
            {
                NavManager!.NavigateTo("/Admin");
                return;
            }

            //TODO Check if game is in progress....
            //If so go to current page...

            //If Game hasn't started then go to Waiting Page
            NavManager!.NavigateTo("/Waiting");
        }
    }
}
