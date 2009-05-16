/* ====================================================================
 * Copyright (c) 2007 Andre Luis Azevedo (az.andrel@yahoo.com.br)
 * All rights reserved.
 *                       
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 *    In addition, the source code must keep original namespace names.
 *
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in
 *    the documentation and/or other materials provided with the
 *    distribution. In addition, the binary form must keep the original 
 *    namespace names and original file name.
 * 
 * 3. The name "ALAZ" or "ALAZ Library" must not be used to endorse or promote 
 *    products derived from this software without prior written permission.
 *
 * 4. Products derived from this software may not be called "ALAZ" or
 *    "ALAZ Library" nor may "ALAZ" or "ALAZ Library" appear in their 
 *    names without prior written permission of the author.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY
 * EXPRESSED OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 * PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR OR
 * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE. 
 */

using System;

namespace ALAZ.SystemEx
{

  /// <summary>
  /// Base class for disposable objects.
  /// </summary>
  public abstract class BaseDisposable : IDisposable
  {

    #region Fields

    private bool FDisposed = false;

    #endregion

    #region Methods

    #region Free

    /// <summary>
    /// This method is called when object is being disposed. Override this method to free resources.
    /// </summary>
    /// <param name="canAccessFinalizable">
    /// Indicates if the method can access Finalizable member objects.
    /// If canAccessFinalizable = false the method was called by GC and you can´t access finalizable member objects.
    /// If canAccessFinalizable = true the method was called by user and you can access all member objects.
    /// </param>
    protected virtual void Free(bool canAccessFinalizable)
    {
      FDisposed = true;
    }

    #endregion

    #region CheckDisposedWithException

    /// <summary>
    /// Checks if object is already disposed.
    /// </summary>
    protected void CheckDisposedWithException()
    {

      if (Disposed)
      {
        throw new ObjectDisposedException(this.ToString());
      }

    }

    #endregion

    #region Dispose

    /// <summary>
    /// Dispose object resources.
    /// </summary>
    public void Dispose()
    {

      lock (this)
      {

        if (!FDisposed)
        {
          try
          {
            Free(true);
          }
          finally
          {
            FDisposed = true;
          }
        }

      }

    }

    #endregion

    #endregion

    #region Properties

    /// <summary>
    /// Indicates is object is already disposed.
    /// </summary>
    protected bool Disposed
    {

      get
      {
        lock (this)
        {
          return FDisposed;
        }
      }

      set
      {
        lock (this)
        {
          FDisposed = value;
        }
      }

    }

    #endregion

  }


}
