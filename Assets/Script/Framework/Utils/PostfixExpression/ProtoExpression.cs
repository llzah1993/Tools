//using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

//[ProtoContract, ProtoSkipOverrideCodeGen]
namespace Framework
{

    public delegate object ProtoExpressionFunction(object[] args);
    public class ProtoExpression
    {
        //[ProtoMember(1)]
        public string[] rpnStream;

        public static Dictionary<string, ProtoExpressionFunction> functionTable = new Dictionary<string, ProtoExpressionFunction>();

        private static Dictionary<string, int> operator_switch_map;

        public static void CheckArgCount(string functionName, object[] args, params string[] argNames)
        {
            if (args.Length != argNames.Length)
            {
                if (argNames.Length == 0)
                {
                    throw new ArgumentException(string.Format("{0} takes 0 arguments.", functionName));
                }
                if (argNames.Length == 1)
                {
                    throw new ArgumentException(string.Format("{0} takes 1 argument: {1}.", functionName, argNames[0]));
                }
                throw new ArgumentException(string.Format("{0} takes {1} arguments: {2}.", functionName, argNames.Length, string.Join(", ", argNames)));
            }
        }

        public static void RegisterFunction(string functionName, string argumentsHint, ProtoExpressionFunction func)
        {
            functionTable[functionName] = func;
        }

        public object RPNEvaluate()
        {
            if ((this.rpnStream == null) || (this.rpnStream.Length == 0))
            {
                return true;
            }
            Stack<object> stack = new Stack<object>();

