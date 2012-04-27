// 
// AbstractManosLogger.cs
//  
// Author:
//       Anirudh Sanjeev <anirudh@anirudhsanjeev.org>
// 
// Copyright (c) 2010 Anirudh Sanjeev
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.


using System;

namespace Manos.Logging
{
	public class ManosConsoleLogger : IManosLogger {

		private string name;
		private LogLevel level;

		public LogLevel Level {
			get { return level; }
		}

		public ManosConsoleLogger (string name, LogLevel level)
		{
			this.name = name;
			this.level = level;
		}
		
		public void Critical (string message, params object[] args)
		{
			if (level >= LogLevel.Critical)
				WriteString ("CRITICAL", message, args);
		}
		
		public void Error (string message, params object[] args)
		{
			if (level >= LogLevel.Error)
				WriteString ("ERROR", message, args);
		}

		public void Info (string message, params object[] args)
		{
			if (level >= LogLevel.Info)
				WriteString ("INFO", message, args);
		}
		
		
		public void Debug (string message, params object[] args)
		{
			if (level >= LogLevel.Debug)
				WriteString ("DEBUG", message, args);
		}
		
		private void WriteString (string level, string message, params object[] args)
		{
			string fmt = String.Format (message, args);
			Console.WriteLine (String.Format("{0} [{1}] {2} - {3}", DateTime.Now.ToString("HH':'mm':'ss'.'fffffff"), level, name, fmt));
		}
	}
}

