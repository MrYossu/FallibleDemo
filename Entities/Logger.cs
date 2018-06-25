using System.Diagnostics;

namespace Entities {
  public class Logger : LoggerInterface {
    public void Log(string msg) {
      Debug.WriteLine("LOG: " + msg);
    }
  }
}