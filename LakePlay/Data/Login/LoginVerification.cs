using Blazored.LocalStorage;

namespace LakePlay.Data.Login
{
    public class LoginVerification
    {
        private readonly UserLoginRepo _userLoginRepo;

        public LoginVerification(UserLoginRepo userLoginRepo)
        {
            _userLoginRepo = userLoginRepo;
        }
        public async Task<bool> VerifyLogin(bool isAdmin = false)
        {
            var user = await _userLoginRepo.Load();
            return VerifyLogin(user, isAdmin);
        }

        public bool VerifyLogin(UserLogin userLogin, bool isAdmin = false)
        {

            if (string.IsNullOrEmpty(userLogin.UserName) || string.IsNullOrEmpty(userLogin.Email) || userLogin.UserId == Guid.Empty)
            {
                return false;
            }
            if (isAdmin == false)
            {
                return true;
            }
            return IsAdmin(userLogin);
        }

        public static bool IsAdmin(UserLogin userLogin)
        {
            return userLogin.UserName.Equals("MRamage338", StringComparison.OrdinalIgnoreCase)
                    && userLogin.Email.Equals("matt@triviaforcheeseheads.com", StringComparison.OrdinalIgnoreCase);
        }
    }
}
