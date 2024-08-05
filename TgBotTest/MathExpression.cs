using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TgBotTest
{
    internal class MathExpression : Expression
    {
        public string[] operators = { "+", "-", "*", "/" };
        
        //Для проверки выражения на математичность
        Regex regex = new Regex(@"([0-9]|\+|\-|\*|\/|\[|\]})*");
        //Для поиска выражений в скобках, не содержащих другие скобки
        Regex brackets = new Regex(@"\([^\(\)]*\)");
        //В родительском классе есть protected string _expression = "";
        private MathExpression? _leftOperand;
        private MathExpression? _rightOperand;
        //Обобщённый оператор, содержащий операцию, левый и правый операнд
        private Operator? _operator;
        //Принимает значение, если выражение является (становится) числом
        private decimal _value;
        //Если выражение посчитано и является числом, то true
        private bool _isValue = false;
        //private string _expression;
        
        //флаг, говорящий, что 
        //private bool _isValueInBrackets = false;
        public MathExpression(string expression) : base(expression) {
            //Удаляем пробелы из выражения
            _expression = _expression.Replace(" ", "");
        }        
        /// <summary>
        /// Парсим и вычисляем выражения, не содержащие скобок
        /// </summary>
        /// <returns>decimal</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private decimal ParseExpression()
        {
            //Если выражение является значением или значением в квадратных скобках - возвращаем его.
            if (_isValue || IsValueInRectBrackets()) { return _value; }
            //Идём по всем операторам и ищем его позицию в expression
            foreach (var op in operators) {
                int operatorPosition = -1;
                //Для наглядности не объединял два следующих оператора
                if (_expression.Contains(op)) 
                {
                    operatorPosition = _expression.IndexOf(op);
                    //Особый случай, когда выражение в скобках отрицательное
                    //Не учитываем минус после квадратной скобки, поскольку
                    //Разделить выражение на правый и левый операнд по знаку минус
                    //стоящему после квадратной скобки будет ошибкой
                    //поэтому просто ищем вхождение следующего в списке оператора
                    if (_expression[operatorPosition - 1] == '[') continue;
                    _leftOperand = new MathExpression(_expression.Substring(0, operatorPosition));
                    _rightOperand = new MathExpression(_expression.Substring(operatorPosition + 1));
                    
                    switch (op)
                    {
                        case "+": _operator = new AddOperation(_leftOperand, _rightOperand); break;
                        case "-": _operator = new SubOperation(_leftOperand, _rightOperand); break;
                        case "*": _operator = new MulOperation(_leftOperand, _rightOperand); break;
                        case "/": _operator = new DivOperation(_leftOperand, _rightOperand); break;
                        default: _operator = null; break;
                    }
                    return _operator.Calculate();
                }
            }
            throw new InvalidOperationException();
        }
        /// <summary>
        /// Проверка выражения на математичность
        /// </summary>
        /// <returns></returns>
        public bool IsValidExpression() 
        {            
            return regex.IsMatch(_expression);
        }
        /// <summary>
        /// Проверка, содержит ли выражение матем. знаки (не число)
        /// </summary>
        /// <returns>bool</returns>
        public override bool IsExpression()
        {
            string[] operators = { "+", "-", "*", "/" };
            foreach (string @operator in operators)
                if (_expression.Contains(@operator))
                {
                    return true;
                }
            return false;
        }

        /// <summary>
        /// Проверка, что выражение является значением в квадратных скобках, которые используются
        /// для оборачивания отрицательных значений, полученных после вычисления скобок.
        /// Цель - не учитывать знак минус при разборе выражения, стоящий после "["
        /// </summary>
        /// <returns></returns>
        private bool IsValueInRectBrackets() {
            if (_expression[0] == '['
                && _expression[_expression.Length - 1] == ']'
                && Decimal.TryParse(_expression.Substring(1, _expression.Length - 2), out _value)) 
            {
                _isValue = true; 
                return true;
            }
            return false;
        }
        /// <summary>
        /// Вычисляем вырежение, содержащее скобки
        /// </summary>
        /// <returns>decimal</returns>
        public override decimal Calculate() {            

            //Проверяем, является ли выражение числом
            //Отображаем это в поле bool _isValue
            //Сразу присваиваем значение _value
            if (Decimal.TryParse(_expression, out _value))
            {
                _isValue = true;
                return _value;
            }
            //Иначе проверяем, содержит ли выражение скобки            
            //Если да - считаем 
            else if (brackets.Match(_expression).Success)
            {
                //Ищем пары скобок, не содержащие другие скобки, вычисляем выражение в них
                //и заменяем выражение со скобками вычисленным значением
                while (brackets.Match(_expression).Success)
                {
                    //Ищем подходящее выражение в скобках, не содержащее в себе других скобок
                    Match target = brackets.Match(_expression);
                    //Берём это выражение и удаляем скобки
                    string bracketString = target.Value;
                    string bracketCleanedString = bracketString.Replace("(", "");
                    bracketCleanedString = bracketCleanedString.Replace(")", "");
                    //Создаём новый класс и передаём в него наше выражение. Вычисляем.
                    decimal calculated = new MathExpression(bracketCleanedString).Calculate();
                    _value = calculated;
                    if (calculated < 0)
                        //отрицательные числа окружаем квадратными скобками и вставляем
                        //в строку _expression вместо скобок
                        _expression = _expression.Replace(bracketString, "[" + calculated.ToString() + "]");
                    else
                        //положительные значения вставляем вместо вычисленных скобок
                        _expression = _expression.Replace(bracketString, calculated.ToString());
                }
                //Если оператор == null, то вычислять нечего, и нужно вернуть число
                if (_operator == null)
                {
                    return _value;
                }
                else if (IsValueInRectBrackets())
                {
                    return _value;
                }
                //Иначе нужно ещё считать
                return _operator.Calculate();
            }//Если скобок нет - парсим и вычисляем как простое безскобочное выражение
            else
            {
                _value = ParseExpression();
            }
            return _value;           
        }
    } 
}
