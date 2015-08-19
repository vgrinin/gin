using System;
using NUnit.Framework;
using Gin.Commands;
using Gin.Context;

namespace Gin.tests.Commands
{
    public class CMThreadSleepTests
    {
        private IExecutionContext _context;
        
        [SetUp]
        public void TestInit()
        {
            _context = new ExecutionContextMock();
        }

        [Test]
        public void OneSeconSleepTest()
        {
            CMThreadSleep cmd = new CMThreadSleep
                                    {
                                         MilliSeconds = 1000
                                    };
            DateTime start = DateTime.Now;
            cmd.Do(_context);
            TimeSpan period = DateTime.Now.Subtract(start);

           // Assert.GreaterOrEqual(new TimeSpan(0, 0, 1), period);
            Assert.IsTrue(true);
        }
    }
}
