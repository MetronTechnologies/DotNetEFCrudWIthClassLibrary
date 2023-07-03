using System;
namespace EfCrudAuthorizationAuthentication.Services;


public class AuthService {

	public static string formUsername(string email) {
		string firstPart = email.Split('@')[0];
		char firstLetter = email[0];
		string username = firstLetter.ToString().ToUpper() + firstPart.Substring(1);
		return username;
	}

}

