using System.Diagnostics;
using Trak_IT.Scripts;
namespace Trak_IT
{
    public partial class LoginPage : ContentPage
    {
        public readonly LocalDBServer _dbServer;
        private readonly UsersProfile _userProfile;
        public LoginPage(LocalDBServer dbServer, UsersProfile userProfile)
        {
            InitializeComponent();
            _dbServer = dbServer;
            _userProfile = userProfile;
        }

        private async void saveButton_Clicked(object sender, EventArgs e)
        {
            await _dbServer.Create(new UserModel
            {
                Username = nameEntry.Text,
                Password = passwordEntry.Text
            });
        }

        private async void loginButton_Clicked(object sender, EventArgs e)
        {
            UserModel user = null;
            try
            {
                user = await _dbServer.GetByUser(nameEntry.Text, passwordEntry.Text);
                if (user != null)
                {
                    _userProfile.Id = user.Id;
                    _userProfile.Name = user.Username;
                    _userProfile.User_role = user.User_Role;
                    _userProfile.Photostring = user.PhotoString;
                    await Shell.Current.GoToAsync("//mainPage");
                }
                else
                {
                    popups.IsVisible = true;
                    popups.Opacity = 100;
                    errorLabel.Text = "Wrong Credentials. Try Again";
                    await Task.Delay(3000);
                    await popups.FadeTo(0, 500, Easing.CubicOut);
                    popups.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                popups.IsVisible = true;
                errorLabel.Text = "Failed";
                Debug.WriteLine(ex);
            }
        }
    }

}
