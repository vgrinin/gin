using NUnit.Framework;


namespace Gin.tests
{
    [TestFixture]
    public class ConnectionStringFilterTests
    {
        [Test]
        public void TestConnectionStringFilter()
        {
            LogMessageFilter filter = new ConnectionStringFilter();
            const string connString = "Data Source=SDB14;Initial Catalog=TEST_2008;Persist Security Info=True;User ID=sa;Password=osbb";
            string result = filter.Filter(connString);
             //
            Assert.AreEqual("Data Source=SDB14;Initial Catalog=TEST_2008;Persist Security Info=True;User ID=sa;Password=**************;", result);
        }
    }
}
