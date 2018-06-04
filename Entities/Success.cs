using System.Runtime.Serialization;

namespace Entities {
  [DataContract]
  public class Success<T> : Fallible<T> {
    [DataMember]
    public T Value { internal get; set; }
  }

  [DataContract]
  public class Success : Fallible {
  }
}