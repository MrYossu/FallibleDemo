using System;
using Client.CustomerServiceReference;
using Entities;

namespace Client {
  class Program {
    static void Main(string[] args) {
      using (CustomerServiceClient client = new CustomerServiceClient()) {
        // Method #1 - get the Fallible object, then deal with it
        Fallible<Customer> cf = client.GetCustomerFallible(1);
        cf.Match(c => Console.WriteLine("Success: (" + c.ID + ") " + c.Name),
          (msg, _) => Console.WriteLine("Exception: " + msg),
          (msg, _) => Console.WriteLine("Bad idea: " + msg));

        // Method #2 - Don't bother assigning the Fallible object to a local variable as we can't do anything
        // with it other than call match, so might as well call it directly. This is neater
        client.GetCustomerFallible(2)
          .Match(c => Console.WriteLine("Success: (" + c.ID + ") " + c.Name),
            (msg, _) => Console.WriteLine("Exception: " + msg),
            (msg, _) => Console.WriteLine("Bad idea: " + msg));

        // Method #3 - Use separate methods. This allows us to use method groups which is even neater
        client.GetCustomerFallible(3)
          .Match(OnSuccess,
            OnFailure,
            // Note that we can't use a method group here as we aren't passing the second parameter along
            (msg, _) => OnBadIdea(msg));

        Console.ReadKey();
      }
    }

    private static void OnSuccess(Customer c) {
      Console.WriteLine("Success: (" + c.ID + ") " + c.Name);
    }

    private static void OnFailure(string msg, string st) {
      Console.WriteLine("Exception: " + msg + ". Stack trace starts... " + st.Substring(0, 20));
    }

    private static void OnBadIdea(string msg) {
      Console.WriteLine("Bad idea: " + msg);
    }
  }
}