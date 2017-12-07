using System.Collections.Generic;

public static class ExpressionSolver {
    private static ExpressionParser parser = new ExpressionParser();
	public static float CalculateFloat(string evalString, List<float> parameters)
    {
        string eval = evalString;
        if (parameters!=null)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                eval = eval.Replace("[p" + i + "]", "(" + parameters[i] + ")");
            }
        }
        eval = ReplaceRandom(eval);
        return (float)parser.EvaluateExpression(eval).Value; 
    }
	public static bool CalculateBool(string evalString, List<float> parameters)
    {
        if (evalString == "")
        {
            return true;
        }
        string eval = evalString;
        if (parameters != null)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                eval = eval.Replace("[p" + i + "]", "(" + parameters[i] + ")");
            }
        }
        eval = ReplaceRandom(eval);
        return BoolExpression.BoolValue(eval, new Dictionary<string, float>());
    }
    private static string ReplaceRandom(string valueEx)
    {
        string result = valueEx;
        if (result.Contains("?"))
        {
            int pos = valueEx.IndexOf("?");
            if (pos < 0)
            {
                result = valueEx;
            }
            result = valueEx.Substring(0, pos) + UnityEngine.Random.Range(0, 100) + valueEx.Substring(pos + 1);
        }
        if (result.Contains("?"))
        {
            return ReplaceRandom(result);
        }
        return result;
    }
}