            #region FOREACH
            foreach (string str in this.rpnStream)
            {
                object operand1;
                object operand2;
                bool flagForLogic;      //for "&&" and "||"
                object[] customOperandArray = new object[1];
                int customOperandIndex = 0;
                string functionNameStr = "";
                ProtoExpressionFunction function;
                string key = str;

                #region IF_KEY
                if (key != null)
                {
                    int switchKey;
                    if (operator_switch_map == null)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>(0x11);
                        dictionary.Add("true", 0);
                        dictionary.Add("false", 1);
                        dictionary.Add("!", 2);
                        dictionary.Add("+", 3);
                        dictionary.Add("-", 4);
                        dictionary.Add("*", 5);
                        dictionary.Add("/", 6);
                        dictionary.Add("%", 7);
                        dictionary.Add(">", 8);
                        dictionary.Add("<", 9);
                        dictionary.Add(">=", 10);
                        dictionary.Add("<=", 11);
                        dictionary.Add("==", 12);
                        dictionary.Add("!=", 13);
                        dictionary.Add("&&", 14);
                        dictionary.Add("||", 15);
                        dictionary.Add("()", 0x10);
                        operator_switch_map = dictionary;
                    }
                    if (operator_switch_map.TryGetValue(key, out switchKey))
                    {
                        switch (switchKey)
                        {
                            case 0:
                                {
                                    stack.Push(true);
                                    continue;
                                }
                            case 1:
                                {
                                    stack.Push(false);
                                    continue;
                                }
                            case 2:
                                operand1 = stack.Pop();
                                if (!(operand1 is bool))
                                {
                                    throw new Exception("operator ! expects boolean operand, but got " + operand1.GetType());
                                }
                                goto Label_01F0;

                            case 3:
                                {
                                    operand1 = stack.Pop();
                                    operand2 = stack.Pop();
                                    if (!(operand1 is double) && !(operand2 is double))
                                    {
                                        goto Label_0259;
                                    }
                                    double num2 = Convert.ToDouble(operand1);
                                    double num3 = Convert.ToDouble(operand2);
                                    stack.Push(num3 + num2);
                                    continue;
                                }
                            case 4:
                                {
                                    operand1 = stack.Pop();
                                    operand2 = stack.Pop();
                                    if (!(operand1 is double) && !(operand2 is double))
                                    {
                                        goto Label_034D;
                                    }
                                    double num10 = Convert.ToDouble(operand1);
                                    double num11 = Convert.ToDouble(operand2);
                                    stack.Push(num11 - num10);
                                    continue;
                                }
                            case 5:
                                {
                                    operand1 = stack.Pop();
                                    operand2 = stack.Pop();
                                    if (!(operand1 is double) && !(operand2 is double))
                                    {
                                        goto Label_0441;
                                    }
                                    double num18 = Convert.ToDouble(operand1);
                                    double num19 = Convert.ToDouble(operand2);
                                    stack.Push(num19 * num18);
                                    continue;
                                }
                            case 6:
                                {
                                    operand1 = stack.Pop();
                                    operand2 = stack.Pop();
                                    if (!(operand1 is double) && !(operand2 is double))
                                    {
                                        goto Label_0535;
                                    }
                                    double num26 = Convert.ToDouble(operand1);
                                    double num27 = Convert.ToDouble(operand2);
                                    stack.Push(num27 / num26);
                                    continue;
                                }
                            case 7:
                                {
                                    operand1 = stack.Pop();
                                    operand2 = stack.Pop();
                                    if (!(operand1 is double) && !(operand2 is double))
                                    {
                                        goto Label_0629;
                                    }
                                    double num34 = Convert.ToDouble(operand1);
                                    double num35 = Convert.ToDouble(operand2);
                                    stack.Push(num35 % num34);
                                    continue;
                                }
                            case 8:
                                {
                                    operand1 = stack.Pop();
                                    operand2 = stack.Pop();
                                    if (!(operand1 is double) && !(operand2 is double))
                                    {
                                        goto Label_071E;
                                    }
                                    double num42 = Convert.ToDouble(operand1);
                                    double num43 = Convert.ToDouble(operand2);
                                    stack.Push(num43 > num42);
                                    continue;
                                }
                            case 9:
                                {
                                    operand1 = stack.Pop();
                                    operand2 = stack.Pop();
                                    if (!(operand1 is double) && !(operand2 is double))
                                    {
                                        goto Label_0816;
                                    }
                                    double num50 = Convert.ToDouble(operand1);
                                    double num51 = Convert.ToDouble(operand2);
                                    stack.Push(num51 < num50);
                                    continue;
                                }
                            case 10:
                                {
                                    operand1 = stack.Pop();
                                    operand2 = stack.Pop();
                                    if (!(operand1 is double) && !(operand2 is double))
                                    {
                                        goto Label_0911;
                                    }
                                    double num58 = Convert.ToDouble(operand1);
                                    double num59 = Convert.ToDouble(operand2);
                                    stack.Push(num59 >= num58);
                                    continue;
                                }
                            case 11:
                                {
                                    operand1 = stack.Pop();
                                    operand2 = stack.Pop();
                                    if (!(operand1 is double) && !(operand2 is double))
                                    {
                                        goto Label_0A15;
                                    }
                                    double num66 = Convert.ToDouble(operand1);
                                    double num67 = Convert.ToDouble(operand2);
                                    stack.Push(num67 <= num66);
                                    continue;
                                }
                            case 12:
                                {
                                    operand1 = stack.Pop();
                                    operand2 = stack.Pop();
                                    if (!(operand1 is bool) && !(operand2 is bool))
                                    {
                                        goto Label_0B16;
                                    }
                                    bool flag1 = (bool)operand1;
                                    bool flag2 = (bool)operand2;
                                    stack.Push(flag2 == flag1);
                                    continue;
                                }
                            case 13:
                                {
                                    operand1 = stack.Pop();
                                    operand2 = stack.Pop();
                                    if (!(operand1 is bool) && !(operand2 is bool))
                                    {
                                        goto Label_0C51;
                                    }
                                    bool flag3 = (bool)operand1;
                                    bool flag4 = (bool)operand2;
                                    stack.Push(flag4 != flag3);
                                    continue;
                                }
                            case 14:
                                operand1 = stack.Pop();
                                operand2 = stack.Pop();
                                if (!(operand1 is bool) || !(operand2 is bool))
                                {
                                    object[] objArray1 = new object[] { "operator && expects boolean operands, but got ", operand2.GetType(), " and ", operand1.GetType() };
                                    throw new Exception(string.Concat(objArray1));
                                }
                                goto Label_0DA2;

                            case 15:
                                operand1 = stack.Pop();
                                operand2 = stack.Pop();
                                if (!(operand1 is bool) || !(operand2 is bool))
                                {
                                    object[] objArray2 = new object[] { "operator || expects boolean operands, but got ", operand2.GetType(), " and ", operand1.GetType() };
                                    throw new Exception(string.Concat(objArray2));
                                }
                                goto Label_0E2D;

                            case 0x10:
                                {
                                    functionNameStr = stack.Pop().ToString();
                                    int customOperandCount = Convert.ToInt32(stack.Pop());
                                    customOperandArray = new object[customOperandCount];
                                    customOperandIndex = customOperandCount - 1;
                                    goto Label_0E9A;
                                }
                        }
                    }
                }

