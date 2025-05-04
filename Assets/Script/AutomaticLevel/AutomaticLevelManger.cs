using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
namespace AutomaticLevelManager
{
    public class AutomaticLevelManger : MonoBehaviour
    {
        public FormulaAnalyzer formulaAnalyzer;
        public FormulaProcessor formulaProcessor;
        public int numberAllowence;
        public int operatorAllowence;
        double result;
        void Awake()
        {
            formulaAnalyzer = gameObject.GetComponent<FormulaAnalyzer>();
            formulaProcessor = gameObject.GetComponent<FormulaProcessor>();
        }
        public string AutomaticGenerateLevel(DifficultyConfig difficultyConfig)
        {
            StringBuilder formula = new StringBuilder();

            int length = UnityEngine.Random.Range(difficultyConfig.minLength, difficultyConfig.maxLength);
            Debug.Log(length);
            List<string> numbers = difficultyConfig.number;
            List<string> operators = difficultyConfig.operation;

            int numberCount = 0;
            bool lastWasOperator = false;
            bool lastWasFactorial = false;

            for (int i = 0; i < length; i++)
            {
                if (i == length - 1)
                {
                    AppendLastCharacter(formula, numbers, ref lastWasOperator, difficultyConfig);
                }
                else if (i == length - 2)
                {
                    AppendSecondLastCharacter(formula, ref numberCount, ref lastWasOperator, ref lastWasFactorial, numbers, operators);
                }
                else if (numberCount == 2)
                {
                    AppendFactorialOrOperator(formula, ref lastWasOperator, ref lastWasFactorial, operators, difficultyConfig);
                    numberCount = 0;
                }
                else if (lastWasOperator || lastWasFactorial)
                {
                    AppendAfterOperatorOrFactorial(formula, ref numberCount, ref lastWasOperator, ref lastWasFactorial, numbers, operators);
                }
                else
                {
                    if (i == 0)
                    {
                        formula.Append(RandomChoice(numbers));
                        numberCount++;
                    }
                    else
                    {
                        AppendNumberOrOperator(formula, ref numberCount, ref lastWasOperator, ref lastWasFactorial, numbers, operators, difficultyConfig);
                    }
                }
            }
            EnsureNoExponentAtPosition(formula, operators, length);
            string afterSolving = formulaProcessor.ProcessFormula(formula.ToString());
            result = formulaAnalyzer.EvaluateFormula(afterSolving.ToString());
            operatorAllowence = formulaAnalyzer.FindMostOperator(formula.ToString());
            numberAllowence = formulaAnalyzer.FindMostFrequentDigit(formula.ToString());
            SetTimeAllowToPress(formula.ToString());
            return afterSolving;
        }

        private void AppendLastCharacter(StringBuilder formula, List<string> numbers, ref bool lastWasOperator, DifficultyConfig difficultyConfig)
        {
            if (!lastWasOperator && (int)difficultyConfig.diffculty == 3 && UnityEngine.Random.value < 0.5f)
            {
                formula.Append("!");
            }
            else
            {
                formula.Append(RandomChoice(numbers));
            }
        }

        private void AppendNumberOrOperator(StringBuilder formula, ref int numberCount, ref bool lastWasOperator, ref bool lastWasFactorial, List<string> numbers, List<string> operators, DifficultyConfig difficultyConfig)
        {
            int length = formula.Length;
            if (length >= 2 && formula[length - 2] == '^' && char.IsDigit(formula[length - 1]))
            {
                string newOperator = RandomChoice(operators.FindAll(op => op != "^" && op != "/" && op != "*"));
                formula.Append(newOperator);
                lastWasOperator = true;
                return;
            }
            if (length >= 2 && formula[length - 2] == '*' && char.IsDigit(formula[length - 1]))
            {
                string newOperator = RandomChoice(operators.FindAll(op => op != "^"));
                formula.Append(newOperator);
                lastWasOperator = true;
                return;
            }
            if (length >= 2 && formula[length - 2] == '/' && char.IsDigit(formula[length - 1]))
            {
                string newOperator = RandomChoice(operators.FindAll(op => op != "^" && op != "/"));
                formula.Append(newOperator);
                lastWasOperator = true;
                return;
            }
            if (length >= 2 && formula[length - 2] == '!' && char.IsDigit(formula[length - 1]))
            {
                string newOperator = RandomChoice(operators.FindAll(op => op != "/"));
                formula.Append(newOperator);
                lastWasOperator = true;
                return;
            }
            if (UnityEngine.Random.value < 0.5f)
            {
                formula.Append(RandomChoice(numbers));
                numberCount++;
                lastWasOperator = false;
            }
            else
            {
                if ((int)difficultyConfig.diffculty >= 3 && CanAddFactorial(formula) && UnityEngine.Random.value <= 0.7f)
                {
                    formula.Append("!");
                    numberCount = 0;
                    lastWasFactorial = true;
                }
                else
                {
                    if ((int)difficultyConfig.diffculty >= 2 && CanAddMuitlpierAnd(formula) && UnityEngine.Random.value <= 0.7f)
                    {
                        formula.Append("^");
                    }
                    else
                    {
                        formula.Append(RandomChoice(operators.FindAll(op => op != "^")));
                    }
                    numberCount = 0;
                    lastWasOperator = true;
                }
            }
        }

