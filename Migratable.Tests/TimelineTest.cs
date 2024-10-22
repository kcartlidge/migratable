using Microsoft.VisualStudio.TestTools.UnitTesting;
using Migratable.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace Migratable.Tests
{
    [TestClass]
    public class TimelineTest
    {
        private ITimeline timeline;
        private IMigrator migrator;
        private IProvider provider;
        private List<string> expectedLines = new List<string>();

        [TestInitialize]
        public void Setup()
        {
            provider = new SampleProvider();
            migrator = new Migrator(provider);

            var migrations = migrator.LoadMigrations("fixtures");
            timeline = new Timeline(provider, migrations, 2, "Status", "You are here");

            expectedLines = new List<string> {
                "  Status",
                "  001 Create account table",
                "  002 Add default users",
                "  <-- You are here",
                "  003 Remove root user",
                "  004 Do something else",
                "  005 One final thing",
            };
        }

        [TestMethod]
        public void Show_WritesExpectedText()
        {
            // This writes to Console so we capture the output for testing.
            var writer = new StringWriter();
            Console.SetOut(writer);

            migrator.SetVersion(3);
            timeline.Show();

            var stdout = writer.ToString();
            var expected = string.Join(Environment.NewLine, expectedLines);
            expected = Environment.NewLine + expected + Environment.NewLine + Environment.NewLine;
            Assert.AreEqual(expected, stdout);
        }

        [TestMethod]
        public void Get_ReturnsExpectedText()
        {
            migrator.SetVersion(3);
            var result = timeline.Get();

            var expected = string.Join("\n", expectedLines);
            var actual = string.Join("\n", result);
            Assert.AreEqual(expected, actual);
        }
    }
}
