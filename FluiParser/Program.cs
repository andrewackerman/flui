using FluiParser.Language;
using FluiParser.Language.Generator;
using FluiParser.Language.Parser;
using FluiParser.Language.Tokenizer;
using FluiParser.Utility;
using System;
using System.IO;
using System.Linq;

namespace FluiParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string testFile = "sample.flui";
            string fileprefix = Path.GetFileNameWithoutExtension(testFile);
            SourceCode code = new SourceCode(File.ReadAllText(testFile));

            var tokens = Tokenizer.Instance.TokenizeFile(code).ToList();
            var errors = Tokenizer.Instance.ErrorSink.ToList();

            File.WriteAllText("tokens.json", Newtonsoft.Json.JsonConvert.SerializeObject(tokens, Newtonsoft.Json.Formatting.Indented));
            File.WriteAllText("token_errors.json", Newtonsoft.Json.JsonConvert.SerializeObject(errors, Newtonsoft.Json.Formatting.Indented));

            var symbolDoc = Parser.Instance.ParseFile(code, tokens);

            File.WriteAllText("symbols.json", Newtonsoft.Json.JsonConvert.SerializeObject(symbolDoc, Newtonsoft.Json.Formatting.Indented));

            var view = Generator.Instance.GenerateViewFile(symbolDoc);
            var viewModel = Generator.Instance.GenerateViewModelFile(symbolDoc);

            File.WriteAllText($"{symbolDoc.ViewClassName.PascalCaseToUnderscore()}.dart", view);
            File.WriteAllText($"{symbolDoc.ViewModelClassName.PascalCaseToUnderscore()}.dart", viewModel);
        }
    }
}
