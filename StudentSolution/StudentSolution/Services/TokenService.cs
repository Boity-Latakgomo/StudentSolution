using Newtonsoft.Json;
using StudentSolution.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace StudentSolution.Services
{
    public static class TokenService
    {

        public static void RemoveTokenPreference()
        {
            Preferences.Remove(Constants.TokenKey);
        }

        public static void SetTokenPreference(string serializedTokenContent)
        {
            Preferences.Set(Constants.TokenKey, serializedTokenContent);
        }

        // This returns the exact token
        public static string GetIdToken()
        {
            Token tokenObject = GetTokenObject();
            return tokenObject.idToken;
        }

        public static string GetUserEmail()
        {
            Token tokenObject = GetTokenObject();
            return tokenObject.User.email;
        }

        public static string GetUserID()
        {
            Token tokenObject = GetTokenObject();
            return tokenObject.User.localId;
        }

        // This method returns a deserialized token object and is consumed in this class only.
        private static Token GetTokenObject()
        {
            string serializedTokenContent = Preferences.Get(Constants.TokenKey, string.Empty);
            return JsonConvert.DeserializeObject<Token>(serializedTokenContent);
        }

    }
}
