using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using log4net;

namespace Entities {
  [DataContract]
  public class Fallible<T> {
    // This method does the fallible work, and so would normally only be called in the service layer
    protected Fallible() {
    }

    // The [CallerMemberName] attribute (.NET4.5/C#5) adds the name of the method that called this, which we use for logging
    public static Fallible<T> Do(Func<T> f) {
      Fallible<T> fall = new Fallible<T>();
      return fall.DoPrivate(f);
    }

    private Fallible<T> DoPrivate(Func<T> f) {
      Fallible<T> result;
      // Create a logger based on the call stack, as that way we can set the type to be the class that made the call to Do()
      ILog logger = LogManager.GetLogger(GetCallingType());
      try {
        T fResult = f();
        result = new Success<T> { Value = fResult };
      }
      catch (BadIdeaException ex) {
        logger.Debug("Bad idea: " + ex.Message + Environment.NewLine + ex.StackTrace);
        result = new BadIdea<T> { Message = ex.Message, StackTrace = ex.StackTrace };
      }
      catch (Exception ex) {
        logger.Error("Exception: " + ex.Message + Environment.NewLine + ex.StackTrace);
        result = new Failure<T> { Message = ex.Message, StackTrace = ex.StackTrace };
      }
      return result;
    }

    private static string GetCallingType() {
      StackTrace stackTrace = new StackTrace();
      string callingType = "";
      for (int i = 0; i < stackTrace.FrameCount && stackTrace.GetFrame(i).GetMethod().DeclaringType != null; i++) {
        callingType = stackTrace.GetFrame(i).GetMethod().DeclaringType.Name;
      }
      return callingType;
    }

    // This method handles the fallible result, so would normally only be called in the client
    // C#7 version
    //public void Match(Action<T> onSuccess, Action<string, string> onFailure, Action<string, string> onBadIdea = null) {
    //  switch (this) {
    //    case Success<T> success:
    //      onSuccess(success.Value);
    //      break;
    //    case BadIdea<T> badIdea:
    //      if (onBadIdea != null) {
    //        onBadIdea(badIdea.Message, badIdea.StackTrace);
    //      } else {
    //        onFailure(badIdea.Message, badIdea.StackTrace);
    //      }
    //      break;
    //    case Failure<T> failure:
    //      // TODO AYS - Handle exception
    //      onFailure(failure.Message, failure.StackTrace);
    //      break;
    //  }
    //}

    // C#6 version
    public void Match(Action<T> onSuccess, Action<string> onFailure, Action<string> onBadIdea = null) {
      if (this is Success<T>) {
        Success<T> success = (Success<T>)this;
        onSuccess(success.Value);
      } else if (this is BadIdea<T>) {
        BadIdea<T> badIdea = (BadIdea<T>)this;
        if (onBadIdea != null) {
          onBadIdea(badIdea.Message);
        } else {
          onFailure(badIdea.Message);
        }
      } else if (this is Failure<T>) {
        Failure<T> failure = (Failure<T>)this;
        onFailure(failure.Message);
      }
    }
  }

  [DataContract]
  public class Fallible {
    protected Fallible() {
    }

    public static Fallible Do(Action f) {
      Fallible fall = new Fallible();
      return fall.DoPrivate(f);
    }

    private Fallible DoPrivate(Action f) {
      Fallible result;
      ILog logger = LogManager.GetLogger(GetCallingType());
      try {
        f();
        result = new Success();
      }
      catch (BadIdeaException ex) {
        logger.Debug("Bad idea: " + ex.Message + Environment.NewLine + ex.StackTrace);
        result = new BadIdea { Message = ex.Message, StackTrace = ex.StackTrace };
      }
      catch (Exception ex) {
        logger.Error("Exception: " + ex.Message + Environment.NewLine + ex.StackTrace);
        result = new Failure { Message = ex.Message, StackTrace = ex.StackTrace };
      }
      return result;
    }

    private static string GetCallingType() {
      StackTrace stackTrace = new StackTrace();
      string callingType = "";
      for (int i = 0; i < stackTrace.FrameCount && stackTrace.GetFrame(i).GetMethod().DeclaringType != null; i++) {
        callingType = stackTrace.GetFrame(i).GetMethod().DeclaringType.Name;
      }
      return callingType;
    }

    public void Match(Action onSuccess, Action<string> onFailure, Action<string> onBadIdea = null) {
      switch (this) {
        case Success _:
          onSuccess();
          break;
        case BadIdea badIdea:
          if (onBadIdea != null) {
            onBadIdea(badIdea.Message);
          } else {
            onFailure(badIdea.Message);
          }
          break;
        case Failure failure:
          onFailure(failure.Message);
          break;
      }
    }
  }
}