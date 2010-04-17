using System;
using System.Threading;
using System.Web;
using System.IO;

namespace Nuxleus.Transform {

  public class TransformServiceAsyncResult : IAsyncResult {

    Boolean _isCompleted;
    AsyncCallback cb = null;
    Object _asyncState;
    public HttpContext _context = null;

    public TransformServiceAsyncResult(AsyncCallback cb, Object extraData) {
      this.cb = cb;
      _asyncState = extraData;
      _isCompleted = false;
    }

    
    public object AsyncState {
      get {
        return _asyncState;
      }
    }

    public bool CompletedSynchronously {
      get {
        return false;
      }
    }

    public WaitHandle AsyncWaitHandle {
      get {
        throw new InvalidOperationException(
                  "ASP.Net should never use this property");
      }
    }

    public bool IsCompleted {
      get {
        return _isCompleted;
      }
    }

    public void CompleteCall() {
      _isCompleted = true;
      if (cb != null) {
        cb(this);
      }
    }

  }
}
