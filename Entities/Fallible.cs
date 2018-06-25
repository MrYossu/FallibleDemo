using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using log4net;

namespace Entities {
  [DataContract]
  public class Fallible<T> {
    // Would be injected
    private ILog _logger = LogManager.GetLogger(typeof(Fallible<T>));

    // This method does the fallible work, and so would normally only be called in the service layer
    protected Fallible() {
    }

    // The [CallerMemberName] attribute (.NET4.5/C#5) adds the name of the method that called this, which we use for logging
    public static Fallible<T> Do(Func<T> f, [CallerMemberName] string callingMethod = null) {
      Fallible<T> fall = new Fallible<T>();
      return fall.DoPrivate(f, callingMethod);
    }

    private Fallible<T> DoPrivate(Func<T> f, string caller) {
      // This is an instance method, so can access the injected logger
      Fallible<T> result;
      try {
        T fResult = f();
        result = new Success<T> { Value = fResult };
      }
      catch (BadIdeaException ex) {
        _logger.Debug("Bad idea: " + ex.Message + " at " + caller + " - " + ex.StackTrace.Substring(0, 30) + "...");
        result = new BadIdea<T> { Message = ex.Message, StackTrace = ex.StackTrace };
      }
      catch (Exception ex) {
        _logger.Error("Exception: " + ex.Message + " at " + caller + " - " + ex.StackTrace.Substring(0, 30) + "...");
        result = new Failure<T> { Message = ex.Message, StackTrace = ex.StackTrace };
      }
      return result;
    }

    // This method handles the fallible result, so would normally only be called in the client
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
  public class Fallible {
    private ILog _logger = LogManager.GetLogger(typeof(Fallible));

    protected Fallible() {
    }

    public static Fallible Do(Action f, [CallerMemberName] string callingMethod = null) {
      Fallible fall = new Fallible();
      return fall.DoPrivate(f, callingMethod);
    }

    private Fallible DoPrivate(Action f, string caller) {
      Fallible result;
      try {
        f();
        result = new Success();
      }
      catch (BadIdeaException ex) {
        _logger.Debug("Bad idea: " + ex.Message + " at " + caller + " - " + ex.StackTrace.Substring(0, 30) + "...");
        result = new BadIdea { Message = ex.Message, StackTrace = ex.StackTrace };
      }
      catch (Exception ex) {
        _logger.Error("Exception: " + ex.Message + " at " + caller + " - " + ex.StackTrace.Substring(0, 30) + "...");
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