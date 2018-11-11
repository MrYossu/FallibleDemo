using System.ServiceModel;
using System.ServiceModel.Activation;
using Entities;

namespace Wcf {
  [ServiceContract(Namespace = "")]
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class CustomerService {
    // Pseudo-inject
    private readonly CustomerServiceLogic _customerServiceLogic = new CustomerServiceLogic();

    [OperationContract]
    public Customer GetCustomerRegular(int id) {
      return _customerServiceLogic.GetCustomer(id);
    }

    [OperationContract]
    [ServiceKnownType("GetCustomerFallibleTypes", typeof(KnownTypesHelper))]
    public Fallible<Customer> GetCustomerFallible(int id) {
      return Fallible<Customer>.Do(() => _customerServiceLogic.GetCustomer(id));
    }

    [OperationContract]
    [ServiceKnownType("GetCustomerFallibleTypes", typeof(KnownTypesHelper))]
    public Fallible UpdateCustomer(Customer c) {
      return Fallible.Do(()=>_customerServiceLogic.UpdateCustomer(c));
    }

  }
}