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
namespace Nuxleus.Extension.Aws.Sdb
{
    public struct Attribute
    {
        string m_name;
        string m_value;

        public Attribute (string Name)
            : this(Name, null)
        {
        }

        public Attribute (string Name, string Value)
        {
            m_name = Name;
            m_value = Value;
        }

        public string Name
        {
            get { return m_name; }
        }

        public string Value
        {
            get { return m_value; }
        }
    }
}
