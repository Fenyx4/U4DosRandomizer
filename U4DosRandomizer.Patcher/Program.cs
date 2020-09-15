using Microsoft.Extensions.CommandLineUtils;
using Octodiff.Core;
using Octodiff.Diagnostics;
using System;
using System.IO;

namespace U4DosRandomizer.Patcher
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLineApplication commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: false);

            CommandOption basePathArg = commandLineApplication.Option(
                "-b |--b <path>",
                "Path to base file. ",
                CommandOptionType.SingleValue);

            CommandOption sigPathArg = commandLineApplication.Option(
                "-s |--s <path>",
                "Path to signature file. ",
                CommandOptionType.SingleValue);

            CommandOption newPathArg = commandLineApplication.Option(
                "-n |--n <path>",
                "Path to new file. ",
                CommandOptionType.SingleValue);

            CommandOption deltaPathArg = commandLineApplication.Option(
                "-d |--d <path>",
                "Path to delta file. ",
                CommandOptionType.SingleValue);

            commandLineApplication.OnExecute(() =>
            {
                var signatureBaseFilePath = Directory.GetCurrentDirectory();
                if (basePathArg.HasValue())
                {
                    if (!File.Exists(basePathArg.Value()))
                    {
                        throw new ArgumentException("Path provided does not exist");
                    }
                    else
                    {
                        signatureBaseFilePath = basePathArg.Value();
                    }
                }
                else
                {
                    throw new ArgumentException("Must provide base path.");
                }

                var signatureFilePath = Directory.GetCurrentDirectory();
                if (sigPathArg.HasValue())
                {
                    signatureFilePath = sigPathArg.Value();
                }
                else
                {
                    throw new ArgumentException("Must provide signature path.");
                }

                var signatureOutputDirectory = Path.GetDirectoryName(signatureFilePath);
                if (!Directory.Exists(signatureOutputDirectory))
                    Directory.CreateDirectory(signatureOutputDirectory);
                var signatureBuilder = new SignatureBuilder();

                using (var basisStream = new FileStream(signatureBaseFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var signatureStream = new FileStream(signatureFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    signatureBuilder.Build(basisStream, new SignatureWriter(signatureStream));
                }

                var newFilePath = Directory.GetCurrentDirectory();
                if (newPathArg.HasValue())
                {
                    if (!File.Exists(newPathArg.Value()))
                    {
                        throw new ArgumentException("Path provided does not exist");
                    }
                    else
                    {
                        newFilePath = newPathArg.Value();
                    }
                }
                else
                {
                    throw new ArgumentException("Must provide new file path.");
                }

                var deltaFilePath = Directory.GetCurrentDirectory();
                if (deltaPathArg.HasValue())
                {
                    deltaFilePath = deltaPathArg.Value();
                }
                else
                {
                    throw new ArgumentException("Must provide delta path.");
                }

                var deltaOutputDirectory = Path.GetDirectoryName(deltaFilePath);
                if (!Directory.Exists(deltaOutputDirectory))
                    Directory.CreateDirectory(deltaOutputDirectory);
                var deltaBuilder = new DeltaBuilder();
                using (var newFileStream = new FileStream(newFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var signatureFileStream = new FileStream(signatureFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var deltaStream = new FileStream(deltaFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    deltaBuilder.BuildDelta(newFileStream, new SignatureReader(signatureFileStream, new ConsoleProgressReporter()), new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(deltaStream)));
                }

                return 0;
            });
            commandLineApplication.Execute(args);
        }
    }
}
