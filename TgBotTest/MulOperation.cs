using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgBotTest
{
    internal class MulOperation : Operator, ICalculate
    {
        public MulOperation(MathExpression left, MathExpression right) : base(left, right) { }
        public override decimal Calculate()
        {
            return _left.Calculate() * _right.Calculate();
        }
    }
}
