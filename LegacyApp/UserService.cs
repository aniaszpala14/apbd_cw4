using System;
using System.Reflection.PortableExecutable;

namespace LegacyApp
{
    public class UserService 
    {
        public bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || !CheckEmail(email) || (CountAge(dateOfBirth) < 21))
            { return false; }
            
            var clientRepository = new ClientRepository();
            var client = clientRepository.GetById(clientId);


            var user = NewUser(client, firstName, lastName, email, dateOfBirth);
            UserCreditLimit(user,client);

            if (user.HasCreditLimit && user.CreditLimit < 500)
            { return false; }

            UserDataAccess.AddUser(user);
            return true;
        }

        public int CountAge(DateTime dateOfBirth)
        {
            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;
            return age;
        }

        public bool CheckEmail(String email)
        {
        return (!email.Contains("@") && !email.Contains(".")) ? false : true;
        }
        public User NewUser(Client client, string firstName, string lastName,string email,DateTime dateOfBirth )
        {
            return new User {
                Client = client,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                FirstName = firstName,
                LastName = lastName };
        }

        public void UserCreditLimit(User user,Client client)
        {
            if (client.Type == "VeryImportantClient")
            {
                user.HasCreditLimit = false;
            }
            else 
            {
                using (var userCreditService = new UserCreditService())
                {
                    int creditLimit = userCreditService.GetCreditLimit(user.LastName, user.DateOfBirth);
                    if(client.Type=="ImportantClient") 
                    { creditLimit = creditLimit * 2;}
                    user.CreditLimit = creditLimit;
                }
            }
        }
    }
}
