using System.Runtime.Serialization;

namespace Entities {
  [DataContract]
  public class Failure<T> : Fallible<T> {
    [DataMember]
    public string Message { internal get; set; }
    [DataMember]
    public string StackTrace { internal get; set; }
  }

  [DataContract]
  public class Failure : Fallible {
    [DataMember]
    public string Message { internal get; set; }
    [DataMember]
    public string StackTrace { internal get; set; }
  }
}