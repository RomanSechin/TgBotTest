using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TgBotTest
{
    internal abstract class Operator : ICalculate
    {
        public MathExpression _left;
        public MathExpression _right;
        public Operator( MathExpression left, MathExpression right)
        {
            _left  = left;
            _right = right;
        }
        public abstract decimal Calculate();
    }
}

