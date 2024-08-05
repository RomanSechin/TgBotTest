using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgBotTest
{
    internal abstract class Expression: IExpression, ICalculate
    {
        protected string _expression;
        public Expression(string expression) { _expression = expression; }
        public abstract bool IsExpression();
        public abstract decimal Calculate();
    }
}
