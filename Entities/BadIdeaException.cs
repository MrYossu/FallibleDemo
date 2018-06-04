using System;

namespace Entities {
  public class BadIdeaException : Exception {
    public BadIdeaException(string message) : base(message) {
    }
  }
}