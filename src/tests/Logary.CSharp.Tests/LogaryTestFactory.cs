using System.IO;
using System.Text.RegularExpressions;
using Logary.Configuration;
using Logary.EventsProcessing;
using Logary.Targets;
using TextWriter = Logary.Targets.TextWriter;
using Hopac;

namespace Logary.CSharp.Tests
{
    public static class LogaryTestFactory
    {
        public static LogManager GetManager()
        {
            var tw = new StringWriter();
            return ConfigureForTextWriter(tw);
        }

        public static LogManager GetManager(out StringWriter allOutput)
        {
            allOutput = new StringWriter();
            return ConfigureForTextWriter(allOutput);
        }

        public static LogManager ConfigureForTextWriter(StringWriter tw)
        {
            var twTarg =
                TextWriter.Create(
                    TextWriter.TextWriterConf.Create(tw, tw,
                        new Microsoft.FSharp.Core.FSharpOption<MessageWriter>(MessageWriterModule.levelDatetimeMessagePathNewLine)),
                    "tw");

            // var twRule =
            //     RuleModule.Create(new Regex(@"^Logary\.CSharp\.Tests"), "tw", LogLevel.Verbose,
            //         message => true);
            var sinks = FSharpList.Create("tw");
            var processor = Events.sink<Alt<Promise<Microsoft.FSharp.Core.Unit>>,Message>(sinks, Events.events<Alt<Promise<Microsoft.FSharp.Core.Unit>>>());

            var config = Config.create("Logary.CSharp.Tests C# low level API","localhost");
            config = Config.ilogger(ILogger.NewConsole(LogLevel.Warn),config);
            config = Config.target(twTarg, config);
            config = Config.processing(processor, config);
            var logary = Config.build(config).ToTask().Result;
            return logary;
        }
    }
}