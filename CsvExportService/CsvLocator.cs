using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

namespace CsvExportService
{
    public class CsvLocator
    {
        private const string fileFormat = "*.csv";
        private IConfigurationRoot configuration;

        private Timer timer;
        private DateTime lastCheckTime;


        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            var dirName = this.configuration.GetSection("DefaultearchDirectoryPath").Value;
            if (!Directory.Exists(dirName))
            {
                throw new ArgumentException($"Directory {dirName} is missing");
            }

            var lastWriteTime = Directory.GetLastWriteTime(dirName);
            if (lastWriteTime > this.lastCheckTime)
            {
                var fileInfos = Directory.GetFiles(dirName, fileFormat)
                 .Select(f => new FileInfo(f))
                .Where(f => f.LastAccessTime >= lastCheckTime);
                ExportToDB(fileInfos);

                this.lastCheckTime = DateTime.Now;
            }
        }

        private void ExportToDB(IEnumerable<FileInfo> fileInfos)
        {
            if (fileInfos.Count() > 0)
            {
                using (var dbContext = new ShopDbContext(configuration.GetConnectionString("DefaultConnection")))
                {
                    foreach (var fileInfo in fileInfos)
                    {
                        //TODO: extract csv parser
                        var file = File.ReadAllLines(fileInfo.FullName);
                        var items = new List<ItemEntity>();

                        foreach (var line in file)
                        {
                            var arr = line.Split(';');
                            //TODO: add check for array size
                            if (arr.Length > 0)
                            {
                                var item = new ItemEntity()
                                {
                                    Name = arr[0],
                                    Price = double.Parse(arr[1])//TODO: add check for type
                                };

                                dbContext.Add(item);
                            }
                        }

                    }
                    dbContext
                        .SaveChanges();
                }
            }
        }

        public void Start()
        {
            //TODO: add DI container
            var builder = new ConfigurationBuilder();
            builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);
            this.configuration = builder.Build();

            lastCheckTime = DateTime.Now;

            var timerInterval = 10000; //adding this to appSettings increases loading time and blocks the service from running
            this.timer = new Timer(timerInterval);

            timer.AutoReset = true;
            timer.Elapsed += this.TimerElapsed;

            timer.Start();
        }
        public void Stop()
        {
            timer.Stop();
        }
    }
}
