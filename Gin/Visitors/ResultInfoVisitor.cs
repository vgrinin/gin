using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Gin.Attributes;
using Gin.Context;


namespace Gin.Commands
{

    public class ResultInfo
    {
        public Type Type;
        public string Description;
        public string Name;

        public override string ToString()
        {
            return Name + " (" + Type.ToString() + ") - " + Description;
        }
    }

    public class ResultInfoVisitor : CommandVisitor
    {

        private List<ResultInfo> _list = new List<ResultInfo>();

        public List<ResultInfo> UserInfos
        {
            get
            {
                return _list;
            }
        }

        public ResultInfoVisitor(Command rootCommand)
        {
            ResultInfoVisitor visitor = new ResultInfoVisitor();
            rootCommand.Visit(visitor);
            _firstRunResults = visitor.UserInfos;
            _findRecursive = true;
        }

        private ResultInfoVisitor()
        {
            _findRecursive = false;
        }

        private bool _findRecursive;
        private List<ResultInfo> _firstRunResults;

        public override void Visit(Command command)
        {
            PropertyInfo[] properties = command.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                GinResultAttribute attr = (GinResultAttribute)property.GetCustomAttributes(typeof(GinResultAttribute), true).FirstOrDefault();
                if (attr != null)
                {
                    if (attr.Result != null)
                    {
                        string name = (string)property.GetValue(command, null);
                        if (!String.IsNullOrEmpty(name))
                        {
                            _list.Add(new ResultInfo()
                            {
                                Type = attr.Result,
                                Name = name,
                                Description = attr.Description
                            });
                        }
                    }
                    else if (_findRecursive && attr.Kind == CommandResultKind.Dynamic)
                    {
                        string name = (string)property.GetValue(command, null);
                        if (name == null)
                        {
                            continue;
                        }
                        ResultInfo find = _firstRunResults.FirstOrDefault(r => ExecutionContext.GetPercentedKey(r.Name) == name);
                        if (find == null)
                        {
                            continue;
                        }
                        List<ParsedResult> members = CMParseResult.GetObjectValueMembers(find.Type, null, name);
                        foreach (var member in members)
                        {
                            if (!String.IsNullOrEmpty(member.Name))
                            {
                                _list.Add(new ResultInfo()
                                {
                                    Type = member.Type,
                                    Name = member.Name,
                                    Description = member.Description
                                });
                            }
                        }
                    }
                }
            }
        }
    }
}