        private void AppendFactorialOrOperator(StringBuilder formula, ref bool lastWasOperator, ref bool lastWasFactorial, List<string> operators, DifficultyConfig difficultyConfig)
        {
            if ((int)difficultyConfig.diffculty == 3 && CanAddFactorial(formula) && UnityEngine.Random.value <= 0.7f)
            {
                formula.Append("!");
                lastWasFactorial = true;
            }
            else if ((int)difficultyConfig.diffculty >= 2 && CanAddMuitlpierAnd(formula) && UnityEngine.Random.value <= 0.5f)
            {
                formula.Append("^");
                lastWasOperator = true;
            }
            else
            {
                formula.Append(RandomChoice(operators.FindAll(op => op != "^" && op != "/")));
                lastWasOperator = true;
            }
        }

        private void AppendSecondLastCharacter(StringBuilder formula, ref int numberCount, ref bool lastWasOperator, ref bool lastWasFactorial, List<string> numbers, List<string> operators)
        {
            if (numberCount < 2 && lastWasOperator)
            {
                formula.Append(RandomChoice(numbers));
                numberCount++;
            }
            else
            {
                formula.Append(RandomChoice(operators.FindAll(op => op != "/" && op != "*" && op != "^")));
                lastWasOperator = true;
                numberCount = 0;
            }
        }
        private void AppendAfterOperatorOrFactorial(StringBuilder formula, ref int numberCount, ref bool lastWasOperator, ref bool lastWasFactorial, List<string> numbers, List<string> operators)
        {
            if (lastWasOperator)
            {
                formula.Append(RandomChoice(numbers));
                numberCount++;
                lastWasOperator = false;
            }
            else if (lastWasFactorial)
            {
                string validOperator = RandomChoice(operators.FindAll(op => op != "^" && op != "/"));
                formula.Append(validOperator);
                numberCount = 0;
                lastWasOperator = true;
                lastWasFactorial = false;
            }
        }

        private static string RandomChoice(List<string> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        private bool CanAddFactorial(StringBuilder formula)
        {
            int length = formula.Length;
            if (length == 0)
                return false;
            char lastChar = formula[length - 1];
            char secondLastChar = length > 1 ? formula[length - 2] : ' ';
            if (char.IsDigit(lastChar) && char.IsDigit(secondLastChar) && formula[length - 2] == '/')
            {
                return false;
            }
            if (char.IsDigit(lastChar) && int.Parse(lastChar.ToString()) < 7)
            {
                return true;
            }
            return false;
        }
        private bool CanAddMuitlpierAnd(StringBuilder formula)
        {
            int length = formula.Length;
            if (length == 0)
                return false;
            char lastChar = formula[length - 1];
            char secondLastChar = length > 1 ? formula[length - 2] : ' ';
            if (char.IsDigit(lastChar) && char.IsDigit(secondLastChar) && formula[length - 2] == '/')
            {
                return false;
            }
            if (!char.IsDigit(secondLastChar) && int.Parse(lastChar.ToString()) < 6)
            {
                return true;
            }
            return false;
        }
        private void EnsureNoExponentAtPosition(StringBuilder formula, List<string> operators, int length)
        {
            if (length < 3) return;

            int targetIndex = length - 3;

            if (formula[targetIndex] == '^')
            {
                List<string> validOperators = operators.Where(op => op != "^").ToList();

                if (validOperators.Count > 0)
                {
                    string newOperator = RandomChoice(validOperators);
                    formula[targetIndex] = newOperator[0];
                }
            }
        }
        private void SetTimeAllowToPress(string formula)
        {
            formulaAnalyzer.FindMostOperator(formula);
            formulaAnalyzer.FindMostFrequentDigit(formula);
        }
    }
}

