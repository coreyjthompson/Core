using System;
using System.Collections.Generic;
using System.Linq;

using AutoFixture;

using Bogus;

using MEI.Core.DomainModels.Common;
using MEI.Core.Infrastructure.Data;

namespace MEI.Core.Tests.Infrastructure.Helpers
{
    public static class TestDataHelper
    {
        public static void AddLogs(this CoreContext db, int size, string mustIncludeAppName, string mustIncludeEnvironment)
        {
            var fixture = new Fixture();

            var appIndex = 0;
            var apps = fixture.Build<AppWithCorrelation>()
                .Without(y => y.AppName)
                .Without(y => y.CorrelationIds)
                .Do(y =>
                {
                    if (appIndex == 0)
                    {
                        y.AppName = mustIncludeAppName;
                        appIndex++;
                    }
                    else
                    {
                        y.AppName = "App" + appIndex++;
                    }

                    y.CorrelationIds = fixture.CreateMany<Guid>().Select(x => x.ToString()).ToList();
                }).CreateMany(5).ToList();

            var environments = new[] { "Development", "UAT", "Production" }.ToList();
            if (!environments.Contains(mustIncludeEnvironment))
            {
                environments.Add(mustIncludeEnvironment);
            }

            var levels = new[] { "Trace", "Debug", "Info", "Warn" };
            var errorLevels = new[] { "Error", "Fatal" };
            var machineNames = new[] { "WebServer1", "WebServer2", "WebServer3", "UserMachine1", "UserMachine2" };
            var faker = new Faker();
            int halfSize = size / 2;
            int quarterSize = halfSize / 2;
            var logIndex = 0;
            var mustIncludeAppIndex = 0;

            var logs = fixture.Build<Log>()
                .Without(x => x.Id)
                .Without(x => x.AppName)
                .Without(x => x.Logger)
                .Without(x => x.CorrelationId)
                .Without(x => x.Environment)
                .Do(x =>
                {
                    AppWithCorrelation app = logIndex >= halfSize
                        ? apps.First(y => y.AppName == mustIncludeAppName)
                        : faker.PickRandom(apps);

                    x.AppName = app.AppName;
                    x.Logger = app.AppName + "." + faker.Lorem.Word();
                    x.CorrelationId = faker.PickRandom(app.CorrelationIds);

                    if (app.AppName == mustIncludeAppName)
                    {
                        x.Environment = mustIncludeAppIndex >= quarterSize ? faker.PickRandom(environments) : mustIncludeEnvironment;

                        mustIncludeAppIndex++;
                    }
                    else
                    {
                        x.Environment = faker.PickRandom(environments);
                    }

                    logIndex++;
                })
                .With(x => x.WhenLogged, () => faker.Noda().ZonedDateTime.Past().ToDateTimeOffset())
                .Without(x => x.Exception)
                .Without(x => x.Message)
                .Without(x => x.Level)
                .Do(x =>
                {
                    if (faker.Random.Bool())
                    {
                        var ex = faker.System.Exception();
                        x.Exception = ex.ToString();
                        x.Message = ex.Message;
                        x.Level = faker.PickRandom(errorLevels);
                    }
                    else
                    {
                        x.Message = faker.Lorem.Sentence();
                        x.Level = faker.PickRandom(levels);
                    }
                })
                .With(x => x.MachineName, () => faker.PickRandom(machineNames))
                .CreateMany(size).ToList();

            db.Logs.AddRange(logs);

            db.SaveChanges();
        }

        private class AppWithCorrelation
        {
            public string AppName { get; set; }

            public List<string> CorrelationIds { get; set; }
        }
    }
}
