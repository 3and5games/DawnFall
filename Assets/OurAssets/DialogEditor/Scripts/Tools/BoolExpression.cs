using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using dotMath.Core;
using System.Text.RegularExpressions;
using dotMath;

public static class BoolExpression
{
    public static bool BoolValue(string aExpression, Dictionary<string, float> eParams)
    {
        aExpression = aExpression.Replace("?", ((int)Random.Range(0, 100)).ToString());
        EquationCompiler oCompiler = new EquationCompiler(aExpression);
        oCompiler.Compile();

        if (aExpression.Length > 8)
        {
            foreach (KeyValuePair<string, float> pair in eParams)
            {
                oCompiler.SetVariable(pair.Key, pair.Value);
            }
        }
        foreach (KeyValuePair<string, float> pair in eParams)
        {
            oCompiler.SetVariable(pair.Key, pair.Value);
        }
        return (oCompiler.Calculate() == 1);
    }
}
