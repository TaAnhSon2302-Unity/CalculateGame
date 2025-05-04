using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class FormulaProcessor : MonoBehaviour
{
    private System.Random random = new System.Random();
    public string ProcessFormula(string formula)
    {
        // Regex để tìm các phép chia trong biểu thức
        string pattern = @"(\d{1,2})/(\d{1,2})";
        return Regex.Replace(formula, pattern, match => ProcessDivision(match.Groups[1].Value, match.Groups[2].Value));
    }
    public string ProcessDivision(string left, string right)
    {
        int leftNum = int.Parse(left);
        int rightNum = int.Parse(right);

        if (left.Length == 1 && right.Length == 1)
        {
            return ProcessXDivY(leftNum);
        }
        else if (left.Length == 2 && right.Length == 1)
        {
            return ProcessXYDivZ(leftNum, rightNum);
        }
        else if (left.Length == 1 && right.Length == 2)
        {
            return ProcessZDivXY(leftNum, rightNum);
        }
        else if (left.Length == 2 && right.Length == 2)
        {
            return ProcessXYDivZW(leftNum, rightNum);
        }

        return $"{left}/{right}";
    }
    private string ProcessXDivY(int x)
    {
        List<int> divisors = GetDivisors(x);
        int y = divisors[random.Next(divisors.Count)];
        return $"{x}/{y}";
    }
    private string ProcessXYDivZ(int xy, int z)
    {
        List<int> multiples = GetMultiples(z);
        int newXY = SelectValidTwoDigit(multiples);
        (int x, int y) = SplitTwoDigit(newXY);
        return $"{x}{y}/{z}";
    }
    private string ProcessZDivXY(int z, int xy)
    {
        return ProcessXYDivZ(xy, z);
    }
    private string ProcessXYDivZW(int xy, int zw)
    {
        List<int> multiples = GetMultiples(zw);
        int newXY = SelectValidTwoDigit(multiples);
        (int x, int y) = SplitTwoDigit(newXY);
        return $"{x}{y}/{zw}";
    }
    private List<int> GetDivisors(int number)
    {
        List<int> divisors = new List<int>();
        {
            for (int i = 1; i <= 9; i++)
            {
                if (number % i == 0)
                {
                    divisors.Add(i);
                }
            }
            return divisors;
        }
    }
    private List<int> GetMultiples(int number)
    {
        List<int> multiples = new List<int>();
        for (int i = 1; i <= 12; i++)
        {
            multiples.Add(number * i);
        }
        return multiples;
    }
    private int SelectValidTwoDigit(List<int> numbers)
    {
        List<int> valid = numbers.FindAll(n => n >= 10 && n <= 99 && !n.ToString().Contains('0'));
        return valid.Count > 0 ? valid[random.Next(valid.Count)] : 1;
    }
    private (int, int) SplitTwoDigit(int number)
    {
        int x = number / 10;
        int y = number % 10;
        return (x, y);
    }
}
public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list, System.Random rng)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]); // Swap vị trí
        }
    }
}
