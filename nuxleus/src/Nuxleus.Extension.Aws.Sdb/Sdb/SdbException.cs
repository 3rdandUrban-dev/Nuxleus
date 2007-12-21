/* -*- Mode: Java; c-basic-offset: 2 -*- */
/*
 * This software code is made available "AS IS" without warranties of any
 * kind.  You may copy, display, modify and redistribute the software
 * code either by itself or as incorporated into your code; provided that
 * you do not remove any proprietary notices.  Your use of this software
 * code is at your own risk and you waive any claim against Amazon
 * Web Services LLC or its affiliates with respect to your use of
 * this software code.
 * 
 * @copyright 2007 Amazon Web Services LLC or its affiliates.
 *            All rights reserved.
 */
using System;

namespace Nuxleus.Extension.Aws.Sdb
{
    public class SdbException : Exception
    {
        private string errorCode;
        public string ErrorCode
        {
            get
            {
                return errorCode;
            }
        }

        private string requestId;
        public string RequestId
        {
            get
            {
                return requestId;
            }
        }

        public SdbException (string requestId, string errorCode,
                string errorMessage)
            :
          base(errorMessage)
        {
            this.errorCode = errorCode;
            this.requestId = requestId;
        }

        public SdbException (string errorCode,
                string errorMessage)
            :
          base(errorMessage)
        {
            this.errorCode = errorCode;
        }
    }
}
