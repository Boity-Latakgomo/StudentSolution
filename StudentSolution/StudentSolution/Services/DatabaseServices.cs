using Firebase.Database;
using Firebase.Database.Query;
using StudentSolution.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentSolution.Services
{
    public class DatabaseServices
    {
        #region firebase constants
        private static string auth = "ySbSRzvoXD2Z26pGfePP2PsUe3BWEYDFdbjfoOCm"; //  app secret

        //FirebaseClient firebase = new FirebaseClient("https://practice-ce7fe-default-rtdb.firebaseio.com/");
        FirebaseClient firebase = new FirebaseClient("https://employee-and-student-solution-default-rtdb.firebaseio.com/",
            new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(auth) });
        #endregion

        public async Task<bool> AddUser(UserRegister userDetails, string userId)
        {
            FirebaseObject<UserRegister> response = await firebase
            .Child("StudentSolution")
            .Child("Students")
            .PostAsync<UserRegister>(new UserRegister()
            {
                Email = userDetails.Email,
                NameAndSurname = userDetails.NameAndSurname,
                EmailUserID = userId
            });

            if (!string.IsNullOrEmpty(response.Key) && response.Object != null)
            {
                //await AddUserEmail(userDetails.EmailAddress);
                return true;
            }
            return false;
        }

        public async Task<UserRegister> GetUser(string UserID)
        {
            var allUsers = (await firebase
             .Child("StudentSolution")
            .Child("Students")
              .OnceAsync<UserRegister>()).Select(item => new UserRegister
              {
                  Email = item.Object.Email,
                  NameAndSurname = item.Object.NameAndSurname,
                  EmailUserID = item.Object.EmailUserID
              }).ToList();

            return allUsers.Where(a => a.EmailUserID == UserID).FirstOrDefault();
        }

        public async Task<List<Opportunity>> GetAllOpportunities()
        {
            return (await firebase
             .Child("EmployeeSolution")
            .Child("LeaveApplication")
              .OnceAsync<Opportunity>()).Select(item => new Opportunity
              {
                  Title = item.Object.Title,
                  Details = item.Object.Details
                  

              }).ToList();
        }
    }
}
