using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entities;

namespace Wcf {
  public class KnownTypesHelper {
    public static IEnumerable<Type> GetCustomerFallibleTypes(ICustomAttributeProvider provider) {
      return GetFallibleTypes<Customer>(provider);
    }

    private static IEnumerable<Type> GetFallibleTypes<T>(ICustomAttributeProvider provider) {
      return new List<Type> {
        typeof(Fallible<T>),
        typeof(Success<T>),
        typeof(BadIdea<T>),
        typeof(Failure<T>)
      }.Union(GetNonGenericFallibleTypes(provider));
    }

    public static IEnumerable<Type> GetNonGenericFallibleTypes(ICustomAttributeProvider provider) {
      return new List<Type> {
        typeof(Fallible),
        typeof(Success),
        typeof(BadIdea),
        typeof(Failure)
      };
    }
  }
}