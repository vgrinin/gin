using Gin.SQL.Util;
using NUnit.Framework;

namespace Gin.tests
{
    [TestFixture]
    public class SQLUtilTests
    {
        [Test]
        public void TestGetConnectionStringPart()
        {
            const string connString =
                "Data Source=SDB14;Initial Catalog=TEST_2008;Persist Security Info=True;User ID=sa;Password=osbb";

            string result = SQLUtil.GetConnectionStringPart(connString, "Initial Catalog");

            Assert.AreEqual("TEST_2008", result);
        }

        [Test]
        public void TestGetConnectionStringParts()
        {
            const string connString =
                "Data Source=SDB14;Initial Catalog=TEST_2008;Persist Security Info=True;User ID=sa;Password=osbb";

            var result = SQLUtil.GetConnectionStringParts(connString);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 5);
            Assert.IsTrue(result.ContainsKey("Persist Security Info"));
            Assert.AreEqual(result["Persist Security Info"], "True");
        }

    }
}
