using System;
using System.Threading;
using Client.CustomerServiceReference;
using Entities;

namespace Client {
  class Program {
    static void Main(string[] args) {
      using (CustomerServiceClient client = new CustomerServiceClient()) {
        // Method #1 - get the Fallible object, then deal with it
        // Note that we use named arguments to make the intention clearer
        Fallible<Customer> cf = client.GetCustomerFallible(1);
        cf.Match(
          onSuccess: c => Log("Success: (" + c.ID + ") " + c.Name),
          onFailure: msg => Log("Exception: " + msg),
          onBadIdea: msg => Log("Bad idea: " + msg)
        );

        // Method #2 - Don't bother assigning the Fallible object to a local variable as we can't do anything
        // with it other than call match, so might as well call it directly. This is neater
        client.GetCustomerFallible(2)
          .Match(
            onSuccess: c => Log("Success: (" + c.ID + ") " + c.Name),
            onFailure: msg => Log("Exception: " + msg),
            onBadIdea: msg => Log("Bad idea: " + msg)
          );

        // Method #3 - Use separate methods. This allows us to use method groups which is even neater
        // Given the obvious names, we don't need named arguments here, in other cases we might
        client.GetCustomerFallible(3)
          .Match(
            OnGetCustomerSuccess,
            OnGetCustomerFailure,
            OnGetCustomerBadIdea
          );

        Console.ReadKey();
      }
    }

    private static void OnGetCustomerSuccess(Customer c) {
      Log("Success: (" + c.ID + ") " + c.Name);
      // Wait until the data is too old, then try to update. This should raise a BadIdeaException
      using (CustomerServiceClient client = new CustomerServiceClient()) {
        // Try this twice, once immediately, and again after an unacceptable pause
        UpdateCustomer(c, client);
        Thread.Sleep(100);
        UpdateCustomer(c, client);
      }
    }

    private static void UpdateCustomer(Customer c, CustomerServiceClient client) {
      client.UpdateCustomer(c)
        .Match(() => {
            Log("Successfully saved the customer");
          },
          msg => {
            Log("Saving the customer failed: " + msg);
          },
          msg => {
            Log("Saving the customer was a bad idea because " + msg);
          });
    }

    private static void OnGetCustomerFailure(string msg) {
      Log("Exception: " + msg);
    }

    private static void OnGetCustomerBadIdea(string msg) {
      Log("Bad idea: " + msg);
    }

    private static void Log(string msg) {
      Console.WriteLine(msg);
      Console.WriteLine("");
    }
  }
}