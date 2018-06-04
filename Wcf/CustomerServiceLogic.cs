﻿using System;
using Entities;

namespace Wcf {
  public class CustomerServiceLogic {
    public Customer GetCustomer(int id) {
      if (id == 1) {
        throw new Exception("Customer has died");
      }
      if (id == 2) {
        throw new BadIdeaException("Customer doesn't want to talk to you");
      }
      return new Customer {
        ID = id,
        Name = "Jim " + id
      };
    }
  }
}