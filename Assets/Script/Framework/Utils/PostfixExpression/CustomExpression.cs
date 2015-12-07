using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Framework;

public class CustomExpression : MonoBehaviour
{
    public static object abs(object[] args)
    {
        string[] argNames = new string[] { "input" };
        ProtoExpression.CheckArgCount("abs", args, argNames);
        float f = Convert.ToSingle(args[0]);
        return Mathf.Abs(f);
    }

    public static object ceil(object[] args)
    {
        string[] argNames = new string[] { "input" };
        ProtoExpression.CheckArgCount("ceil", args, argNames);
        float f = (float)args[0];
        return Mathf.Ceil(f);
    }

    private static object concat(object[] args)
    {
        if (args.Length < 2)
        {
            throw new ArgumentException("You must pass at least two values to concat, but they can be any type (and will be converted to strings automatically).");
        }
        return string.Concat(args);
    }

    private static object deviceModelContains(object[] args)
    {
        if (args.Length < 1)
        {
            throw new ArgumentException("deviceModelContains requires at least one modelName string to look for.");
        }
        IEnumerator<string> enumerator = Enumerable.Cast<string>(args).GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                string current = enumerator.Current;
                if (SystemInfo.deviceModel.IndexOf(current, StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    return true;
                }
            }
        }
        finally
        {

        }
        return false;
    }

    public static object exp(object[] args)
    {
        string[] argNames = new string[] { "power" };
        ProtoExpression.CheckArgCount("exp", args, argNames);
        float power = (float)args[0];
        return Mathf.Exp(power);
    }

    public static object floor(object[] args)
    {
        string[] argNames = new string[] { "input" };
        ProtoExpression.CheckArgCount("floor", args, argNames);
        float f = (float)args[0];
        return Mathf.Floor(f);
    }

    public static object If(object[] args)
    {
        string[] argNames = new string[] { "test", "resultIfTrue", "resultIfFalse" };
        ProtoExpression.CheckArgCount("If", args, argNames);
        if (Convert.ToBoolean(args[0]))
        {
            return args[1];
        }
        return args[2];
    }

    private static object isAndroid(object[] args)
    {
        ProtoExpression.CheckArgCount("isAndroid", args, new string[0]);
        return true;
    }

    private static object isIOS(object[] args)
    {
        ProtoExpression.CheckArgCount("isIOS", args, new string[0]);
        return false;
    }


    public static object log(object[] args)
    {
        string[] argNames = new string[] { "f", "p" };
        ProtoExpression.CheckArgCount("log", args, argNames);
        float f = (float)args[0];
        float p = (float)args[1];
        return Mathf.Log(f, p);
    }

    public static object log10(object[] args)
    {
        string[] argNames = new string[] { "f" };
        ProtoExpression.CheckArgCount("log10", args, argNames);
        float f = (float)args[0];
        return Mathf.Log10(f);
    }

    public static object lookup(object[] args)
    {
        if (args.Length < 3)
        {
            throw new Exception("lookup takes 3 or more arguments: index, value[0], value[1], ..., value[n]");
        }
        int index = 1 + ((int)args[0]);
        if (index < 1)
        {
            index = 1;
        }
        else if (index > (args.Length - 1))
        {
            index = args.Length - 1;
        }
        return args[index];
    }

    public static object max(object[] args)
    {
        string[] argNames = new string[] { "a", "b" };
        ProtoExpression.CheckArgCount("max", args, argNames);
        if ((args[0] is int) && (args[1] is int))
        {
            return Math.Max((int)args[0], (int)args[1]);
        }
        if ((args[0] is float) && (args[1] is int))
        {
            return Math.Max((float)args[0], (float)((int)args[1]));
        }
        if ((args[0] is int) && (args[1] is float))
        {
            return Math.Max((float)((int)args[0]), (float)args[1]);
        }
        if (!(args[0] is float) || !(args[1] is float))
        {
            throw new ArgumentException("max only works with int or float arguments.");
        }
        return Math.Max((float)args[0], (float)args[1]);
    }

    public static object min(object[] args)
    {
        string[] argNames = new string[] { "a", "b" };
        ProtoExpression.CheckArgCount("min", args, argNames);
        if ((args[0] is int) && (args[1] is int))
        {
            return Math.Min((int)args[0], (int)args[1]);
        }
        if ((args[0] is float) && (args[1] is int))
        {
            return Math.Min((float)args[0], (float)((int)args[1]));
        }
        if ((args[0] is int) && (args[1] is float))
        {
            return Math.Min((float)((int)args[0]), (float)args[1]);
        }
        if (!(args[0] is float) || !(args[1] is float))
        {
            throw new ArgumentException("min only works with int or float arguments.");
        }
        return Math.Min((float)args[0], (float)args[1]);
    }

    private static object nearestfloor(object[] args)
    {
        int num;
        if (args.Length < 3)
        {
            throw new Exception("nearestfloor takes 3 or more arguments: input, floor0, floor1, ...");
        }
        if (args[0] is float)
        {
            num = (int)((float)args[0]);
        }
        else
        {
            num = (int)args[0];
        }
        int num2 = num;
        int num3 = 0x7fffffff;
        int num4 = (int)args[1];
        for (int i = 1; i < args.Length; i++)
        {
            int num6 = (int)args[i];
            if (num6 < num4)
            {
                num4 = num6;
            }
            int num7 = num - num6;
            if ((num7 >= 0) && (num7 < num3))
            {
                num2 = num6;
                num3 = num7;
            }
        }
        if (num3 == 0x7fffffff)
        {
            return num4;
        }
        return num2;
    }

    public static object pow(object[] args)
    {
        string[] argNames = new string[] { "f", "p" };
        ProtoExpression.CheckArgCount("pow", args, argNames);
        float f = (float)args[0];
        float p = (float)args[1];
        return Mathf.Pow(f, p);
    }

    public static object progressFromBool(object[] args)
    {
        string[] argNames = new string[] { "boolean" };
        ProtoExpression.CheckArgCount("progressFromBool", args, argNames);
        return (!((bool)args[0]) ? 0f : 100f);
    }

    public static object progressFromFloat(object[] args)
    {
        string[] argNames = new string[] { "float", "target" };
        ProtoExpression.CheckArgCount("progressFromFloat", args, argNames);
        float num = (float)Convert.ToDouble(args[0]);
        float num2 = (float)Convert.ToDouble(args[1]);
        float num3 = num / num2;
        if (num2 < (num + 0.001f))
        {
            return 100f;
        }
        return Mathf.Clamp((float)(num3 * 100f), (float)0f, (float)99.9f);
    }

    private static void ProtoExpressionInit()
    {
        ProtoExpression.RegisterFunction("min", null, new ProtoExpressionFunction(CustomExpression.min));
        ProtoExpression.RegisterFunction("max", null, new ProtoExpressionFunction(CustomExpression.max));
        ProtoExpression.RegisterFunction("abs", null, new ProtoExpressionFunction(CustomExpression.abs));
        ProtoExpression.RegisterFunction("floor", null, new ProtoExpressionFunction(CustomExpression.floor));
        ProtoExpression.RegisterFunction("ceil", null, new ProtoExpressionFunction(CustomExpression.ceil));
        ProtoExpression.RegisterFunction("pow", null, new ProtoExpressionFunction(CustomExpression.pow));
        ProtoExpression.RegisterFunction("exp", null, new ProtoExpressionFunction(CustomExpression.exp));
        ProtoExpression.RegisterFunction("log", null, new ProtoExpressionFunction(CustomExpression.log));
        ProtoExpression.RegisterFunction("log10", null, new ProtoExpressionFunction(CustomExpression.log10));
        ProtoExpression.RegisterFunction("round", null, new ProtoExpressionFunction(CustomExpression.round));
        ProtoExpression.RegisterFunction("roundup", null, new ProtoExpressionFunction(CustomExpression.roundup));
        ProtoExpression.RegisterFunction("lookup", "index, value0, value1, ...", new ProtoExpressionFunction(CustomExpression.lookup));
        ProtoExpression.RegisterFunction("nearestfloor", "input, floor0, floor1, ...", new ProtoExpressionFunction(CustomExpression.nearestfloor));
        ProtoExpression.RegisterFunction("concat", "string1, string2, ...", new ProtoExpressionFunction(CustomExpression.concat));
        ProtoExpression.RegisterFunction("if", "test, resultIfTrue, resultIfFalse", new ProtoExpressionFunction(CustomExpression.If));

        ProtoExpression.RegisterFunction("isIOS", null, new ProtoExpressionFunction(CustomExpression.isIOS));
        ProtoExpression.RegisterFunction("isAndroid", null, new ProtoExpressionFunction(CustomExpression.isAndroid));
        ProtoExpression.RegisterFunction("deviceModelContains", "'iPhone3', 'iPhone4', 'etc'", new ProtoExpressionFunction(CustomExpression.deviceModelContains));
    }

    public static object round(object[] args)
    {
        string[] argNames = new string[] { "value", "digits" };
        ProtoExpression.CheckArgCount("round", args, argNames);
        float num = (float)args[0];
        float num2 = (float)args[1];
        return (float)Math.Round((double)num, (int)num2);
    }

    public static object roundup(object[] args)
    {
        string[] argNames = new string[] { "number", "digits" };
        ProtoExpression.CheckArgCount("roundup", args, argNames);
        float num = (float)args[0];
        float num2 = (float)args[1];
        return (float)RoundUp((double)num, (int)num2);
    }

    public static double RoundUp(double num, int place)
    {
        double num2 = num * Math.Pow(10.0, (double)place);
        num2 = Math.Sign(num2) * Math.Abs(Math.Floor((double)(num2 + 0.5)));
        return (num2 / Math.Pow(10.0, (double)place));
    }

    private void Start()
    {
        ProtoExpressionInit();
        UnityEngine.Object.Destroy(this);
    }
}