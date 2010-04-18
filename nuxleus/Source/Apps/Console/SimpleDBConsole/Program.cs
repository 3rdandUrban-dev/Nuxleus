using System;
using System.IO;
using System.Threading;
using System.Reflection;
using Nuxleus.Agent;
using Nuxleus.Extension.Aws;
using Nuxleus.Extension.Aws.SimpleDb;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace SimpleDBConsole {

    enum SimpleDBCommand {
        LIST_DOMAINS,
        CREATE_DOMAIN,
        DELETE_DOMAIN,
        GET_ATTRIBUTES,
        PUT_ATTRIBUTES,
        QUERY,
        VERSION,
        EXIT
    }


    public enum Style { Prompt, Out, Error }

    public class Program {

        static bool tabCompletion = true;
        static bool autoIndent = false;
        static bool colorfulConsole = false;

        public static bool TabCompletion {
            get {
                return tabCompletion;
            }
            set {
                tabCompletion = value;
            }
        }

        public static bool AutoIndent {
            get {
                return autoIndent;
            }
            set {
                autoIndent = value;
            }
        }

        public static bool ColorfulConsole {
            get {
                return colorfulConsole;
            }
            set {
                colorfulConsole = value;
            }
        }

        static InteractiveConsole m_console;
        public static InteractiveConsole InteractiveConsole {
            get {
                if (m_console == null) {
                    m_console = new InteractiveConsole(true);
                }
                return m_console;
            }
            set {
                m_console = value;
            }
        }


        static void Main(string[] args) {
            bool continueInteraction = true;
            while (continueInteraction) {
                InteractiveConsole.Write(">>> ", Style.Prompt);
                bool readStatement = ReadStatement(out continueInteraction);
                if (!readStatement) {
                    InteractiveConsole.Write("Operation Not Found\n", Style.Error);
                }
            }
        }

        private static bool TreatAsBlankLine(string line, int autoIndentSize) {
            if (line.Length == 0)
                return true;
            if (autoIndentSize != 0 && line.Trim().Length == 0 && line.Length == autoIndentSize) {
                return true;
            }
            return false;
        }

        public static bool ReadStatement(out bool continueInteraction) {
            StringBuilder b = new StringBuilder();
            int autoIndentSize = 0;

            InteractiveConsole.Write(String.Empty, Style.Prompt);

            while (true) {
                string line = InteractiveConsole.ReadLine(autoIndentSize);
                continueInteraction = true;

                bool allowIncompleteStatement = !TreatAsBlankLine(line, autoIndentSize);
                b.Append(line);
                b.Append("\n");

                string nLine = b.ToString();
                if (nLine == "\n" || (!allowIncompleteStatement && nLine.Trim().Length == 0))
                    return true;

                IOperation operation;
                if (Parser.TryParseLine(nLine, out operation)) {
                    List<string> result = operation.Invoke();
                    foreach (string item in result) {
                        InteractiveConsole.Write(item, Style.Out);
                    }
                    return true;
                } else {
                    return false;
                }

                // Keep on reading input
                //InteractiveConsole.Write(nLine, Style.Prompt);
            }
        }
    }

    class ConsoleOptions {
        internal static bool PrintVersionAndExit = false;
        internal static int AutoIndentSize = 4;
    }

    public class InteractiveConsole {

        private AutoResetEvent ctrlCEvent;
        private Thread MainEngineThread = Thread.CurrentThread;

        public InteractiveConsole(bool colorfulConsole) {

            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
            ctrlCEvent = new AutoResetEvent(false);
            if (colorfulConsole) {
                SetupColors();
            }
        }

        void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
            try {
                if (e.SpecialKey == ConsoleSpecialKey.ControlC) {
                    e.Cancel = true;
                    ctrlCEvent.Set();
                    Thread.CurrentThread.Abort(new KeyboardInterruptException(""));
                }
            } catch {
                this.Write("\n", Style.Out);
            }
        }

        public void SetupColors() {
            PromptColor = ConsoleColor.DarkGray;
            OutColor = ConsoleColor.DarkBlue;
            ErrorColor = ConsoleColor.DarkRed;
        }

        /// <summary>
        /// The console input buffer.
        /// </summary>
        private StringBuilder input = new StringBuilder();
        /// <summary>
        /// Current position - index into the input buffer
        /// </summary>
        private int current = 0;
        /// <summary>
        /// The number of white-spaces displayed for the auto-indenation of the current line
        /// </summary>
        private int autoIndentSize = 0;
        /// <summary>
        /// Length of the output currently rendered on screen.
        /// </summary>
        private int rendered = 0;
        /// <summary>
        /// Cursort anchor - position of cursor when the routine was called
        /// </summary>
        Cursor cursor;
        /// <summary>
        /// Command history
        /// </summary>
        private History history = new History();
        /// <summary>
        /// Tab options available in current context
        /// </summary>
        private SuperConsoleOptions options = new SuperConsoleOptions();
        /// <summary>
        /// Class managing the command history.
        /// </summary>
        class History {
            private ArrayList list = new ArrayList();
            private int current = 0;
            private bool increment = false;     // increment on Next()

            private string Current {
                get {
                    return current >= 0 && current < list.Count ? (string)list[current] : String.Empty;
                }
            }

            public void Add(string line, bool setCurrentAsLast) {
                if (line != null && line.Length > 0) {
                    int oldCount = list.Count;
                    list.Add(line);
                    if (setCurrentAsLast || current == oldCount) {
                        current = list.Count;
                    } else {
                        current++;
                    }
                    // Do not increment on the immediately following Next()
                    increment = false;
                }
            }

            public string Previous() {
                if (current > 0) {
                    current--;
                    increment = true;
                }
                return Current;
            }

            public string Next() {
                if (current + 1 < list.Count) {
                    if (increment)
                        current++;
                    increment = true;
                }
                return Current;
            }
        }

        /// <summary>
        /// List of available options
        /// </summary>
        class SuperConsoleOptions {
            private ArrayList list = new ArrayList();
            private int current = 0;
            private string root;

            public int Count {
                get {
                    return list.Count;
                }
            }

            private string Current {
                get {
                    return current >= 0 && current < list.Count ? (string)list[current] : String.Empty;
                }
            }

            public void Clear() {
                list.Clear();
                current = -1;
            }

            public void Add(string line) {
                if (line != null && line.Length > 0) {
                    list.Add(line);
                }
            }

            public string Previous() {
                if (list.Count > 0) {
                    current = ((current - 1) + list.Count) % list.Count;
                }
                return Current;
            }

            public string Next() {
                if (list.Count > 0) {
                    current = (current + 1) % list.Count;
                }
                return Current;
            }

            public string Root {
                get {
                    return root;
                }
                set {
                    root = value;
                }
            }
        }
        public static string[] GetArgs(string[] args) {
            for (int i = 0; i < args.Length; i++) {
                Console.Write("Enter {0}: ", args[i]);
                args[i] = Console.ReadLine();
            }
            return args;
        }

        private static void PrintVersion() {
            try {
                System.Reflection.Assembly asm
                    = System.Reflection.Assembly.GetAssembly(typeof(SimpleDBConsole.Program));
                Console.WriteLine("SimpleDBConsole-" + asm.GetName().Version);
            } catch {
                Console.WriteLine("SimpleDBConsole v0.2.5.7");
            }
        }

        /// <summary>
        /// Cursor position management
        /// </summary>
        struct Cursor {
            /// <summary>
            /// Beginning position of the cursor - top coordinate.
            /// </summary>
            private int anchorTop;
            /// <summary>
            /// Beginning position of the cursor - left coordinate.
            /// </summary>
            private int anchorLeft;

            public int Top {
                get {
                    return anchorTop;
                }
            }
            public int Left {
                get {
                    return anchorLeft;
                }
            }

            public void Anchor() {
                anchorTop = Console.CursorTop;
                anchorLeft = Console.CursorLeft;
            }

            public void Reset() {
                Console.CursorTop = anchorTop;
                Console.CursorLeft = anchorLeft;
            }

            public void Place(int index) {
                Console.CursorLeft = (anchorLeft + index) % Console.BufferWidth;
                int cursorTop = anchorTop + (anchorLeft + index) / Console.BufferWidth;
                if (cursorTop >= Console.BufferHeight) {
                    anchorTop -= cursorTop - Console.BufferHeight + 1;
                    cursorTop = Console.BufferHeight - 1;
                }
                Console.CursorTop = cursorTop;
            }

            public void Move(int delta) {
                int position = Console.CursorTop * Console.BufferWidth + Console.CursorLeft + delta;
                Console.CursorLeft = position % Console.BufferWidth;
                Console.CursorTop = position / Console.BufferWidth;
            }
        };

        private IEnumerable GetOptionsList(string attr, string pref, string root) {
            string[] attrTree = attr.Split(new char[] { '.' });
            switch (attrTree[0]) {
                case "sdb":
                    return new string[] { "list", "create", "get", "put", "delete", "query" };
                case "ec2":
                    return new string[] { "images", "security", "instances", "block", "delete" };
                case "s3":
                    return new string[] { "get", "put", "delete" };
                case "sqs":
                    if (attrTree.Length >= 2) {
                        switch (attrTree[1]) {
                            case "queue":
                                return new string[] { "list" };
                            default:
                                return new string[] { "queue", "peek", "get", "put", "delete" };
                        }
                    } else {
                        return new string[] { "queue", "peek", "get", "put", "delete" };
                    }

                default:
                    return new string[] { "sdb", "s3", "sqs", "ec2" };
            }
        }

        private bool GetOptions() {
            options.Clear();

            int len;
            for (len = input.Length; len > 0; len--) {
                char c = input[len - 1];
                if (Char.IsLetterOrDigit(c)) {
                    continue;
                } else if (c == '.' || c == '_') {
                    continue;
                } else {
                    break;
                }
            }

            string name = input.ToString(len, input.Length - len);
            if (name.Trim().Length > 0) {
                int lastDot = name.LastIndexOf('.');
                string attr, pref, root;
                if (lastDot < 0) {
                    attr = String.Empty;
                    pref = name;
                    root = input.ToString(0, len);
                } else {
                    attr = name.Substring(0, lastDot);
                    pref = name.Substring(lastDot + 1);
                    root = input.ToString(0, len + lastDot + 1);
                }

                try {
                    Console.WriteLine("attr: {0}, pref: {1}, root: {2}", attr, pref, root);
                    IEnumerable result = GetOptionsList(attr, pref, root);
                    //IEnumerable result = engine.Evaluate(String.Format("dir({0})", attr)) as IEnumerable;
                    options.Root = root;
                    foreach (string option in result) {
                        if (option.StartsWith(pref, StringComparison.CurrentCultureIgnoreCase)) {
                            options.Add(option);
                        }
                    }
                } catch {
                    options.Clear();
                }
                return true;
            } else {
                return false;
            }
        }

        private void SetInput(string line) {
            input.Length = 0;
            input.Append(line);

            current = input.Length;

            Render();
        }

        private void Initialize() {
            cursor.Anchor();
            input.Length = 0;
            current = 0;
            rendered = 0;
            //changed = false;
        }

        private bool BackspaceAutoIndentation() {
            if (input.Length == 0 || input.Length > autoIndentSize)
                return false;

            // Is the auto-indenation all white space, or has the user since edited the auto-indentation?
            for (int i = 0; i < input.Length; i++) {
                if (input[i] != ' ')
                    return false;
            }

            // Calculate the previous indentation level
            int newLength = ((input.Length - 1) / ConsoleOptions.AutoIndentSize) * ConsoleOptions.AutoIndentSize;

            int backspaceSize = input.Length - newLength;
            input.Remove(newLength, backspaceSize);
            current -= backspaceSize;
            Render();
            return true;
        }

        private void Backspace() {
            if (BackspaceAutoIndentation())
                return;

            if (input.Length > 0 && current > 0) {
                input.Remove(current - 1, 1);
                current--;
                Render();
            }
        }

        private void Delete() {
            if (input.Length > 0 && current < input.Length) {
                input.Remove(current, 1);
                Render();
            }
        }

        private void Insert(ConsoleKeyInfo key) {
            char c;
            if (key.Key == ConsoleKey.F6) {
                //Debug.Assert(FinalLineText.Length == 1);

                c = FinalLineText[0];
            } else {
                c = key.KeyChar;
            }
            Insert(c);
        }

        private void Insert(char c) {
            if (current == input.Length) {
                if (Char.IsControl(c)) {
                    string s = MapCharacter(c);
                    current++;
                    input.Append(c);
                    Console.Write(s);
                    rendered += s.Length;
                } else {
                    current++;
                    input.Append(c);
                    Console.Write(c);
                    rendered++;
                }
            } else {
                input.Insert(current, c);
                current++;
                Render();
            }
        }

        private string MapCharacter(char c) {
            if (c == 13)
                return "\r\n";
            if (c <= 26)
                return "^" + ((char)(c + 'A' - 1)).ToString();

            return "^?";
        }

        private int GetCharacterSize(char c) {
            if (Char.IsControl(c)) {
                return MapCharacter(c).Length;
            } else {
                return 1;
            }
        }

        private void Render() {
            cursor.Reset();
            StringBuilder output = new StringBuilder();
            int position = -1;
            for (int i = 0; i < input.Length; i++) {
                if (i == current) {
                    position = output.Length;
                }
                char c = input[i];
                if (Char.IsControl(c)) {
                    output.Append(MapCharacter(c));
                } else {
                    output.Append(c);
                }
            }

            if (current == input.Length) {
                position = output.Length;
            }

            string text = output.ToString();
            Console.Write(text);

            if (text.Length < rendered) {
                Console.Write(new String(' ', rendered - text.Length));
            }
            rendered = text.Length;
            cursor.Place(position);
        }

        private void MoveLeft(ConsoleModifiers keyModifiers) {
            if ((keyModifiers & ConsoleModifiers.Control) != 0) {
                // move back to the start of the previous word
                if (input.Length > 0 && current != 0) {
                    bool nonLetter = IsSeperator(input[current - 1]);
                    while (current > 0 && (current - 1 < input.Length)) {
                        MoveLeft();

                        if (IsSeperator(input[current]) != nonLetter) {
                            if (!nonLetter) {
                                MoveRight();
                                break;
                            }

                            nonLetter = false;
                        }
                    }
                }
            } else {
                MoveLeft();
            }
        }

        private bool IsSeperator(char ch) {
            return !Char.IsLetter(ch);
        }

        private void MoveRight(ConsoleModifiers keyModifiers) {
            if ((keyModifiers & ConsoleModifiers.Control) != 0) {
                // move to the next word
                if (input.Length != 0 && current < input.Length) {
                    bool nonLetter = IsSeperator(input[current]);
                    while (current < input.Length) {
                        MoveRight();

                        if (current == input.Length)
                            break;
                        if (IsSeperator(input[current]) != nonLetter) {
                            if (nonLetter)
                                break;

                            nonLetter = true;
                        }
                    }
                }
            } else {
                MoveRight();
            }
        }

        private void MoveRight() {
            if (current < input.Length) {
                char c = input[current];
                current++;
                cursor.Move(GetCharacterSize(c));
            }
        }

        private void MoveLeft() {
            if (current > 0 && (current - 1 < input.Length)) {
                current--;
                char c = input[current];
                cursor.Move(-GetCharacterSize(c));
            }
        }

        private const int TabSize = 4;
        private void InsertTab() {
            for (int i = TabSize - (current % TabSize); i > 0; i--) {
                Insert(' ');
            }
        }

        private void MoveHome() {
            current = 0;
            cursor.Reset();
        }

        private void MoveEnd() {
            current = input.Length;
            cursor.Place(rendered);
        }

        public string ReadLine(int autoIndentSizeInput) {
            Initialize();

            autoIndentSize = autoIndentSizeInput;
            for (int i = 0; i < autoIndentSize; i++)
                Insert(' ');

            bool inputChanged = false;
            bool optionsObsolete = false;

            for (; ; ) {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key) {
                    case ConsoleKey.Backspace:
                        Backspace();
                        inputChanged = optionsObsolete = true;
                        break;
                    case ConsoleKey.Delete:
                        Delete();
                        inputChanged = optionsObsolete = true;
                        break;
                    case ConsoleKey.Enter:
                        Console.Write("\n");
                        string line = input.ToString();
                        if (line == FinalLineText)
                            return null;
                        if (line.Length > 0) {
                            history.Add(line, inputChanged);
                        }
                        return line;
                    case ConsoleKey.Tab: {
                            bool prefix = false;
                            if (optionsObsolete) {
                                prefix = GetOptions();
                                optionsObsolete = false;
                            }

                            if (options.Count > 0) {
                                string part = (key.Modifiers & ConsoleModifiers.Shift) != 0 ? options.Previous() : options.Next();
                                SetInput(options.Root + part);
                            } else {
                                if (prefix) {
                                    Console.Beep();
                                } else {
                                    InsertTab();
                                }
                            }
                            inputChanged = true;
                            break;
                        }
                    case ConsoleKey.UpArrow:
                        SetInput(history.Previous());
                        optionsObsolete = true;
                        inputChanged = false;
                        break;
                    case ConsoleKey.DownArrow:
                        SetInput(history.Next());
                        optionsObsolete = true;
                        inputChanged = false;
                        break;
                    case ConsoleKey.RightArrow:
                        MoveRight(key.Modifiers);
                        optionsObsolete = true;
                        break;
                    case ConsoleKey.LeftArrow:
                        MoveLeft(key.Modifiers);
                        optionsObsolete = true;
                        break;
                    case ConsoleKey.Escape:
                        SetInput(String.Empty);
                        inputChanged = optionsObsolete = true;
                        break;
                    case ConsoleKey.Home:
                        MoveHome();
                        optionsObsolete = true;
                        break;
                    case ConsoleKey.End:
                        MoveEnd();
                        optionsObsolete = true;
                        break;
                    case ConsoleKey.LeftWindows:
                    case ConsoleKey.RightWindows:
                        // ignore these
                        continue;
                    default:
                        if (key.KeyChar == '\x0D')
                            goto case ConsoleKey.Enter;      // Ctrl-M
                        if (key.KeyChar == '\x08')
                            goto case ConsoleKey.Backspace;  // Ctrl-H
                        Insert(key);
                        inputChanged = optionsObsolete = true;
                        break;
                }
            }
        }

        string FinalLineText {
            get {
                return Environment.OSVersion.Platform != PlatformID.Unix ? "\x1A" : "\x04";
            }
        }

        public ConsoleColor PromptColor = Console.ForegroundColor;
        public ConsoleColor OutColor = Console.ForegroundColor;
        public ConsoleColor ErrorColor = Console.ForegroundColor;


        public void Write(string text, Style style) {
            switch (style) {
                case Style.Prompt:
                    WriteColor(text, PromptColor);
                    break;
                case Style.Out:
                    WriteColor(text, OutColor);
                    break;
                case Style.Error:
                    WriteColor(text, ErrorColor);
                    break;
            }
        }

        public void WriteLine(string text, Style style) {
            Write(text + Environment.NewLine, style);
        }

        private void WriteColor(string s, ConsoleColor c) {
            ConsoleColor origColor = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.Write(s);
            Console.ForegroundColor = origColor;
        }
    }

    [Serializable]
    public class KeyboardInterruptException : Exception {
        public KeyboardInterruptException() : base() { }
        public KeyboardInterruptException(string msg) : base(msg) { }
        public KeyboardInterruptException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}