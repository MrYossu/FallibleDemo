using System;
using Client.CustomerServiceReference;
using Entities;

namespace Client {
  class Program {
    static void Main(string[] args) {
      using (CustomerServiceClient client = new CustomerServiceClient()) {
        // Private method to save me repeating the same code for each service call. In a real app you probably wouldn't be doing the same thing so many times, so wouldn't need to do this
        // Note that private methods are a C#7 feature, so if you're using C#6, you'll need to move this out of Main()
        void Match(int id) {
          Fallible<Customer> cf = client.GetCustomerFallible(id);
          cf.Match(c => Console.WriteLine("Success: (" + c.ID + ") " + c.Name),
            (msg, _) => Console.WriteLine("Exception: " + msg),
            (msg, _) => Console.WriteLine("Bad idea: " + msg));
        }

        Match(1);
        Match(2);
        Match(3);
        Console.ReadKey();
      }
    }
  }
}