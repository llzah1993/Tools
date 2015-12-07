using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class InToPostTransfer : MonoBehaviour {

    ArrayList operatorList = new ArrayList();
    Dictionary<string, int> funcParamrterCountMap = new Dictionary<string, int>();
	// Use this for initialization
	void Start () {
        //operatorList.Add("true");
        //operatorList.Add("false");
        operatorList.Add("!");
        operatorList.Add("+");
        operatorList.Add("-");
        operatorList.Add("*");
        operatorList.Add("/");
        operatorList.Add("%");
        operatorList.Add(">");
        operatorList.Add("<");
        operatorList.Add(">=");
        operatorList.Add("<=");
        operatorList.Add("==");
        operatorList.Add("!=");
        operatorList.Add("&&");
        operatorList.Add("||");

        string infix = "{func1(func2(3.14,true,2), false)>=35}>48";
        string postfix = InToPost(infix);
        Debug.Log("infix:" + infix + "      ;postfix:" + postfix);
        //operatorList.Add("()");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //private int parameterCount = 0;

    public string InToPost(string infixExpression)
    {
        funcParamrterCountMap.Clear();
        string postfixExpression = "";
        int index = 0;
        Stack<string> stack = new Stack<string>();
        int lastIndexOfOperator = 0;
        string operatorCurrent;
        string operatorInStacktop = "";
        int lastIndexOfOperand = 0;
        string operandCurrent = "";
        int lastIndexOfCustomFunction = 0;
        string customFunction = "";
        int parameterCount = 0;
        while (index < infixExpression.Length || stack.Count > 0)
        {
            
            if (index >= infixExpression.Length)        //last something
            {
                postfixExpression += stack.Pop();
            }else if (infixExpression[index] == '{')      //{
            {
                stack.Push("{");
                index++;
                continue;
            }
            else if (infixExpression[index] == '}')
            {
                operatorInStacktop = stack.Pop().ToString();
                while (operatorInStacktop != "{")
                {
                    postfixExpression += operatorInStacktop + ",";
                    operatorInStacktop = stack.Pop().ToString();
                }
                index++;
                continue;
            }
            else if (infixExpression[index] == '(' || infixExpression[index] == ',' || infixExpression[index] == ' ')
            {
                //this.parameterCount = 0;
                index++;
                continue;
            }
            else if (infixExpression[index] == ')')
            {
                customFunction = stack.Pop().ToString();
                parameterCount = funcParamrterCountMap[customFunction];
                //parameterCount = 1;
                //postfixExpression += customFunction + customFunction + ",(),";
                postfixExpression += parameterCount.ToString() + "," + customFunction + ",(),";
                index++;
            }
            else
            {
                lastIndexOfOperator = MatchOperator(infixExpression, index);
                if (lastIndexOfOperator != -1)       //operator
                {
                    operatorCurrent = infixExpression.Substring(index, lastIndexOfOperator - index);
                    if (stack.Count == 0)
                    {
                        stack.Push(operatorCurrent);
                        index = lastIndexOfOperator;
                        continue;
                    }
                    operatorInStacktop = stack.Peek().ToString();
                    if (this.GetOperatorPriority(operatorCurrent) > this.GetOperatorPriority(operatorInStacktop))
                    {
                        stack.Push(operatorCurrent);
                        index = lastIndexOfOperator;
                        continue;
                    }
                    else
                    {
                        postfixExpression += stack.Pop() + ",";
                        stack.Push(operatorCurrent);
                        index = lastIndexOfOperator;
                        continue;
                    }
                }
                else        //operand
                {
                    lastIndexOfOperand = this.GetOperand(infixExpression, index);
                    if (lastIndexOfOperand != -1)
                    {
                        operandCurrent = infixExpression.Substring(index, lastIndexOfOperand - index);
                        postfixExpression += operandCurrent + ",";
                        index = lastIndexOfOperand;
                        continue;
                    }
                    else        //custom function
                    {
                        lastIndexOfCustomFunction = this.GetCustomFunction(infixExpression, index);
                        customFunction = infixExpression.Substring(index, lastIndexOfCustomFunction - index);
                        funcParamrterCountMap.Add(customFunction, this.GetFuncParameterCount(infixExpression, lastIndexOfCustomFunction + 1));
                        stack.Push(customFunction);
                        index = lastIndexOfCustomFunction;
                        continue;
                    }
                }
            }
        }

        return postfixExpression;
    }

    private int MatchOperator(string infixExpression, int beginIndex)
    {
        int lastIndex = beginIndex;
        string str = infixExpression.Substring(beginIndex, lastIndex - beginIndex + 1);
        while (operatorList.Contains(str) && lastIndex < infixExpression.Length)
        {
            lastIndex++;
            if (lastIndex == infixExpression.Length)
            {
                continue;
            }
            str = infixExpression.Substring(beginIndex, lastIndex - beginIndex + 1);
        }
        if (lastIndex == beginIndex)
        {
            lastIndex = -1;
        }
        return lastIndex;
    }

    private int GetOperand(string infixExpression, int beginIndex)
    {
        int lastIndex = beginIndex;
        char ch = infixExpression[lastIndex];
        switch (ch)
        { 
            case 't':
                if (infixExpression.Substring(beginIndex, 4) == "true")
                {
                    lastIndex = beginIndex + 4;
                    //this.parameterCount++;
                }
                break;
            case 'f':
                if (infixExpression.Substring(beginIndex, 5) == "false")
                {
                    lastIndex = beginIndex + 5;
                    //this.parameterCount++;
                }
                break;
            default:
                while ( (char.IsDigit(ch) || ch == '.') && lastIndex < infixExpression.Length )
                {
                    lastIndex++;
                    if (lastIndex == infixExpression.Length)
                    {
                        continue;
                    }
                    ch = infixExpression[lastIndex];
                }
                //this.parameterCount++;
                break;

        }
        if (lastIndex == beginIndex)
        {
            lastIndex = -1;
        }
        return lastIndex;
    }

    private int GetCustomFunction(string infixExpression, int beginIndex)
    {
        int lastIndex = beginIndex;
        char ch = infixExpression[lastIndex];
        while (ch != '(' && lastIndex < infixExpression.Length )
        {
            lastIndex++;
            if (lastIndex == infixExpression.Length)
            {
                continue;
            }
            ch = infixExpression[lastIndex];
        }
        return lastIndex;
    }

    private int GetFuncParameterCount(string infixExpression, int beginIndex)
    {
        int parametrCount = 1;
        int index = beginIndex;
        char ch = infixExpression[index];
        while (ch != ')')
        {
            if (ch == '(')
            { 
                index++;
                ch = infixExpression[index];
                while (ch != ')')
                {
                    index++;
                    ch = infixExpression[index];
                }
                index++;
                ch = infixExpression[index];
            }
            if (ch == ',')
            {
                parametrCount++;
            }
            index++;
            ch = infixExpression[index];
        }
        return parametrCount;
    }

    //private int GetTrueOrFalse(string infixExpression, int beginIndex)
    //{
    //    int lastIndex = beginIndex;
    //    char ch = infixExpression[lastIndex];
    //    return lastIndex;
    //}

    private int GetOperatorPriority(string ope)
    {
        switch (ope)
        { 
            case "||":
                return 1;
            case "&&":
                return 2;
            case "==":
                return 3;
            case "!=":
                return 3;
            case "<=":
                return 4;
            case ">=":
                return 4;
            case "<":
                return 4;
            case ">":
                return 4;
            case "+":
                return 5;
            case "-":
                return 5;
            case "*":
                return 6;
            case "/":
                return 6;
            case "%":
                return 6;
            case "!":
                return 7;
            case "{":
                return 0;
            default:
                return -1;
        }
    }

}
