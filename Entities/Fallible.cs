using System;
using System.Runtime.Serialization;

namespace Entities {
  [DataContract]
  public abstract class Fallible<T> {
    public static Fallible<T> Do(Func<T> f) {
      Fallible<T> result;
      try {
        T fResult = f();
        result = new Success<T> { Value = fResult };
      }
      catch (BadIdeaException ex) {
        result = new BadIdea<T> { Message = ex.Message, StackTrace = ex.StackTrace };
      }
      catch (Exception ex) {
        // NOTE that in a real application, we would log the exception at this point. Only issue is that this method is static, so no injection
        result = new Failure<T> { Message = ex.Message, StackTrace = ex.StackTrace };
      }
      return result;
    }

    // C#7 version
    public void Match(Action<T> onSuccess, Action<string, string> onFailure, Action<string, string> onBadIdea = null) {
      switch (this) {
        case Success<T> success:
          onSuccess(success.Value);
          break;
        case BadIdea<T> badIdea:
          if (onBadIdea != null) {
            onBadIdea(badIdea.Message, badIdea.StackTrace);
          } else {
            onFailure(badIdea.Message, badIdea.StackTrace);
          }
          break;
        case Failure<T> failure:
          onFailure(failure.Message, failure.StackTrace);
          break;
      }
    }

    // C#6 version
    //public void Match(Action<T> onSuccess, Action<string, string> onFailure, Action<string, string> onBadIdea = null) {
    //  if (this is Success<T>) {
    //    Success<T> success = (Success<T>)this;
    //    onSuccess(success.Value);
    //  } else if (this is BadIdea<T>) {
    //    BadIdea<T> badIdea = (BadIdea<T>)this;
    //    if (onBadIdea != null) {
    //      onBadIdea(badIdea.Message, badIdea.StackTrace);
    //    } else {
    //      onFailure(badIdea.Message, badIdea.StackTrace);
    //    }
    //  } else if (this is Failure<T>) {
    //    Failure<T> failure = (Failure<T>)this;
    //    onFailure(failure.Message, failure.StackTrace);
    //  }
    //}
  }

  [DataContract]
  public abstract class Fallible {
    public static Fallible Do(Action f) {
      Fallible result;
      try {
        f();
        result = new Success();
      }
      catch (BadIdeaException ex) {
        result = new BadIdea { Message = ex.Message, StackTrace = ex.StackTrace };
      }
      catch (Exception ex) {
        // NOTE that in a real application, we would log the exception at this point
        result = new Failure { Message = ex.Message, StackTrace = ex.StackTrace };
      }
      return result;
    }

    public void Match(Action onSuccess, Action<string, string> onFailure, Action<string, string> onBadIdea = null) {
      switch (this) {
        case Success _:
          onSuccess();
          break;
        case BadIdea badIdea:
          if (onBadIdea != null) {
            onBadIdea(badIdea.Message, badIdea.StackTrace);
          } else {
            onFailure(badIdea.Message, badIdea.StackTrace);
          }
          break;
        case Failure failure:
          onFailure(failure.Message, failure.StackTrace);
          break;
      }
    }
  }
}