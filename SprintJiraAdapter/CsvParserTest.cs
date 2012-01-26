using NUnit.Framework;

namespace SprintJiraAdapter
{
    [TestFixture]
    class csvParserTest
    {
        [Test]
        public void HeaderAndOneLineSuccess()
        {
             var results = CSVParse.ParseCsvFile("BasicTest.txt");

            Assert.That(results.Count, Is.EqualTo(2));
            
            Assert.That(results[0].Count, Is.EqualTo(4));
            Assert.That(results[0][0], Is.EqualTo("Header1"));
            Assert.That(results[0][1], Is.EqualTo("Header2"));


            Assert.That(results[0][2], Is.EqualTo("Header3"));
            Assert.That(results[0][3], Is.EqualTo("Header4"));

            Assert.That(results[1].Count, Is.EqualTo(4));
            Assert.That(results[1][0], Is.EqualTo("1"));
            Assert.That(results[1][1], Is.EqualTo("Data1"));
            Assert.That(results[1][2], Is.EqualTo("Data2"));
            Assert.That(results[1][3], Is.EqualTo("Data3"));
        }

        [Test]
        public void FirstValueEmptyConcatenateSuccess()
        {
            var results = CSVParse.ParseCsvFile("AppendLines.txt");

            Assert.That(results.Count, Is.EqualTo(3));

            Assert.That(results[0].Count, Is.EqualTo(4));
            Assert.That(results[0][0], Is.EqualTo("Header1"));
            Assert.That(results[0][1], Is.EqualTo("Header2"));
            Assert.That(results[0][2], Is.EqualTo("Header3"));
            Assert.That(results[0][3], Is.EqualTo("Header4"));

            Assert.That(results[1].Count, Is.EqualTo(4));
            Assert.That(results[1][0], Is.EqualTo("1"));
            Assert.That(results[1][1], Is.EqualTo("Data1"));
            Assert.That(results[1][2], Is.EqualTo("Data2isD2"));
            Assert.That(results[1][3], Is.EqualTo("Data3"));

            Assert.That(results[2].Count, Is.EqualTo(4));
            Assert.That(results[2][0], Is.EqualTo("2"));
            Assert.That(results[2][1], Is.EqualTo("Meta1"));
            Assert.That(results[2][2], Is.EqualTo("Meta2"));
            Assert.That(results[2][3], Is.EqualTo("Meta3"));        
        }

        [Test]
        public void TestActualFile()
        {
            var results = CSVParse.ParseCsvFile("IssueLog.csv");

            Assert.That(results.Count, Is.EqualTo(4));

            Assert.That(results[0].Count, Is.EqualTo(38));
            Assert.That(results[0][0], Is.EqualTo("Ticket Number "));
            Assert.That(results[0][10], Is.EqualTo("Priority "));
            Assert.That(results[0][15], Is.EqualTo("Test Type "));
            Assert.That(results[0][25], Is.EqualTo("Submitter Location "));
            Assert.That(results[0][35], Is.EqualTo("assigned_id "));

        }
    }
}
