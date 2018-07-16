using FluiParser.Language.Syntax;
using FluiParser.Language.Syntax.Nodes;
using FluiParser.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language.Generator
{
    public sealed partial class Generator
    {
        private static readonly string _viewHeader = "// DO NOT WRITE CODE IN THIS FILE\n// IT WILL GET OVERWRITTEN WHEN THE UI CODE IS REBUILT\n\nimport 'package:flutter/material.dart';\n\nimport './{0}.dart';\n\nclass {1} extends {2} {{\n\t@override\n\tWidget build(BuildContext context) {{\n\t\treturn ";
        private static readonly string _viewFooter = ";\n\t}\n}";

        private int _indentLevel;
        private string Indent => new string('\t', _indentLevel);

        private void ParseSymbolsForView()
        {
            _indentLevel = 2;

            string viewModelClass = _sourceDoc.ViewModelClassName;
            string viewClass = _sourceDoc.ViewClassName;
            _builder.Append(string.Format(_viewHeader, viewModelClass.PascalCaseToUnderscore(), viewClass, viewModelClass));

            ElementSingleNode root = _sourceDoc.ViewClass.Child as ElementSingleNode;
            ParseElementSingle(root, true);

            _builder.Append(_viewFooter);
        }

        private void ParseSymbol(SyntaxNode node, bool insertIndent = true)
        {
            if (insertIndent)
            {
                _builder.Append(Indent);
            }

            switch (node.Kind)
            {
                case SyntaxKind.ElementNode:
                    ParseElement(node as ElementNode);
                    break;

                case SyntaxKind.ElementSingleNode:
                    ParseElementSingle(node as ElementSingleNode);
                    break;

                case SyntaxKind.AttributeNode:
                    ParseAttribute(node as AttributeNode);
                    break;

                case SyntaxKind.AttributeSingleNode:
                    ParseAttributeSingle(node as AttributeSingleNode);
                    break;

                case SyntaxKind.IdentifierNode:
                    ParseIdentifier(node as IdentifierNode);
                    break;

                case SyntaxKind.FunctionCallNode:
                    ParseFunctionCall(node as FunctionCallNode);
                    break;

                case SyntaxKind.CallbackNode:
                    ParseCallback(node as CallbackNode);
                    break;

                case SyntaxKind.ConstantNode:
                    ParseConstant(node as ConstantNode);
                    break;

                default:
                    throw new NotImplementedException($"Unrecognized node type {node.Kind}");
            }
        }

        private void ParseElement(ElementNode node)
        {
            _builder.Append($"{node.Value} (");

            _indentLevel++;

            foreach (var child in node.Children)
            {
                _builder.AppendLine();
                ParseSymbol(child);

                if (child.Category == SyntaxCategory.Value || child.Kind == SyntaxKind.AttributeSingleNode || child.Kind == SyntaxKind.ElementSingleNode)
                {
                    _builder.Append(",");
                }
            }

            _indentLevel--;

            _builder.AppendLine();
            _builder.Append($"{Indent}),");
        }

        private void ParseElementSingle(ElementSingleNode node, bool isRoot = false)
        {
            _builder.Append($"{node.Value}(");

            if (node.Child.Category == SyntaxCategory.Value)
            {
                ParseSymbol(node.Child, false);
                _builder.Append($")");
            }
            else
            {
                _builder.AppendLine();
                _indentLevel++;
                ParseSymbol(node.Child);
                _indentLevel--;
                _builder.AppendLine();
                _builder.Append($"{Indent})");
            }

            if (!isRoot)
            {
                _builder.Append(",");
            }
        }

        private void ParseAttribute(AttributeNode node)
        {
            _builder.Append($"{node.Value}: [");

            _indentLevel++;

            foreach (var child in node.Children)
            {
                _builder.AppendLine();
                ParseSymbol(child);
            }

            _indentLevel--;

            _builder.AppendLine();
            _builder.Append($"{Indent}],");
        }

        private void ParseAttributeSingle(AttributeSingleNode node)
        {
            _builder.Append($"{node.Value}: ");
            ParseSymbol(node.Child, false);
        }

        private void ParseIdentifier(IdentifierNode node)
        {
            _builder.Append(node.Value);
        }

        private void ParseCallback(CallbackNode node)
        {
            _builder.Append($"() => {node.Value}()");
        }

        private void ParseFunctionCall(FunctionCallNode node)
        {
            _builder.Append($"{node.Value}()");
        }

        private void ParseConstant(ConstantNode node)
        {
            switch (node.ConstantKind)
            {
                case ConstantKind.Null:
                    _builder.Append("null");
                    break;

                case ConstantKind.Boolean:
                case ConstantKind.Integer:
                case ConstantKind.Float:
                    _builder.Append(node.Value);
                    break;

                case ConstantKind.String:
                    _builder.Append($"'{node.Value}'");
                    break;

                default:
                    throw new NotImplementedException($"Unrecognized constnat type {node.ConstantKind}");
            }
        }
    }
}
