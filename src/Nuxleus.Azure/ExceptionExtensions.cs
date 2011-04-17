﻿using System.Text;
using System;

namespace Azure.Toolkit
{
    public static class ExceptionExtensions
    {
        const string Line = "==============================================================================";

        static string BuildMessage(Exception exception)
        {
            return string.Format("{0}{1}{2}:{3}{4}{5}{6}{7}", Line, Environment.NewLine, exception.GetType().Name,
                                 exception.Message, Environment.NewLine, exception.StackTrace, Environment.NewLine, Line);
        }

        public static string TraceInformation(this Exception exception)
        {
            if (exception == null)
                return string.Empty;

            var exceptionInformation = new StringBuilder();

            exceptionInformation.Append(BuildMessage(exception));

            Exception inner = exception.InnerException;

            while (inner != null)
            {
                exceptionInformation.Append(Environment.NewLine);
                exceptionInformation.Append(Environment.NewLine);
                exceptionInformation.Append(BuildMessage(inner));
                inner = inner.InnerException;
            }

            return exceptionInformation.ToString();
        }
    }
}
