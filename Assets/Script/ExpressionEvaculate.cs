using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using UnityEngine;

public class ExpressionEvaculate : MonoBehaviour
{
    public double? EvaluateExpression(string expression)
    {
        try
        {
            expression = expression.Replace(" ", ""); // Loại bỏ khoảng trắng thừa
            expression = Regex.Replace(expression, @"(\d+)!", match =>
                       {
                           int num = int.Parse(match.Groups[1].Value);
                           return Factorial(num).ToString();
                       });

            expression = Regex.Replace(expression, @"(\d+)\^(\d+)", match =>
            {
                double baseNum = double.Parse(match.Groups[1].Value);
                double exponent = double.Parse(match.Groups[2].Value);
                return Math.Pow(baseNum, exponent).ToString();
            });
            return (double)new DataTable().Compute(expression, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi tính toán biểu thức: " + expression + "\n" + ex.Message);
            return null;
        }
    }

    private int Factorial(int n)
    {
        if (n == 0 || n == 1) return 1;
        int result = 1;
        for (int i = 2; i <= n; i++)
        {
            result *= i;
        }
        return result;
    }
}
