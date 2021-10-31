using Acr.UserDialogs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using StudentSolution.Models;
using StudentSolution.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace StudentSolution.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        INavigationService _navigationService;
        public IList<Opportunity> Opportunities { get; set; }
        public DelegateCommand LogoutCommand { get; set; }
        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            GetOpportunities();
            LogoutCommand = new DelegateCommand(() => logout());
            _navigationService = navigationService;
        }

        private async Task GetOpportunities()
        {
            UserDialogs.Instance.Loading("Loading...");

            DatabaseServices databaseServices = new DatabaseServices();
            Opportunities = await databaseServices.GetAllOpportunities();

            UserDialogs.Instance.Loading().Dispose();
        }

        private async void logout()
        {
            TokenService.RemoveTokenPreference();
            Preferences.Clear();
            //Preferences.Remove(Constants.UserDetails);
            await _navigationService.NavigateAsync("/LoginPage");
        }
    }
}
