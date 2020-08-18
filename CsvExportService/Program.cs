using System;
using Topshelf;

namespace CsvExportService
{
    class Program
    {
        static void Main(string[] args)
        {

            var exitCode = HostFactory.Run(hc =>
                {
                    hc.Service<CsvLocator>(s =>
                    {
                        s.ConstructUsing(csvLocator => new CsvLocator());
                        s.WhenStarted(csvLocator => csvLocator.Start());
                        s.WhenStopped(csvLocator => csvLocator.Stop());
                    });
                    hc.RunAsLocalSystem();

                    hc.SetServiceName("CsvLocator");
                    hc.SetDescription("This is a service locating .csv files in a specified directory");
                    hc.SetDisplayName("Csv Locator");
                });

            var exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
