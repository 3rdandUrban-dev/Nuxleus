﻿using System.Collections.Generic;
using Antlr.Runtime;

namespace Lextm.SharpSnmpLib.Mib
{
    public partial class SmiParser
    {
        public readonly IList<CompilerError> Errors = new List<CompilerError>();
        public readonly IList<CompilerWarning> Warnings = new List<CompilerWarning>();

        public string FileName { get; private set; }

        public MibDocument GetDocument(string fileName)
        {
            Errors.Clear();
            Warnings.Clear();
            FileName = fileName;
            var document = statement().result;
            document.FileName = fileName;
            return document;
        }

        public MibDocument GetDocument()
        {
            return GetDocument(string.Empty);
        }

        public override void ReportError(RecognitionException e)
        {
            Errors.Add(new CompilerError(e) {FileName = FileName});
            base.ReportError(e);
        }

        private void WantModuleName(IToken token)
        {
            if (!token.IsValidIdentifier(this))
            {
                return;
            }

            if (!token.IsPascalCase())
            {
                var message = string.IsNullOrEmpty(FileName)
                                  ? string.Format(
                                      "warning N0005 : module names should all be in uppercase. Rename {0} to {1}",
                                      token.Text, token.Text.PascalCase())
                                  : string.Format(
                                      "{0} ({1},{2}) : warning N0005 : module names should all be in uppercase. Rename {3} to {4}",
                                      FileName,
                                      token.Line, token.CharPositionInLine + 1, token.Text,
                                      token.Text.PascalCase());
                Warnings.Add(new CompilerWarning(token, FileName, message));
            }
        }

        private void WantTypeName(IToken token)
        {
            if (!token.IsValidIdentifier(this))
            {
                return;
            }

            if (!token.IsPascalCase())
            {
                var message = string.IsNullOrEmpty(FileName)
                                  ? string.Format(
                                      "warning N0011 : type names should all be in Pascal case. Rename {0} to {1}",
                                      token.Text, token.Text.PascalCase())
                                  : string.Format(
                                      "{0} ({1},{2}) : warning N0011 : type names should all be in Pascal case. Rename {3} to {4}",
                                      FileName,
                                      token.Line, token.CharPositionInLine + 1, token.Text, token.Text.PascalCase());
                Warnings.Add(new CompilerWarning(token, FileName, message));
            }
        }

        private void WantMacroName(IToken token)
        {
            if (!token.IsValidIdentifier(this))
            {
                return;
            }

            if (!token.IsUppercase())
            {
                var message = string.IsNullOrEmpty(FileName)
                                  ? string.Format(
                                      "warning N0012 : macro names should all be in uppercase. Rename {0} to {1}",
                                      token.Text, token.Text.ToUpperInvariant())
                                  : string.Format(
                                      "{0} ({1},{2}) : warning N0012 : macro names should all be in uppercase. Rename {3} to {4}",
                                      FileName,
                                      token.Line, token.CharPositionInLine + 1, token.Text,
                                      token.Text.ToUpperInvariant());
                Warnings.Add(new CompilerWarning(token, FileName, message));
            }
        }

        private void WantObjectName(IToken token)
        {
            if (!token.IsValidIdentifier(this))
            {
                return;
            }

            if (!token.IsCamelCase())
            {
                var message = string.IsNullOrEmpty(FileName)
                                  ? string.Format(
                                      "warning N0013 : object names should all be in camel case. Rename {0} to {1}",
                                      token.Text, token.Text.CamelCase())
                                  : string.Format(
                                      "{0} ({1},{2}) : warning N0013 : object names should all be in camel case. Rename {3} to {4}",
                                      FileName,
                                      token.Line, token.CharPositionInLine + 1, token.Text, token.Text.CamelCase());
                Warnings.Add(new CompilerWarning(token, FileName, message));
            }
        }

        private void WantNumberName(IToken token)
        {
            if (!token.IsValidIdentifier(this))
            {
                return;
            }

            if (!token.IsCamelCase())
            {
                var message = string.IsNullOrEmpty(FileName)
                                  ? string.Format(
                                      "warning N0014 : number names should all be in camel case. Rename {0} to {1}",
                                      token.Text, token.Text.CamelCase())
                                  : string.Format(
                                      "{0} ({1},{2}) : warning N0014 : number names should all be in camel case. Rename {3} to {4}",
                                      FileName,
                                      token.Line, token.CharPositionInLine + 1, token.Text, token.Text.CamelCase());
                Warnings.Add(new CompilerWarning(token, FileName, message));
            }
        }

