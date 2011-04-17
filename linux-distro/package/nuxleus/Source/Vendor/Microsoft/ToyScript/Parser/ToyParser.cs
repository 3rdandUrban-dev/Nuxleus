/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Microsoft Permissive License. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Microsoft Permissive License, please send an email to 
 * ironpy@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Microsoft Permissive License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Scripting.Internal.Ast;
using System.Diagnostics;
using Microsoft.Scripting;


namespace ToyScript.Parser {
    class ToyParser {
        private IEnumerator<Token> _tokens;

        public ToyParser(string text) {
            _tokens = new ToyTokenizer(text).GetTokens().GetEnumerator();
        }

        public Node Parse()
        {
            Expression curExpr = null;
            for (; ; ) {
                _tokens.MoveNext();
                Token t = _tokens.Current;

                switch (t.Kind) {
                    case TokenKinds.EOF:
                        return curExpr;
                    case TokenKinds.Operator:
                        if (curExpr == null) {
                            throw new SyntaxErrorException("expected value before operator");
                        }

                        curExpr = ActionExpression.Operator(((OperatorToken)t).DlrOperator,
                            curExpr,
                            (Expression)Parse());
                        break;
                    case TokenKinds.Constant:
                        curExpr = new ConstantExpression(((ConstantToken)t).Value);
                        break;
                }
            }
        }

        public Statement ParseStatement() {
            Node n = Parse();

            return n as Statement ?? new ExpressionStatement((Expression)n);
        }

        private static Expression MakePrintExpr(Expression input) {
            return MethodCallExpression.Call(null, typeof(ToyHelpers).GetMethod("Print"), input);
        }

        public Statement ParseInteractiveStatement() {
            Node n = Parse();

            return n as Statement ?? new ExpressionStatement(MakePrintExpr((Expression)n));
        }
    }

    class ToyTokenizer {
        private string _text;

        public ToyTokenizer(string text) {
            _text = text;
        }

        public IEnumerable<Token> GetTokens() {
            int index = 0;
            while (index < _text.Length) {
                char ch = _text[index++];
                int start;
                switch (ch) {
                    case '+':
                        yield return new OperatorToken(OperatorKinds.Plus, index - 1, index);
                        break;
                    case '-':
                        yield return new OperatorToken(OperatorKinds.Minus, index - 1, index);
                        break;
                    case '*':
                        yield return new OperatorToken(OperatorKinds.Multiply, index - 1, index);
                        break;
                    case '/':
                        yield return new OperatorToken(OperatorKinds.Divide, index - 1, index);
                        break;
                    case '.':
                        yield return new OperatorToken(OperatorKinds.GetMember, index - 1, index);
                        break;
                    case '(':
                        yield return new OperatorToken(OperatorKinds.OpenParen, index - 1, index);
                        break;
                    case ')':
                        yield return new OperatorToken(OperatorKinds.CloseParen, index - 1, index);
                        break;
                    case '"':
                        start = index;
                        while (_text[++index] != '"') ;
                        index++;
                        yield return new ConstantToken(_text.Substring(start, index - start - 2), start - 1, index);
                        break;
                    default:
                        if (Char.IsWhiteSpace(ch)) {
                            continue;
                        } else if (Char.IsDigit(ch)) {
                            start = index - 1;
                            int length = 1;
                            while (Char.IsDigit(_text[start + length])) {
                                length++;
                                index++;
                            }
                            yield return new ConstantToken(Int32.Parse(_text.Substring(start, length)), start, start+length);
                        } else if (Char.IsLetter(ch)) {
                            throw new NotImplementedException();
                        } else {
                            throw new NotImplementedException();
                        }
                        break;
                }
            }
            yield return new EOFToken();
        }

    }

    abstract class Token {
        private int _start, _end;
        protected Token(int start, int end) {
            _start = start;
            _end = end;
        }

        public int Start {
            get {
                return _start;
            }

        }

        public int End {
            get {
                return _end;
            }
        }

        public abstract TokenKinds Kind {
            get;            
        }
    }

    class OperatorToken : Token {
        private OperatorKinds _operator;

        public OperatorToken(OperatorKinds oper, int start, int end)
            : base(start, end) {
            _operator = oper;
        }

        public override TokenKinds Kind {
            get { return TokenKinds.Operator; }
        }

        public OperatorKinds Operator {
            get {
                return _operator;
            }
        }

        public Operators DlrOperator {
            get {
                switch (Operator) {
                    case OperatorKinds.Plus: return Operators.Add;
                    case OperatorKinds.Minus: return Operators.Subtract;
                    case OperatorKinds.Multiply: return Operators.Multiply;
                    case OperatorKinds.Divide: return Operators.Divide;
                    default: throw new NotImplementedException();
                }
            }
        }
    }

    class ConstantToken : Token {
        private object _value;

        public ConstantToken(object token, int start, int end)
            : base(start, end) {
            _value = token;
        }

        public override TokenKinds Kind {
            get { return TokenKinds.Constant; }
        }
        public object Value {
            get {
                return _value;
            }
        }
    }

    class EOFToken : Token {
        public EOFToken()
            : base(-1, -1) {
        }

        public override TokenKinds Kind {
            get { return TokenKinds.EOF; }
        }

    }


    enum TokenKinds {
        None,
        Constant,
        Operator,
        EOF
    }

    enum OperatorKinds {
        None,
        Plus,
        Minus,
        Divide,
        Multiply,
        GetMember,
        OpenParen,
        CloseParen,

    }
}

