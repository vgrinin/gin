using Gin.Attributes;
using NUnit.Framework;
using  Gin.Context;
using Gin.Commands;

namespace Gin.tests.Commands
{

    internal class PropertiesResult
    {
        [GinResult(Result = typeof (string), Kind = CommandResultKind.Primitive, Description = "Аргумент 1")]
        public string Argument1 { get; set; }

        public string Argument2 { get; set; }
    }

    internal class FieldsResult
    {
        [GinResult(Result = typeof (string), Kind = CommandResultKind.Primitive, Description = "Аргумент 1")] 
        public string Argument1;

        public string Argument2;
    }

    internal class NonParsedResult
    {
        public string Argument1;
        public static string Argument2;
    }

    [TestFixture]
    public class CMParseResultTests
    {
        private IExecutionContext _context;

        [SetUp]
        public void TestInit()
        {
            _context = new ExecutionContextMock();
        }

        [Test]
        public void ParseObjectPropertiesTest()
        {
            CMParseResult cmd =
                new CMParseResult
                    {
                        ArgumentName = "inputValue"
                    };
            _context.SaveResult("inputValue",
                                new PropertiesResult
                                    {
                                        Argument1 = "arg1",
                                        Argument2 = "arg2"
                                    });
            cmd.Do(_context);

            Assert.AreEqual("arg1", _context.GetResult("inputValue.Argument1"));
            Assert.AreEqual("arg2", _context.GetResult("inputValue.Argument2"));
        }

        [Test]
        public void ParseObjectFieldsTest()
        {
            CMParseResult cmd =
                new CMParseResult
                {
                    ArgumentName = "inputValue"
                };
            _context.SaveResult("inputValue",
                                new FieldsResult
                                {
                                    Argument1 = "arg1",
                                    Argument2 = "arg2"
                                });
            cmd.Do(_context);

            Assert.AreEqual("arg1", _context.GetResult("inputValue.Argument1"));
            Assert.AreEqual("arg2", _context.GetResult("inputValue.Argument2"));
        }

        [Test]
        public void ParseObjectNonParsedTest()
        {
            CMParseResult cmd =
                new CMParseResult
                {
                    ArgumentName = "inputValue"
                };
            NonParsedResult.Argument2 = "arg2";
            NonParsedResult inp = new NonParsedResult
                                      {
                                          Argument1 = "arg1"
                                      };
            _context.SaveResult("inputValue", inp);
            cmd.Do(_context);

            Assert.AreEqual("arg1", _context.GetResult("inputValue.Argument1"));
            Assert.AreEqual(null, _context.GetResult("inputValue.Argument2"));
        }

        [Test]
        public void ParseObjectCleanPercentsTest()
        {
            CMParseResult cmd =
                new CMParseResult
                {
                    ArgumentName = "%inputValue%"
                };
            _context.SaveResult("inputValue",
                                new FieldsResult
                                {
                                    Argument1 = "arg1",
                                    Argument2 = "arg2"
                                });
            cmd.Do(_context);

            Assert.AreEqual("arg1", _context.GetResult("inputValue.Argument1"));
            Assert.AreEqual("arg2", _context.GetResult("inputValue.Argument2"));
        }

    }
}