        private void WantIdName(IToken token)
        {
            if (!token.IsValidIdentifier(this))
            {
                return;
            }

            if (!token.IsCamelCase())
            {
                var message = string.IsNullOrEmpty(FileName)
                                  ? string.Format(
                                      "warning N0015 : identifier names should all be in camel case. Rename {0} to {1}",
                                      token.Text, token.Text.CamelCase())
                                  : string.Format(
                                      "{0} ({1},{2}) : warning N0015 : identifier names should all be in camel case. Rename {3} to {4}",
                                      FileName,
                                      token.Line, token.CharPositionInLine + 1, token.Text, token.Text.CamelCase());
                Warnings.Add(new CompilerWarning(token, FileName, message));
            }
        }

        private void WantConstraintName(IToken token)
        {
            if (!token.IsValidIdentifier(this))
            {
                return;
            }

            if (!token.IsCamelCase())
            {
                var message = string.IsNullOrEmpty(FileName)
                                  ? string.Format(
                                      "warning N0016 : constraint names should all be in camel case. Rename {0} to {1}",
                                      token.Text, token.Text.CamelCase())
                                  : string.Format(
                                      "{0} ({1},{2}) : warning N0016 : constraint names should all be in camel case. Rename {3} to {4}",
                                      FileName,
                                      token.Line, token.CharPositionInLine + 1, token.Text, token.Text.CamelCase());
                Warnings.Add(new CompilerWarning(token, FileName, message));
            }
        }

        private void WantChoiceName(IToken token)
        {
            if (!token.IsValidIdentifier(this))
            {
                return;
            }

            if (!token.IsCamelCase())
            {
                var message = string.IsNullOrEmpty(FileName)
                                  ? string.Format(
                                      "warning N0017 : choice names should all be in camel case. Rename {0} to {1}",
                                      token.Text, token.Text.CamelCase())
                                  : string.Format(
                                      "{0} ({1},{2}) : warning N0017 : choice names should all be in camel case. Rename {3} to {4}",
                                      FileName,
                                      token.Line, token.CharPositionInLine + 1, token.Text, token.Text.CamelCase());
                Warnings.Add(new CompilerWarning(token, FileName, message));
            }
        }

        private void WantBitName(IToken token)
        {
            if (!token.IsValidIdentifier(this))
            {
                return;
            }

            if (!token.IsCamelCase())
            {
                var message = string.IsNullOrEmpty(FileName)
                                  ? string.Format(
                                      "warning N0018 : bit names should all be in camel case. Rename {0} to {1}",
                                      token.Text, token.Text.CamelCase())
                                  : string.Format(
                                      "{0} ({1},{2}) : warning N0018 : bit names should all be in camel case. Rename {3} to {4}",
                                      FileName,
                                      token.Line, token.CharPositionInLine + 1, token.Text, token.Text.CamelCase());
                Warnings.Add(new CompilerWarning(token, FileName, message));
            }
        }

        private void WantCamelCase(IToken token)
        {
            if (!token.IsValidIdentifier(this))
            {
                return;
            }

            if (!token.IsCamelCase())
            {
                var message = string.IsNullOrEmpty(FileName)
                                  ? string.Format(
                                      "warning N0019 : names should all be in camel case. Rename {0} to {1}",
                                      token.Text, token.Text.CamelCase())
                                  : string.Format(
                                      "{0} ({1},{2}) : warning N0019 : names should all be in camel case. Rename {3} to {4}",
                                      FileName,
                                      token.Line, token.CharPositionInLine + 1, token.Text, token.Text.CamelCase());
                Warnings.Add(new CompilerWarning(token, FileName, message));
            }
        }

        private void WantPascalCase(IToken token)
        {
            if (!token.IsValidIdentifier(this))
            {
                return;
            }

            if (!token.IsPascalCase())
            {
                var message = string.IsNullOrEmpty(FileName)
                                  ? string.Format(
                                      "warning N0020 : names should all be in Pascal case. Rename {0} to {1}",
                                      token.Text, token.Text.PascalCase())
                                  : string.Format(
                                      "{0} ({1},{2}) : warning N0020 : names should all be in Pascal case. Rename {3} to {4}",
                                      FileName,
                                      token.Line, token.CharPositionInLine + 1, token.Text, token.Text.PascalCase());
                Warnings.Add(new CompilerWarning(token, FileName, message));
            }
        }
    }
}
