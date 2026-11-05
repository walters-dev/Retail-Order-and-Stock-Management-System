using BCrypt.Net;
using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
namespace RetailOrderStockManagementSystem.Backend.Security
{


	public class Encryption
	{

		public string HashPassword(string password)// hashes inputted password with salt
		{
			
			return BCrypt.Net.BCrypt.HashPassword(password, 12);// bcrypt stores salt with hashed password 

		}
		
		public bool VerifyPassword(string enteredPassword, string existingPassword)//checks if password is the same as the password in database
		{
            //hashes enteredpassword with the salt in hashpassword then compares 2
            var isValid = BCrypt.Net.BCrypt.Verify(enteredPassword, existingPassword);
			if (isValid)
			{
				return true;
			}
			else
			{
				return false;
			}

		}
		
	}
}
