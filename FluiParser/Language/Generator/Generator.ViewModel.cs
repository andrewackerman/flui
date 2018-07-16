using FluiParser.Language.Syntax;
using FluiParser.Language.Syntax.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language.Generator
{
    public sealed partial class Generator
    {
        private static readonly string _viewModelHeader = "import 'package:flutter/material.dart';\n\nclass {0} extends StatelessWidget {{\n";
        private static readonly string _viewModelFooter = "\n}\n";

        private static readonly string _identifierViewModel = "\tvar {0} = null; // TODO: Populate field {0}";
        private static readonly string _functionViewModel = "\t{0}() {{\n\t\t// TODO: Populate function {0}\n\t}}";

        private bool _memberAdded;

        private void ParseSymbolsForViewModel()
        {
            _memberAdded = false;

            string viewModelClass = _sourceDoc.ViewModelClassName;
            _builder.Append(string.Format(_viewModelHeader, viewModelClass));

            ElementSingleNode root = _sourceDoc.ViewClass.Child as ElementSingleNode;
            ParseNode(root);

            _builder.Append(_viewModelFooter);
        }

        private void ParseNode(SyntaxNode node)
        {
            switch (node.Kind)
            {
                case SyntaxKind.ElementNode:
                    foreach (var child in (node as ElementNode).Children) ParseNode(child);
                    break;

                case SyntaxKind.AttributeNode:
                    foreach (var child in (node as AttributeNode).Children) ParseNode(child);
                    break;

                case SyntaxKind.ElementSingleNode:
                    ParseNode((node as ElementSingleNode).Child);
                    break;

                case SyntaxKind.AttributeSingleNode:
                    ParseNode((node as AttributeSingleNode).Child);
                    break;

                case SyntaxKind.IdentifierNode:
                    ParseIdentifierViewModel(node as IdentifierNode);
                    break;

                case SyntaxKind.FunctionCallNode:
                    ParseFunctionCallViewModel(node as FunctionCallNode);
                    break;

                case SyntaxKind.CallbackNode:
                    ParseCallbackViewModel(node as CallbackNode);
                    break;

                case SyntaxKind.ConstantNode:
                    break;

                default:
                    throw new NotImplementedException($"Unrecognized node type {node.Kind}");
            }
        }

        private void ParseFunctionCallViewModel(FunctionCallNode node)
        {
            if (_memberAdded)
            {
                _builder.AppendLine();
                _builder.AppendLine();
            }

            _builder.Append(string.Format(_functionViewModel, node.Value));

            _memberAdded = true;
        }

        private void ParseCallbackViewModel(CallbackNode node)
        {
            if (_memberAdded)
            {
                _builder.AppendLine();
                _builder.AppendLine();
            }

            _builder.Append(string.Format(_functionViewModel, node.Value));

            _memberAdded = true;
        }

        private void ParseIdentifierViewModel(IdentifierNode node)
        {
            if (_memberAdded)
            {
                _builder.AppendLine();
                _builder.AppendLine();
            }

            _builder.Append(string.Format(_identifierViewModel, node.Value));

            _memberAdded = true;
        }
    }
}
