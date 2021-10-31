using Acr.UserDialogs;
using Firebase.Auth;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using StudentSolution.Models;
using StudentSolution.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Xamarin.Essentials;

namespace StudentSolution.ViewModels
{
    public class LoginPageViewModel : BindableBase
    {
        INavigationService _navigationService;
        public string Email { get; set; }
        public string Password { get; set; }
        public DelegateCommand ToRegisterCommand { get; set; }
        public DelegateCommand LoginCommand { get; set; }
        public LoginPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            ToRegisterCommand = new DelegateCommand(OpenRegisterPage);
            LoginCommand = new DelegateCommand(OnLogin);
        }

        private void OnLogin()
        {

            UserDialogs.Instance.ShowLoading("Logging in...");

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                UserDialogs.Instance.Loading().Dispose();
                UserDialogs.Instance.Alert("You are not connected to internet", "Alert", "Ok");
                return;
            }

            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
            {
                UserLogin user_login_details = new UserLogin()
                {
                    EmailAddress = Email,
                    Password = Password
                };
                try
                {
                    MailAddress m = new MailAddress(user_login_details.EmailAddress);

                    if (user_login_details.Password.Length >= 6)
                    {
                        AuthenticateUser(user_login_details);
                    }
                    else
                    {
                        UserDialogs.Instance.Loading().Dispose();
                        UserDialogs.Instance.Toast("Password must have 6 or more characters");
                    }
                }
                catch (FormatException)
                {
                    UserDialogs.Instance.Loading().Dispose();
                    UserDialogs.Instance.Toast("Enter valid email address");
                }

            }
            else
            {
                UserDialogs.Instance.Loading().Dispose();
                UserDialogs.Instance.Toast("Please enter all details");
            }
        }

        private async void AuthenticateUser(UserLogin userLogin)
        {

            // firebase
            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Constants.WebAPIkey));
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(userLogin.EmailAddress, userLogin.Password);
                if (auth != null)
                {

                    var content = await auth.GetFreshAuthAsync();

                    var userId = auth.User.LocalId;


                    DatabaseServices databaseServices = new DatabaseServices();
                    UserRegister user = await databaseServices.GetUser(userId);
                    if (user != null)
                    {

                        var serializedTokenContent = JsonConvert.SerializeObject(content);
                        var serializedUserDetails = JsonConvert.SerializeObject(user);

                        // this saves the returned value into preferences
                        TokenService.SetTokenPreference(serializedTokenContent);
                        Preferences.Set(Constants.UserDetails, serializedUserDetails);

                        await _navigationService.NavigateAsync("/NavigationPage/MainPage");
                        UserDialogs.Instance.Loading().Dispose();
                        UserDialogs.Instance.Toast("Login successful");
                    }
                    else
                    {
                        UserDialogs.Instance.Loading().Dispose();
                        await UserDialogs.Instance.AlertAsync("Something went wrong, please try again", "Error", "OK");
                        return;
                    }

                }
                else
                {
                    UserDialogs.Instance.Loading().Dispose();
                    await UserDialogs.Instance.AlertAsync("Something went wrong, please try again", "Error", "OK");
                    return;
                }



            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Loading().Dispose();
                await App.Current.MainPage.DisplayAlert("Alert", "Invalid email or password", "OK");
            }
        }

        private void OpenRegisterPage()
        {
            _navigationService.NavigateAsync("/RegisterPage");
        }
    }
}
