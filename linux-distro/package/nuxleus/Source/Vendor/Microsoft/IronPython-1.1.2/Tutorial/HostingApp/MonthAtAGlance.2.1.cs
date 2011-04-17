/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public
 * License. A  copy of the license can be found in the License.html file at the
 * root of this distribution. If  you cannot locate the  Microsoft Public
 * License, please send an email to  dlr@microsoft.com. By using this source
 * code in any fashion, you are agreeing to be bound by the terms of the 
 * Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using IronPython.Hosting;
using IronPython.Compiler;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace HostingApp {
    public partial class MonthAtAGlance : Form {

        // Application variables
        private Dictionary<System.DateTime, string> dateEntries;
        private System.DateTime lastDate;

        private PythonEngine engine;

        public MonthAtAGlance() {
            InitializeComponent();
            InitializeApp();
            InitializePythonEngine();
        }

        void InitializePythonEngine() {
            engine = new PythonEngine();
            engine.Import("site");
            engine.Globals["dateentries"] = dateEntries;
            engine.Execute("print y");
        }


        public delegate void DateTextChangedEventHandler(DateTime day, string newText);

        public event DateTextChangedEventHandler EntryChangedHandler;

        private void defaultEvtHandler(DateTime day, string nt) {

        }


        private void InitializeApp() {
            dateEntries = new Dictionary<DateTime, string>();
            lastDate = monthCalendar.TodayDate;
            EntryChangedHandler = new DateTextChangedEventHandler(defaultEvtHandler);


        }

        private bool dateSubmitEvent(DateTime dt, string txt) {
            string oldDateText; // current text in the date we are leaving
            // if txt != this, place txt in the date we are leaving
            string newDateText; // current text in the date we are entering
            // to be moved to textBox

            bool wasUpdate = true;

            if (!dateEntries.TryGetValue(dt, out newDateText))
                newDateText = "";
            textBox.Text = newDateText;

            if (!dateEntries.TryGetValue(lastDate, out oldDateText))
                oldDateText = "";

            if (oldDateText == txt)
                wasUpdate = false;

            else {
                if (dateEntries.ContainsKey(lastDate))
                    dateEntries.Remove(lastDate);
                if (txt != "")
                    dateEntries.Add(lastDate, txt);
                else
                    wasUpdate = false;
            }

            lastDate = dt;
            return wasUpdate;
        }

        private void monthCalendar_DateChanged(object sender, DateRangeEventArgs e) {
            DateTime changedTo = e.End;
            string currText = textBox.Text;
            if (dateSubmitEvent(changedTo, currText) &&
                EntryChangedHandler != null)
                EntryChangedHandler(changedTo, currText);

            engine.Execute("n = dateentries.Count");
            int nEntries = engine.EvaluateAs<int>("n");
            label.Text = "There are now " + nEntries.ToString() + " entries in the dictionary.";
        }
    }
}