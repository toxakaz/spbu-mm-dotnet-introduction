using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    public class PathGetter
    {
        private static readonly char[] separator = ['[', ']', ','];
        private static readonly Exception InvalidPath = new("invalid path");

        public static Func<T, U> GetPath<T, U>(string path)
        {
            string[] steps = path.Split('.');
            var obj = Expression.Parameter(typeof(T), "obj");
            Expression expressionTree = obj;
            Type currType = typeof(T);
            foreach (var s in steps[..^0])
            {
                string[] step = s.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (step.Length > 1)
                {
                    var currField = currType.GetProperty(step[0]) ?? throw InvalidPath;
                    currType = currField.PropertyType;
                    expressionTree = Expression.Property(expressionTree, step[0]);

                    currType = currType.GetElementType() ?? throw InvalidPath;
                    var indexes = step[1..].Select(x => Expression.Constant(int.Parse(x))).ToList();
                    expressionTree = Expression.ArrayIndex(expressionTree, indexes);
                }
                else
                    throw InvalidPath;
            }
            expressionTree = Expression.Convert(expressionTree, typeof(U));
            return (Func<T, U>)Expression.Lambda(typeof(Func<T, U>), expressionTree, obj).Compile();
        }
    }
}