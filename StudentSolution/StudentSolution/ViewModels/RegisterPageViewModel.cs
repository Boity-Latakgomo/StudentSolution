using Acr.UserDialogs;
using Firebase.Auth;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using StudentSolution.Models;
using StudentSolution.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Xamarin.Essentials;

namespace StudentSolution.ViewModels
{
    public class RegisterPageViewModel : BindableBase
    {
        INavigationService _navigationService;
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DelegateCommand ToLoginCommand { get; set; }
        public DelegateCommand RegisterCommand { get; set; }
        public RegisterPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            ToLoginCommand = new DelegateCommand(OpenLoginPage);
            RegisterCommand = new DelegateCommand(OnRegister);
        }

        private void OnRegister()
        {
            UserDialogs.Instance.Loading();

            if (!Regex.IsMatch(Name, @"^[a-zA-Z]+$") || !Regex.IsMatch(Surname, @"^[a-zA-Z]+$"))
            {
                UserDialogs.Instance.Toast("Your entered an invalid name or surname");
                UserDialogs.Instance.Loading().Dispose();
                return;
            }
            // Check for connectivity before making any connections
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                UserDialogs.Instance.Loading().Dispose();
                UserDialogs.Instance.Alert("You are not connected to internet", "Alert", "Ok");
                return;
            }

            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Surname) &&
                !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
            {

                // Checking for individual field validation before proceeding
                if (AllFieldsValidated())
                {
                    UserRegister create_user_details = new UserRegister()
                    {
                        Email = Email,
                        NameAndSurname = Surname + " " + Name
                    };


                    CreateUser(create_user_details);
                }
                else
                {
                    UserDialogs.Instance.Loading().Dispose();
                    UserDialogs.Instance.Toast("Please make sure all fields are valid");

                }
            }
            else
            {
                UserDialogs.Instance.Loading().Dispose();
                UserDialogs.Instance.Toast("Enter all details");
            }
        }

        private bool AllFieldsValidated()
        {
            bool FoundValidField = true;
            // Email validation 
            try
            {
                MailAddress m = new MailAddress(Email);
            }
            catch (FormatException)
            {
                UserDialogs.Instance.Toast("Enter valid email address");
                FoundValidField = false;
            }

            if (Password.Length < 6)
            {
                // Password must have 6 or more characters
                FoundValidField = false;
                UserDialogs.Instance.Toast("Entered password must have 6 or more symbols");
            }

            return FoundValidField;
        }

        private async void CreateUser(UserRegister user_details)
        {
            // firebaseAuth sign up
            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Constants.WebAPIkey));
                var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(user_details.Email, Password);
                string userId = auth.User.LocalId;

                DatabaseServices databaseServices = new DatabaseServices();
                var UserAdded = await databaseServices.AddUser(user_details, userId);

                if (!UserAdded)
                {
                    // delete user account and prompt a user to register again
                    // return;
                }

                await _navigationService.NavigateAsync("/LoginPage");
                UserDialogs.Instance.Loading().Dispose();
                UserDialogs.Instance.Toast("Sign up successful");

                //await App.Current.MainPage.DisplayAlert("Alert", gettoken, "Ok");

            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Loading().Dispose();
                await UserDialogs.Instance.AlertAsync("Failed to signup, something went wrong", "Error", "OK");
            }


        }

        private void OpenLoginPage()
        {
            _navigationService.NavigateAsync("/LoginPage");
        }
    }
}
