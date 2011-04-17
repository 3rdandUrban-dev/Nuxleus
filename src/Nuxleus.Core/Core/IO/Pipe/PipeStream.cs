// Lifted directly from http://code.google.com/p/fastajaxproxy/source/browse/trunk/App_Code/PipeStream.cs?r=2
// and thus far has made no changes to its original state except for changing the namespace it which it resides.
// If this proves to be a useful utility I plan to spend time optimizing the current state and extending the
// feature set to inherit the multipeer-2-multipeer message dissemintation and distribution capabilities provided
// by the core Nuxleus messaging sub-system.
//
// M. David Peterson 2010-12-17 at 9:42 P.M. Mountain Standard Time

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Nuxleus.Core.IO
{
	/// <summary>
	/// PipeStream is a thread-safe read/write data stream for use between two threads in a 
	/// single-producer/single-consumer type problem.
	/// </summary>
	/// <version>2006/10/13 1.0</version>
	/// <license>
	///	Copyright (c) 2006 James Kolpack (james dot kolpack at google mail)
	///	
	///	Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
	///	associated documentation files (the "Software"), to deal in the Software without restriction, 
	///	including without limitation the rights to use, copy, modify, merge, publish, distribute, 
	///	sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
	///	furnished to do so, subject to the following conditions:
	///	
	///	The above copyright notice and this permission notice shall be included in all copies or 
	///	substantial portions of the Software.
	///	
	///	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
	///	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
	///	PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
	///	LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT 
	///	OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
	///	OTHER DEALINGS IN THE SOFTWARE.
	/// </license>
	public class PipeStream : Stream
	{
		#region Private members
		/// <summary>
		/// Queue of bytes provides the datastructure for transmitting from an
		/// input stream to an output stream.
		/// </summary>
		/// <remarks>Possible more effecient ways to accomplish this.</remarks>
		private Queue<byte> mBuffer = new Queue<byte>();

		/// <summary>
		/// Event occurs after data is written.
		/// </summary>
		private ManualResetEvent mWriteEvent;
		
		/// <summary>
		/// Event occurs after data is read.
		/// </summary>
		private ManualResetEvent mReadEvent;
		
		/// <summary>
		/// Indicates that the input stream has been flushed and that
		/// all remaining data should be written to the output stream.
		/// </summary>
		private bool mFlushed = false;
		
		/// <summary>
		/// Maximum number of bytes to store in the buffer.
		/// </summary>
		private long mMaxBufferLength = 1 * MB;
		
		/// <summary>
		/// Setting this to true will cause Read() to block if it appears
		/// that it will run out of data.
		/// </summary>
		private bool mBlockLastRead = false;

        private int _ReadWriteTimeout;

        private long _TotalWrite = 0;

		#endregion

		#region Public Const members

		/// <summary>
		/// Number of bytes in a kilobyte
		/// </summary>
		public const long KB = 1024;
		
		/// <summary>
		/// Number of bytes in a megabyte
		/// </summary>
		public const long MB = KB * 1024;

		#endregion

		#region Public properties

		/// <summary>
		/// Gets or sets the maximum number of bytes to store in the buffer.
		/// </summary>
		/// <value>The length of the max buffer.</value>
		public long MaxBufferLength
		{
			get { return mMaxBufferLength; }
			set { mMaxBufferLength = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to block last read method before the buffer is empty.
		/// When true, Read() will block until it can fill the passed in buffer and count.
		/// When false, Read() will not block, returning all the available buffer data.
		/// </summary>
		/// <remarks>
		/// Setting to true will remove the possibility of ending a stream reader prematurely.
		/// </remarks>
		/// <value>
		/// 	<c>true</c> if block last read method before the buffer is empty; otherwise, <c>false</c>.
		/// </value>
		public bool BlockLastReadBuffer
		{
			get
			{
				return mBlockLastRead;
			}
			set
			{
				mBlockLastRead = value;

				// when turning off the block last read, signal Read() that it may now read the rest of the buffer.
				if (!mBlockLastRead)
					mWriteEvent.Set(); 
			}
		}

		#endregion

		#region Constructor


		/// <summary>
		/// Initializes a new instance of the <see cref="PipeStream"/> class.
		/// </summary>
		public PipeStream(int readWriteTimeout)
		{
            this._ReadWriteTimeout = readWriteTimeout;
			mWriteEvent = new ManualResetEvent(false);
			mReadEvent = new ManualResetEvent(false);
		}

		#endregion

		#region Stream overide methods

		///<summary>
		///When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
		///</summary>
		///
		///<exception cref="T:System.IO.IOException">An I/O error occurs. </exception><filterpriority>2</filterpriority>
		public override void Flush()
		{
			mFlushed = true;
			mWriteEvent.Set(); // signal any waiting read events.
		}

		///<summary>
		///When overridden in a derived class, sets the position within the current stream.
		///</summary>
		///
		///<returns>
		///The new position within the current stream.
		///</returns>
		///
		///<param name="offset">A byte offset relative to the origin parameter. </param>
		///<param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"></see> indicating the reference point used to obtain the new position. </param>
		///<exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
		///<exception cref="T:System.NotSupportedException">The stream does not support seeking, such as if the stream is constructed from a pipe or console output. </exception>
		///<exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception><filterpriority>1</filterpriority>
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		///<summary>
		///When overridden in a derived class, sets the length of the current stream.
		///</summary>
		///
		///<param name="value">The desired length of the current stream in bytes. </param>
		///<exception cref="T:System.NotSupportedException">The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output. </exception>
		///<exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
		///<exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception><filterpriority>2</filterpriority>
		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}


		///<summary>
		///When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
		///</summary>
		///
		///<returns>
		///The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
		///</returns>
		///
		///<param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream. </param>
		///<param name="count">The maximum number of bytes to be read from the current stream. </param>
		///<param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source. </param>
		///<exception cref="T:System.ArgumentException">The sum of offset and count is larger than the buffer length. </exception>
		///<exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
		///<exception cref="T:System.NotSupportedException">The stream does not support reading. </exception>
		///<exception cref="T:System.ArgumentNullException">buffer is null. </exception>
		///<exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
		///<exception cref="T:System.ArgumentOutOfRangeException">offset or count is negative. </exception><filterpriority>1</filterpriority>
		public override int Read(byte[] buffer, int offset, int count)
		{
            //using (new ProxyHelpers.TimedLog("PipeStrem\tRead"))
            //{
                if (offset != 0)
                    throw new NotImplementedException("Offsets with value of non-zero are not supported");
                if (buffer == null)
                    throw new ArgumentException("Buffer is null");
                if (offset + count > buffer.Length)
                    throw new ArgumentException("The sum of offset and count is greater than the buffer length. ");
                if (offset < 0 || count < 0)
                    throw new ArgumentOutOfRangeException("offset or count is negative.");
                if (BlockLastReadBuffer && count >= mMaxBufferLength)
                    throw new ArgumentException("count > mMaxBufferLength");

                if (count == 0)
                    return 0;

                int readLength;

                while ((Length < count && !mFlushed) ||
                       (Length < (count + 1) && BlockLastReadBuffer))
                {
                    mWriteEvent.Reset(); // turn off an existing write signal
                    mReadEvent.Set(); // signal any waiting reads, preventing deadlock
                    mWriteEvent.WaitOne(this._ReadWriteTimeout, false); // wait until a write occurs
                }
                lock (mBuffer)
                {
                    // fill the read buffer
                    readLength = ReadToBuffer(buffer, offset, count);
                    mReadEvent.Set();
                }
                return readLength;
            //}
		}

        protected virtual int ReadToBuffer(byte[] buffer, int offset, int count)
        {
            int readLength = 0;
			
            for (; readLength < count && Length > 0; readLength++)
            {
                buffer[readLength] = mBuffer.Dequeue();
            }

            return readLength;
        }

		///<summary>
		///When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
		///</summary>
		///
		///<param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream. </param>
		///<param name="count">The number of bytes to be written to the current stream. </param>
		///<param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream. </param>
		///<exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
		///<exception cref="T:System.NotSupportedException">The stream does not support writing. </exception>
		///<exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
		///<exception cref="T:System.ArgumentNullException">buffer is null. </exception>
		///<exception cref="T:System.ArgumentException">The sum of offset and count is greater than the buffer length. </exception>
		///<exception cref="T:System.ArgumentOutOfRangeException">offset or count is negative. </exception><filterpriority>1</filterpriority>
		public override void Write(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
				throw new ArgumentException("Buffer is null");
			if (offset + count > buffer.Length)
				throw new ArgumentException("The sum of offset and count is greater than the buffer length. ");
			if (offset < 0 || count < 0)
				throw new ArgumentOutOfRangeException("offset or count is negative.");
			if (count == 0)
				return;
			while (Length >= mMaxBufferLength)
			{
				mReadEvent.Reset();
				mWriteEvent.Set(); // release any blocked read events
				mReadEvent.WaitOne(this._ReadWriteTimeout, false);
			}
			lock (mBuffer)
			{
				mFlushed = false; // if it were flushed before, it soon will not be.

                WriteToBuffer(buffer, offset, count);

                this._TotalWrite += count;

				mWriteEvent.Set(); // signal that write has occured
			}
		}

        protected virtual void WriteToBuffer(byte[] buffer, int offset, int count)
        {
            // queue up the buffer data
            for (int i = offset; i < count; i++)
            {
                mBuffer.Enqueue(buffer[i]);
            }
        }

		///<summary>
		///When overridden in a derived class, gets a value indicating whether the current stream supports reading.
		///</summary>
		///
		///<returns>
		///true if the stream supports reading; otherwise, false.
		///</returns>
		///<filterpriority>1</filterpriority>
		public override bool CanRead
		{
			get
			{
				 return true;
			}
		}

		///<summary>
		///When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
		///</summary>
		///
		///<returns>
		///true if the stream supports seeking; otherwise, false.
		///</returns>
		///<filterpriority>1</filterpriority>
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		///<summary>
		///When overridden in a derived class, gets a value indicating whether the current stream supports writing.
		///</summary>
		///
		///<returns>
		///true if the stream supports writing; otherwise, false.
		///</returns>
		///<filterpriority>1</filterpriority>
		public override bool CanWrite
		{
			get
			{
				 return true;
			}
		}

		///<summary>
		///When overridden in a derived class, gets the length in bytes of the stream.
		///</summary>
		///
		///<returns>
		///A long value representing the length of the stream in bytes.
		///</returns>
		///
		///<exception cref="T:System.NotSupportedException">A class derived from Stream does not support seeking. </exception>
		///<exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception><filterpriority>1</filterpriority>
		public override long Length
		{
			get
			{
				 return mBuffer.Count;
			}
		}

		///<summary>
		///When overridden in a derived class, gets or sets the position within the current stream.
		///</summary>
		///
		///<returns>
		///The current position within the stream.
		///</returns>
		///
		///<exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
		///<exception cref="T:System.NotSupportedException">The stream does not support seeking. </exception>
		///<exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception><filterpriority>1</filterpriority>
		public override long Position
		{
			get
			{
				 return 0;
			}
			set
			{
				 throw new NotImplementedException();
			}
		}
		#endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            mBuffer.Clear();
            TotalWrite = 0;

            (mWriteEvent as IDisposable).Dispose();
            (mReadEvent as IDisposable).Dispose();
        }

        public long TotalWrite
        {
            get
            {
                return _TotalWrite;
            }
            set
            {
                _TotalWrite = value;
            }
        }
    }
}
