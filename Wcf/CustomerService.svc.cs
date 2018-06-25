using System.ServiceModel;
using System.ServiceModel.Activation;
using Entities;
using log4net;

namespace Wcf {
  [ServiceContract(Namespace = "")]
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class CustomerService {
    // Pseudo-inject
    private readonly CustomerServiceLogic _customerServiceLogic = new CustomerServiceLogic();
    private static ILog _logger;

    public CustomerService() {
      SetUpLog4net();
    }

    [OperationContract]
    public Customer GetCustomerRegular(int id) {
      _logger.Debug("GetCustomerRegular(" + id + ")");
      return _customerServiceLogic.GetCustomer(id);
    }

    [OperationContract]
    [ServiceKnownType("GetCustomerFallibleTypes", typeof(KnownTypesHelper))]
    public Fallible<Customer> GetCustomerFallible(int id) {
      _logger.Debug("GetCustomerFallible(" + id + ")");
      return Fallible<Customer>.Do(() => _customerServiceLogic.GetCustomer(id));
    }

    [OperationContract]
    [ServiceKnownType("GetCustomerFallibleTypes", typeof(KnownTypesHelper))]
    public Fallible UpdateCustomer(Customer c) {
      _logger.Debug("UpdateCustomer(" + c.ID + ")");
      return Fallible.Do(()=>_customerServiceLogic.UpdateCustomer(c));
    }

    private static void SetUpLog4net() {
      log4net.Config.XmlConfigurator.Configure();
      _logger = LogManager.GetLogger(typeof(CustomerService));
    }
  }
}