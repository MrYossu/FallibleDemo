using System;
using System.Threading;
using Client.CustomerServiceReference;
using Entities;
using log4net;

namespace Client {
  class Program {
    private static ILog _logger;

    static void Main(string[] args) {
      SetUpLog4net();
      using (CustomerServiceClient client = new CustomerServiceClient()) {
        _logger.Debug("Starting Main");
        // Method #1 - get the Fallible object, then deal with it
        Fallible<Customer> cf = client.GetCustomerFallible(1);
        cf.Match(c => Console.WriteLine("Success: (" + c.ID + ") " + c.Name),
          (msg, _) => Console.WriteLine("Exception: " + msg),
          (msg, _) => Console.WriteLine("Bad idea: " + msg));

        // Method #2 - Don't bother assigning the Fallible object to a local variable as we can't do anything
        // with it other than call match, so might as well call it directly. This is neater
        _logger.Debug("Calling fallible directly");
        client.GetCustomerFallible(2)
          .Match(c => Console.WriteLine("Success: (" + c.ID + ") " + c.Name),
            (msg, _) => Console.WriteLine("Exception: " + msg),
            (msg, _) => Console.WriteLine("Bad idea: " + msg));

        // Method #3 - Use separate methods. This allows us to use method groups which is even neater
        _logger.Debug("Using separate methods");
        client.GetCustomerFallible(3)
          .Match(OnSuccess,
            OnFailure,
            // Note that we can't use a method group here as we aren't passing the second parameter along
            (msg, _) => OnBadIdea(msg));

        // In production code, the above would probably be written as follows, as it is neater...
        //client.GetCustomerFallible(3)
        //  .Match(OnSuccess, OnFailure, (msg, _) => OnBadIdea(msg));
        // ...or if we aren't anticipating a bad idea...
        //client.GetCustomerFallible(3).Match(OnSuccess, OnFailure);

        Console.ReadKey();
      }
    }

    private static void SetUpLog4net() {
      log4net.Config.XmlConfigurator.Configure();
      _logger = LogManager.GetLogger(typeof(Program));
    }

    private static void OnSuccess(Customer c) {
      _logger.Debug("Success: (" + c.ID + ") " + c.Name);
      Console.WriteLine("Success: (" + c.ID + ") " + c.Name);
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
            Console.WriteLine("Successfully saved the customer");
            _logger.Debug("Successfully saved the customer");
          },
          (msg, _) => {
            Console.WriteLine("Saving the customer failed: " + msg);
            _logger.Debug("Saving the customer failed: " + msg);
          },
          (msg, _) => {
            Console.WriteLine("Saving the customer was a bad idea because " + msg);
            _logger.Debug("Saving the customer was a bad idea because " + msg);
          });
    }

    private static void OnFailure(string msg, string st) {
      _logger.Debug("Exception: " + msg + ". Stack trace starts... " + st.Substring(0, 20));
      Console.WriteLine("Exception: " + msg + ". Stack trace starts... " + st.Substring(0, 20));
    }

    private static void OnBadIdea(string msg) {
      _logger.Debug("Bad idea: " + msg);
      Console.WriteLine("Bad idea: " + msg);
    }
  }
}