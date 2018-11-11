using System;
using System.Threading;
using Client.CustomerServiceReference;
using Entities;

namespace Client {
  class Program {
    static void Main(string[] args) {
      using (CustomerServiceClient customerService = new CustomerServiceClient()) {
        // Method #1 - get the Fallible object, then deal with it
        // Note that we use named arguments to make the intention clearer
        Log("Requesting customer #1 (who has died)");
        Fallible<Customer> cf = customerService.GetCustomerFallible(1);
        cf.Match(
          onSuccess: c => Log("Success: (" + c.ID + ") " + c.Name),
          onFailure: msg => Log("Exception: " + msg),
          onBadIdea: msg => Log("Bad idea: " + msg)
        );
        Log("=============================");

        // Method #2 - Don't bother assigning the Fallible object to a local variable as we can't do anything
        // with it other than call match, so might as well call it directly. This is neater
        Log("Requesting customer #2 (who is rude)");
        customerService.GetCustomerFallible(2)
          .Match(
            onSuccess: c => Log("Success: (" + c.ID + ") " + c.Name),
            onFailure: msg => Log("Exception: " + msg),
            onBadIdea: msg => Log("Bad idea: " + msg)
          );
        Log("=============================");

        // Method #3 - Use separate methods. This allows us to use method groups which is even neater
        // Given the obvious names, we don't need named arguments here
        Log("Requesting customer #3 (who returns without problem)");
        customerService.GetCustomerFallible(3)
          .Match(
            OnGetCustomerSuccess,
            OnGetCustomerFailure,
            OnGetCustomerBadIdea
          );
        Log("=============================");

        Console.ReadKey();
      }
    }

    private static void OnGetCustomerSuccess(Customer c) {
      Log("Success: (" + c.ID + ") " + c.Name);
      // Wait until the data is too old, then try to update. This should raise a BadIdeaException
      using (CustomerServiceClient client = new CustomerServiceClient()) {
        // Try this twice, once immediately, and again after an unacceptable pause
        Log($"Updating customer #{c.ID} immediately (which should succeed)");
        UpdateCustomer(c, client);
        Thread.Sleep(100);
        Log($"Updating customer #{c.ID} after a pause of 0.1 seconds (which should fail)");
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