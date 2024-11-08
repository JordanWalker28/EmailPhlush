using System;

namespace EmailPhlush
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("The application requires exactly 3 arguments: email, password, and method.");
                return;
            }

            var email = args[0];
            var password = args[1];
            var method = args[2];

            var emailJobProcessor = new EmailPhlush(email, password, method);
            emailJobProcessor.Execute();
        }
    }
}