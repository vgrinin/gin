using System;
using Gin.Context;
using Gin.Logging;
using NUnit.Framework;
using Gin.Commands;

namespace Gin.tests.Commands
{
    [TestFixture]
    public class BaseCommandTests
    {
        private ExecutionContextMock _context;
        private EmptyCommand _testCommand;
        private ExecutionLoggerMock _logger;

        [SetUp]
        public void TestInit()
        {
            _context = new ExecutionContextMock();
            _testCommand = new EmptyCommand();
            _testCommand.UserInfo = new UserInfoEmbedded
            {
                MessageGuid = "guid",
                MessageText = "text"
            };
            
            Logging.Logging log = new Logging.Logging();
            _logger = new ExecutionLoggerMock();
            log.AddLogger(_logger);
            _context.Log = log;
        }

        [Test]
        public void CheckPendingCancelTest()
        {
            _context.Log.SetPendingCancel();
            try
            {
                _testCommand.Execute(_context);
                Assert.IsFalse(true);
            }
            catch(Exception ex)
            {
                Assert.IsInstanceOf(typeof(PackageExecutionCancelledException), ex);
            }
        }

        [Test]
        public void UserInfosNormalRunningTest()
        {
            _testCommand.Execute(_context);

            TestUserInfoAssertion(0, "guid", "text", UserInfoState.Running);
            TestUserInfoAssertion(1, "guid", "text", UserInfoState.Success);
        }

        [Test]
        public void ResultTest()
        {
            CommandResult result = _testCommand.Execute(_context);

            Assert.AreEqual(CommandResult.Next, result);
        }

        [Test]
        public void CheckPendingCancelUserInfoTest()
        {
            _context.Log.SetPendingCancel();
            try
            {
                _testCommand.Execute(_context);
            }
            catch
            {
            }

            TestUserInfoAssertion("guid", "text", UserInfoState.Fault);
        }

        [Test]
        public void ErrorLevelStopExecutionTest()
        {
            _testCommand.ThrowException = new TestException();
            _testCommand.ErrorLevel = CommandErrorLevel.StopExecution;
            try
            {
                _testCommand.Execute(_context);
                Assert.IsFalse(true);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf(typeof (TestException), ex);
            }
        }

        [Test]
        public void ErrorLevelStopExecutionUserInfoTest()
        {
            _testCommand.ThrowException = new TestException();
            _testCommand.ErrorLevel = CommandErrorLevel.StopExecution;
            try
            {
                _testCommand.Execute(_context);
            }
            catch
            {
            }

            Assert.AreEqual(2, _logger.Events.Count);
            TestUserInfoAssertion(0, "guid", "text", UserInfoState.Running);
            TestUserInfoAssertion(1, "guid", "text", UserInfoState.Fault);
        }

        [Test]
        public void ErrorLevelSkipErrorTest()
        {
            _testCommand.ThrowException = new TestException();
            _testCommand.ErrorLevel = CommandErrorLevel.SkipError;
            _testCommand.Execute(_context);

            Assert.AreEqual(2, _logger.Events.Count);
            TestUserInfoAssertion(0, "guid", "text", UserInfoState.Running);
            TestUserInfoAssertion(1, "guid", "text", UserInfoState.Dismiss);
        }

        [Test]
        public void ErrorLevelAppendLogTest()
        {
            _testCommand.ThrowException = new TestException();
            _testCommand.ErrorLevel = CommandErrorLevel.AppendLog;
            _testCommand.Execute(_context);

            Assert.AreEqual(3, _logger.Events.Count);
            TestUserInfoAssertion(0, "guid", "text", UserInfoState.Running);
            Assert.AreEqual(ExecutionLoggerEventType.Exception, _logger.Events[1].Type);
            TestUserInfoAssertion(2, "guid", "text", UserInfoState.Dismiss);
        }

        [Test]
        public void ErrorLevelAskUserWithSkipTest()
        {
            _testCommand.ThrowException = new TestException();
            _testCommand.ErrorLevel = CommandErrorLevel.AskUser;
            _context.SkipError = true;
            _testCommand.Execute(_context);

            Assert.AreEqual(2, _logger.Events.Count);
            TestUserInfoAssertion(0, "guid", "text", UserInfoState.Running);
            TestUserInfoAssertion(1, "guid", "text", UserInfoState.Dismiss);
        }

        [Test]
        public void ErrorLevelAskUserNoSkipTest()
        {
            _testCommand.ThrowException = new TestException();
            _testCommand.ErrorLevel = CommandErrorLevel.AskUser;
            _context.SkipError = false;
            try
            {
                _testCommand.Execute(_context);
                Assert.IsFalse(true);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf(typeof (TestException), ex);
            }
        }

        [Test]
        public void ErrorLevelAskUserNoSkipUserInfosTest()
        {
            _testCommand.ThrowException = new TestException();
            _testCommand.ErrorLevel = CommandErrorLevel.AskUser;
            _context.SkipError = false;
            try
            {
                _testCommand.Execute(_context);
            }
            catch
            {
            }

            Assert.AreEqual(2, _logger.Events.Count);
            TestUserInfoAssertion(0, "guid", "text", UserInfoState.Running);
            TestUserInfoAssertion(1, "guid", "text", UserInfoState.Fault);
        }

        [Test]
        public void VisitTest()
        {
            TestVisitor visitor = new TestVisitor();
            _testCommand.Visit(visitor);

            Assert.IsInstanceOf(typeof(EmptyCommand), visitor.VisitedCommand);
        }

        private void TestUserInfoAssertion(string guid, string message, UserInfoState state)
        {
            TestUserInfoAssertion(0, guid, message, state);

        }

        private void TestUserInfoAssertion(int number, string guid, string message, UserInfoState state)
        {
            LoggedEvent evt = _logger.Events[number];
            Assert.AreEqual(ExecutionLoggerEventType.UserInfo, evt.Type);
            Assert.IsInstanceOf(typeof(UserInfoData), evt.Data);
            UserInfoData data = (UserInfoData)evt.Data;
            Assert.AreEqual(guid, data.Guid);
            Assert.AreEqual(message, data.Message);
            Assert.AreEqual(state, data.State);

        }
    }
}
