using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CalculateGameSoundManager;
using CalculateUIManager;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

namespace CalculateLevelManager
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager instance { get; private set; }
        [SerializeField] private Transform parentTransform;
        [SerializeField] public TMPrefab textPrefab;
        [SerializeField] public List<string> formulaSegments;
        [SerializeField] public List<TextMeshProUGUI> generatedTexts;
        [SerializeField] public List<TMPrefab> tMPrefabs;
        [SerializeField] public List<int> changedIndex = new List<int>();
        public List<ButtonPrefab> pressedButtons = new List<ButtonPrefab>();
        [SerializeField] public string currentExpression;
        public UIManager uIManager;
        public int indexLevel = 1;
        public int visblaeOperators;
        public string automaticFormula;
        [SerializeField] public DifficultyConfig randomDifficulty;
        public Action<int> OnLevelIndexChange;
        public int levelIndexForHard = 35;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            indexLevel = PlayerPrefs.GetInt("LevelIndex", 1);
        }
        public LevelSO currentLevel;
        public void LoadLevel(int index)
        {
            currentLevel = Resources.Load<LevelSO>($"Levels/Level {index}");
        }
        #region Special Expression
        public string SolvePowerExpression(string expression)
        {
            Regex powerRegex = new Regex(@"(\d+)\s*\^\s*(\d+)");
            while (powerRegex.IsMatch(expression))
            {
                expression = powerRegex.Replace(expression, math =>
                {
                    double baseNume = Convert.ToDouble(math.Groups[1].Value);
                    double exponent = Convert.ToDouble(math.Groups[2].Value);
                    return Math.Pow(baseNume, exponent).ToString();
                });
            }
            return expression;
        }
        public string SolveFactorialExpression(string expression)
        {
            Regex factorialRegex = new Regex(@"(\d+)!");

            while (factorialRegex.IsMatch(expression))
            {
                expression = factorialRegex.Replace(expression, match =>
                {
                    int num = Convert.ToInt32(match.Groups[1].Value);
                    return Factorial(num).ToString();
                });
            }
            return expression;
        }
        #endregion
        public long Factorial(int n)
        {
            if (n == 0 || n == 1) return 1;
            long result = 1;
            for (int i = 2; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }
        public void GenerateTextFromHint(string hintFormula, LevelSO levelSO)
        {
            ResetList();
            int operatorIndex = 0;
            for (int i = 0; i < hintFormula.Length; i++)
            {
                char c = hintFormula[i];
                TMPrefab newTextObject = Instantiate(textPrefab, parentTransform);

                if (char.IsDigit(c))
                {
                    newTextObject.textMesh.text = "<sprite name=\"question\">";
                    newTextObject.StartBounceEffect();
                }
                else if ("+-*/^!".Contains(c))
                {
                    if (levelSO.operationHideIndex.Contains(operatorIndex))
                    {
                        newTextObject.textMesh.text = "<sprite name=\"question\">";
                        newTextObject.StartBounceEffect();
                    }
                    else
                    {
                        newTextObject.textMesh.text = c.ToString();
                        newTextObject.SetColorForText();
                        visblaeOperators++;
                    }
                    operatorIndex++;
                }
                else
                {
                    newTextObject.textMesh.text = c.ToString();
                }
                tMPrefabs.Add(newTextObject);
                formulaSegments.Add(newTextObject.textMesh.text);
                generatedTexts.Add(newTextObject.textMesh);
            }
        }
        public void GenerateTextFromAutomatic(string formula, DifficultyConfig difficultyConfig)
        {
            ResetList();
            for (int i = 0; i < formula.Length; i++)
            {
                char c = formula[i];
                TMPrefab newTextObject = Instantiate(textPrefab, parentTransform);
                if (char.IsDigit(c))
                {
                    newTextObject.textMesh.text = "<sprite name=\"question\">";
                }
                else if ("+-*/^!".Contains(c))
                {
                    newTextObject.textMesh.text = "<sprite name=\"question\">";
                }
                tMPrefabs.Add(newTextObject);
                formulaSegments.Add(newTextObject.textMesh.text);
                generatedTexts.Add(newTextObject.textMesh);
            }
        }
        public void FinalResult()
        {
            currentExpression = string.Join("", formulaSegments);
            if (!string.IsNullOrEmpty(currentExpression))
            {
                try
                {
                    string processedExpression = SolvePowerExpression(SolveFactorialExpression(currentExpression));
                    object result = new System.Data.DataTable().Compute(processedExpression, null);
                    if (double.TryParse(result.ToString(), out double finalResult))
                    {
                        if (finalResult == CalculateRequirementResult(indexLevel <= levelIndexForHard ? currentLevel.hintFormula : automaticFormula))
                        {
                            uIManager.SetActiveWinUI();
                            CalculateSoundManager.instance.PlaySound(CalculateSoundName.WIN, 0.2f);
                        }
                        else
                        {
                            uIManager.SetActiveLoseUI(finalResult.ToString());
                            CalculateSoundManager.instance.PlaySound(CalculateSoundName.LOSE, 0.2f);
                        }
                    }
                    else
                    {
                        CalculateSoundManager.instance.PlaySound(CalculateSoundName.LOSE, 0.2f);
                        uIManager.SetActiveLoseUI(finalResult.ToString());
                    }
                }
                catch (Exception)
                {
                    CalculateSoundManager.instance.PlaySound(CalculateSoundName.LOSE, 0.2f);
                    uIManager.SetActiveLoseUI("No Result");
                }
            }
        }
        public double CalculateRequirementResult(string hintFormula)
        {
            try
            {
                string processedExpression = SolvePowerExpression(SolveFactorialExpression(hintFormula));
                object result = new System.Data.DataTable().Compute(processedExpression, null);


                if (double.TryParse(result.ToString(), out double finalResult))
                {
                    return finalResult;
                }
                else
                {
                    return double.NaN;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Lỗi tính toán: " + ex.Message);
                return double.NaN;
            }
        }
        public double EvaluateExpression(string expression)
        {
            try
            {
                string processedExpression = SolvePowerExpression(SolveFactorialExpression(expression));
                object result = new System.Data.DataTable().Compute(processedExpression, null);
                return Convert.ToDouble(result);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Lỗi khi tính toán biểu thức: {ex.Message}");
            }
            return 0;
        }
        public string BuildExpression(List<string> segments)
        {
            StringBuilder expression = new StringBuilder();
            bool lastWasBlank = false;
            for (int i = 0; i < segments.Count; i++)
            {
                string current = segments[i];

                if (current == "<sprite name=\"question\">")
                {
                    lastWasBlank = true;
                    continue;

                }
                if (current == "!" && i > 0 && segments[i - 1] != "<sprite name=\"question\">")
                {
                    expression.Append(current);
                    continue;
                }
                if ("+-*/^".Contains(current))
                {
                    bool leftIsBlank = (i > 0 && segments[i - 1] == "<sprite name=\"question\">");
                    bool rightIsBlank = (i < segments.Count - 1 && segments[i + 1] == "<sprite name=\"question\">");

                    if (!leftIsBlank && !rightIsBlank)
                    {
                        expression.Append(current);
                    }
                    continue;
                }
                if (current == "!")
                {
                    if (!lastWasBlank && expression.Length > 0 && char.IsDigit(expression[expression.Length - 1]))
                    {
                        expression.Append(current);
                    }
                    continue;
                }
                expression.Append(current);
            }
            Debug.Log(expression);
            return expression.ToString();
        }
        private void ResetList()
        {
            foreach (var text in generatedTexts)
            {
                Destroy(text.gameObject);
            }
            foreach (var item in tMPrefabs)
            {
                Destroy(item.gameObject);
            }
            formulaSegments.Clear();
            generatedTexts.Clear();
            changedIndex.Clear();
            tMPrefabs.Clear();
            pressedButtons.Clear();
            visblaeOperators = 0;
        }
        public void OnIndexLevelChange(int indexLevel)
        {
            OnLevelIndexChange?.Invoke(indexLevel);
        }

    }
}

