using System;
using System.Collections;
using System.Collections.Generic;
using com.amazon.s3;

namespace X5 {

    public class ClipItem {
        /// <summary>
        /// 
        /// </summary>

        public ClipItem () { }
        /// <summary>
        /// 
        /// </summary>
        private string _Data;

        public string Data {

            get {
                return this._Data;
            }
            set {
                this._Data = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private SortedList _MetaData = new SortedList();

        public SortedList MetaData {
            get {
                return this._MetaData;
            }

            set {

                this._MetaData = value;
            }
        }
    }
}