                #endregion IF_KEY
                goto Label_0EDB;
            Label_01F0:
                stack.Push(!((bool)operand1));
                continue;
            Label_0259:
                if ((operand1 is float) || (operand2 is float))
                {
                    float num4 = Convert.ToSingle(operand1);
                    float num5 = Convert.ToSingle(operand2);
                    stack.Push(num5 + num4);
                }
                else if ((operand1 is long) || (operand2 is long))
                {
                    long num6 = Convert.ToInt64(operand1);
                    long num7 = Convert.ToInt64(operand2);
                    stack.Push(num7 + num6);
                }
                else
                {
                    int num8 = Convert.ToInt32(operand1);
                    int num9 = Convert.ToInt32(operand2);
                    stack.Push(num9 + num8);
                }
                continue;
            Label_034D:
                if ((operand1 is float) || (operand2 is float))
                {
                    float num12 = Convert.ToSingle(operand1);
                    float num13 = Convert.ToSingle(operand2);
                    stack.Push(num13 - num12);
                }
                else if ((operand1 is long) || (operand2 is long))
                {
                    long num14 = Convert.ToInt64(operand1);
                    long num15 = Convert.ToInt64(operand2);
                    stack.Push(num15 - num14);
                }
                else
                {
                    int num16 = Convert.ToInt32(operand1);
                    int num17 = Convert.ToInt32(operand2);
                    stack.Push(num17 - num16);
                }
                continue;
            Label_0441:
                if ((operand1 is float) || (operand2 is float))
                {
                    float num20 = Convert.ToSingle(operand1);
                    float num21 = Convert.ToSingle(operand2);
                    stack.Push(num21 * num20);
                }
                else if ((operand1 is long) || (operand2 is long))
                {
                    long num22 = Convert.ToInt64(operand1);
                    long num23 = Convert.ToInt64(operand2);
                    stack.Push(num23 * num22);
                }
                else
                {
                    int num24 = Convert.ToInt32(operand1);
                    int num25 = Convert.ToInt32(operand2);
                    stack.Push(num25 * num24);
                }
                continue;
            Label_0535:
                if ((operand1 is float) || (operand2 is float))
                {
                    float num28 = Convert.ToSingle(operand1);
                    float num29 = Convert.ToSingle(operand2);
                    stack.Push(num29 / num28);
                }
                else if ((operand1 is long) || (operand2 is long))
                {
                    long num30 = Convert.ToInt64(operand1);
                    long num31 = Convert.ToInt64(operand2);
                    stack.Push(num31 / num30);
                }
                else
                {
                    int num32 = Convert.ToInt32(operand1);
                    int num33 = Convert.ToInt32(operand2);
                    stack.Push(num33 / num32);
                }
                continue;
            Label_0629:
                if ((operand1 is float) || (operand2 is float))
                {
                    float num36 = Convert.ToSingle(operand1);
                    float num37 = Convert.ToSingle(operand2);
                    stack.Push(num37 % num36);
                }
                else if ((operand1 is long) || (operand2 is long))
                {
                    long num38 = Convert.ToInt64(operand1);
                    long num39 = Convert.ToInt64(operand2);
                    stack.Push(num39 % num38);
                }
                else
                {
                    int num40 = Convert.ToInt32(operand1);
                    int num41 = Convert.ToInt32(operand2);
                    stack.Push(num41 % num40);
                }
                continue;
            Label_071E:
                if ((operand1 is float) || (operand2 is float))
                {
                    float num44 = Convert.ToSingle(operand1);
                    float num45 = Convert.ToSingle(operand2);
                    stack.Push(num45 > num44);
                }
                else if ((operand1 is long) || (operand2 is long))
                {
                    long num46 = Convert.ToInt64(operand1);
                    long num47 = Convert.ToInt64(operand2);
                    stack.Push(num47 > num46);
                }
                else
                {
                    int num48 = Convert.ToInt32(operand1);
                    int num49 = Convert.ToInt32(operand2);
                    stack.Push(num49 > num48);
                }
                continue;
            Label_0816:
                if ((operand1 is float) || (operand2 is float))
                {
                    float num52 = Convert.ToSingle(operand1);
                    float num53 = Convert.ToSingle(operand2);
                    stack.Push(num53 < num52);
                }
                else if ((operand1 is long) || (operand2 is long))
                {
                    long num54 = Convert.ToInt64(operand1);
                    long num55 = Convert.ToInt64(operand2);
                    stack.Push(num55 < num54);
                }
                else
                {
                    int num56 = Convert.ToInt32(operand1);
                    int num57 = Convert.ToInt32(operand2);
                    stack.Push(num57 < num56);
                }
                continue;
            Label_0911:
                if ((operand1 is float) || (operand2 is float))
                {
                    float num60 = Convert.ToSingle(operand1);
                    float num61 = Convert.ToSingle(operand2);
                    stack.Push(num61 >= num60);
                }
                else if ((operand1 is long) || (operand2 is long))
                {
                    long num62 = Convert.ToInt64(operand1);
                    long num63 = Convert.ToInt64(operand2);
                    stack.Push(num63 >= num62);
                }
                else
                {
                    int num64 = Convert.ToInt32(operand1);
                    int num65 = Convert.ToInt32(operand2);
                    stack.Push(num65 >= num64);
                }
                continue;
            Label_0A15:
                if ((operand1 is float) || (operand2 is float))
                {
                    float num68 = Convert.ToSingle(operand1);
                    float num69 = Convert.ToSingle(operand2);
                    stack.Push(num69 <= num68);
                }
                else if ((operand1 is long) || (operand2 is long))
                {
                    long num70 = Convert.ToInt64(operand1);
                    long num71 = Convert.ToInt64(operand2);
                    stack.Push(num71 <= num70);
                }
                else
                {
                    int num72 = Convert.ToInt32(operand1);
                    int num73 = Convert.ToInt32(operand2);
                    stack.Push(num73 <= num72);
                }
                continue;
            Label_0B16:
                if ((operand1 is double) || (operand2 is double))
                {
                    double num74 = Convert.ToDouble(operand1);
                    double num75 = Convert.ToDouble(operand2);
                    stack.Push(num75 == num74);
                }
                else if ((operand1 is float) || (operand2 is float))
                {
                    float num76 = Convert.ToSingle(operand1);
                    float num77 = Convert.ToSingle(operand2);
                    stack.Push(num77 == num76);
                }
                else if ((operand1 is long) || (operand2 is long))
                {
                    long num78 = Convert.ToInt64(operand1);
                    long num79 = Convert.ToInt64(operand2);
                    stack.Push(num79 == num78);
                }
                else
                {
                    int num80 = Convert.ToInt32(operand1);
                    int num81 = Convert.ToInt32(operand2);
                    stack.Push(num81 == num80);
                }
                continue;
            Label_0C51:
                if ((operand1 is double) || (operand2 is double))
                {
                    double num82 = Convert.ToDouble(operand1);
                    double num83 = Convert.ToDouble(operand2);
                    stack.Push(num83 != num82);
                }
                else if ((operand1 is float) || (operand2 is float))
                {
                    float num84 = Convert.ToSingle(operand1);
                    float num85 = Convert.ToSingle(operand2);
                    stack.Push(num85 != num84);
                }
                else if ((operand1 is long) || (operand2 is long))
                {
                    long num86 = Convert.ToInt64(operand1);
                    long num87 = Convert.ToInt64(operand2);
                    stack.Push(num87 != num86);
                }
                else
                {
                    int num88 = Convert.ToInt32(operand1);
                    int num89 = Convert.ToInt32(operand2);
                    stack.Push(num89 != num88);
                }
                continue;
            Label_0DA2:
                flagForLogic = (bool)operand1;
                bool flag5 = (bool)operand2;
                stack.Push(!flag5 ? ((bool)false) : ((bool)flagForLogic));
                continue;
            Label_0E2D:
                flagForLogic = (bool)operand1;
                bool flag6 = (bool)operand2;
                stack.Push(flag6 ? ((bool)true) : ((bool)flagForLogic));
                continue;
            Label_0E89:
                customOperandArray[customOperandIndex] = stack.Pop();
                customOperandIndex--;
            Label_0E9A:
                if (customOperandIndex >= 0)
                {
                    goto Label_0E89;
                }
                if (!functionTable.TryGetValue(functionNameStr, out function))
                {
                    throw new Exception("Unknown function: " + functionNameStr);
                }
                stack.Push(function(customOperandArray));
                continue;
            Label_0EDB:
                if (str.StartsWith("'"))
                {
                    stack.Push(str.Substring(1));
                }
                else if (Enumerable.Contains<char>(str, '.'))
                {
                    float floatNum;
                    if (float.TryParse(str, out floatNum))
                    {
                        stack.Push(floatNum);
                    }
                    else
                    {
                        stack.Push(double.Parse(str));
                    }
                }
                else if (char.IsDigit(str[0]))
                {
                    int intNum;
                    if (int.TryParse(str, out intNum))
                    {
                        stack.Push(intNum);
                    }
                    else
                    {
                        stack.Push(long.Parse(str));
                    }
                }
                else
                {
                    stack.Push(str);
                }
            }

            #endregion FOREACH

            return stack.Pop();

        }
    }
}