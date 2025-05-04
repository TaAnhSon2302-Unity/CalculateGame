using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class FormulaAnalyzer : MonoBehaviour
{
    public int FindMostOperator(string formula)
    {
        string operatorPattern = @"[\+\-\*\/\!\^]";

        var operatorMatches = Regex.Matches(formula, operatorPattern);
        var operatorsCount = operatorMatches.GroupBy(o => o.Value).ToDictionary(g => g.Key, g => g.Count());
        var mostFrequentOperator = operatorsCount.OrderByDescending(x => x.Value).FirstOrDefault();
        return mostFrequentOperator.Value;
    }
    public int FindMostFrequentDigit(string formula)
    {
        string digitPattern = @"[1-9]";
        var digitMatches = Regex.Matches(formula,digitPattern);
        var digitCount = digitMatches.GroupBy(d=> d.Value).ToDictionary(g=>g.Key,g=>g.Count());
        var FindMostFrequentDigit = digitCount.OrderByDescending(x=>x.Value).FirstOrDefault();
        return FindMostFrequentDigit.Value;
    }
    public double EvaluateFormula(string formula)
    {
        try
        {
            formula = Regex.Replace(formula, @"(\d+)!", match =>
            {
                int num = int.Parse(match.Groups[1].Value);
                return Factorial(num).ToString();
            });
            formula = Regex.Replace(formula, @"(\d+)\^(\d+)", match =>
            {
                double baseNum = double.Parse(match.Groups[1].Value);
                double exponent = double.Parse(match.Groups[2].Value);
                return Math.Pow(baseNum, exponent).ToString();
            });
            DataTable table = new DataTable();
            var result = table.Compute(formula, "");

            return Convert.ToDouble(result);
        }
        catch
        {
            return double.MaxValue;
        }
    }
    public static int Factorial(int n)
    {
        if (n < 0) return 1;
        int result = 1;
        for (int i = 2; i <= n; i++)
        {
            result *= i;
        }
        return result;
    }
}
