using Blazored.LocalStorage;

namespace TriviaForCheeseHeads.Data.Login
{
    public class UserLoginRepo
    {
        private readonly ILocalStorageService _localStorage;

        public UserLoginRepo(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }
        public async Task<UserLogin> Load()
        {
            UserLogin userLogin = new()
            {
                UserName = await _localStorage!.GetItemAsStringAsync("UserName"),
                Email = await _localStorage.GetItemAsStringAsync("Email")
            };
            var userId = await _localStorage.GetItemAsStringAsync("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                userLogin.UserId = Guid.Parse(userId);
            }
            return userLogin;
        }

        public async Task Save(UserLogin userLogin)
        {
            await _localStorage!.SetItemAsStringAsync("UserName", userLogin.UserName);
            await _localStorage.SetItemAsStringAsync("Email", userLogin.Email);
            await _localStorage.SetItemAsStringAsync("UserId", userLogin.UserId.ToString());
        }
        public async Task Remove()
        {
            await _localStorage!.RemoveItemAsync("UserName");
            await _localStorage.RemoveItemAsync("Email");
            await _localStorage.RemoveItemAsync("UserId");
        }

    }
}
