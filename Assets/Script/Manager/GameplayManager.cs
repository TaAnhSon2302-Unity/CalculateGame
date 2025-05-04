using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CalculateLevelManager;
using CalculateUIManager;
using AutomaticLevelManager;
using CalculateGameSoundManager;
using DG.Tweening;

namespace CalculateGameplayManager
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] int turnCount;
        public LevelManager levelManager;
        public UIManager uIManager;
        public AutomaticLevelManger automaticLevel;
        public CalculateSoundManager calculateSoundManager;
        public int hintIndex = 0;
        public int hintClickCount = 0;
        public static float hintCoolDown = 10f;
        public string hintstring;

        public GameConfig gameConfig;
        public List<ButtonPrefab> buttonPrefabs = new List<ButtonPrefab>();
        public Color blinkColor = Color.white;
        public float duration = 1f;
        public int loopCount = 5;
        public bool isSoundOn = true;


        void Awake()
        {
            levelManager.OnLevelIndexChange += OnLevelChange;
        }
        void Start()
        {

            uIManager.currentResult.color = Color.red;
            ButtonPrefab.OnButtonPressed += HandleButtonPress;
            if (levelManager.indexLevel <= levelManager.levelIndexForHard)
            {
                levelManager.LoadLevel(levelManager.indexLevel);
                levelManager.GenerateTextFromHint(levelManager.currentLevel.hintFormula, levelManager.currentLevel);
                LoadData();
                uIManager.SetUpUI(levelManager.currentLevel);
            }
            else
            {
                SetAutoMaticLevel();
            }
            uIManager.hintCount.text = hintClickCount.ToString();
            SetUpCLickCountForButton();
            AddListener();
            calculateSoundManager.gameObject.SetActive(LoadBool("SoundOn"));
            uIManager.SetBoolForSound(isSoundOn);
        }
        void LoadData()
        {
            if (levelManager.indexLevel <= levelManager.levelIndexForHard)
            {
                turnCount = levelManager.currentLevel.hintFormula.Length - levelManager.visblaeOperators;
                hintstring = levelManager.currentLevel.hintFormula;
            }
            else
            {
                turnCount = levelManager.automaticFormula.Length - levelManager.visblaeOperators;
                hintstring = levelManager.automaticFormula;
            }
            ResetHint();
        }
        void AddListener()
        {
            uIManager.nextLevel.onClick.AddListener(NextLevel);
            uIManager.backspace.onClick.AddListener(HandleBackSpace);
            uIManager.hintButton.onClick.AddListener(ShowNextHintCharcater);
            uIManager.retry.onClick.AddListener(Retry);
            uIManager.soundButton.onClick.AddListener(SetActiveSound);
        }
        private void HandleButtonPress(string input, ButtonPrefab buttonPrefab)
        {
            if (turnCount <= 0) return;
            int index = levelManager.formulaSegments.IndexOf("<sprite name=\"question\">");
            if (index == -1) return;
            calculateSoundManager.PlaySound(CalculateSoundName.CLICK, 0.3f);
            levelManager.changedIndex.Add(index);
            levelManager.formulaSegments[index] = input;
            levelManager.generatedTexts[index].text = input;
            levelManager.pressedButtons.Add(buttonPrefab);
            levelManager.tMPrefabs[index].KillDoTween();
            buttonPrefab.buttonEffect.PlayEffect();
            turnCount--;
            buttonPrefab.StartVFX();

            string updatedExpression = levelManager.BuildExpression(levelManager.formulaSegments);
            if (!string.IsNullOrEmpty(updatedExpression))
            {
                double? result = levelManager.EvaluateExpression(updatedExpression);
                if (result.HasValue && (char.IsDigit(input[0]) || input == "!"))
                {

                    uIManager.UpdateCurrentResult(result.Value);
                }
            }
            if (turnCount == 0)
            {
                uIManager.currentResult.DOColor(Color.white, duration).SetLoops(loopCount, LoopType.Yoyo)
                .OnComplete(
                    () => levelManager.FinalResult());
            }
            buttonPrefab.CheckClickCount();
            buttonPrefab.UpdateClickCount();
            uIManager.TurnOffHintButton();
        }
        public void NextLevel()
        {
            levelManager.indexLevel++;
            if (levelManager.indexLevel <= levelManager.levelIndexForHard)
            {
                levelManager.OnIndexLevelChange(levelManager.indexLevel);
                RetryCurrrentLevel();
            }
            else
            {
                levelManager.OnIndexLevelChange(levelManager.indexLevel);
                SetAutoMaticLevel();
            }
        }
        public void OnLevelChange(int levelIndex)
        {
            PlayerPrefs.SetInt("LevelIndex", levelIndex);
            PlayerPrefs.Save();
            Debug.Log("Save");
        }
        public void Retry()
        {
            if (levelManager.indexLevel <= levelManager.levelIndexForHard)
            {

                ResetLevel();
            }
            else
            {
                ResetAutoMaticLevel();
            }
        }
        public void ResetLevel()
        {
            calculateSoundManager.PlaySound(CalculateSoundName.RETRY, 0.2f);
            if (levelManager.indexLevel <= levelManager.levelIndexForHard)
            {
                levelManager.LoadLevel(levelManager.indexLevel);
                levelManager.GenerateTextFromHint(levelManager.currentLevel.hintFormula, levelManager.currentLevel);
                turnCount = levelManager.currentLevel.hintFormula.Length - levelManager.visblaeOperators;
                hintstring = levelManager.currentLevel.hintFormula;
                uIManager.SetUpUI(levelManager.currentLevel);
                ResetHint();
                SetUpCLickCountForButton();
                ResetUI();
            }
        }
        public void RetryCurrrentLevel()
        {
            calculateSoundManager.PlaySound(CalculateSoundName.RETRY, 0.2f);
            if (levelManager.indexLevel <= levelManager.levelIndexForHard)
            {
                levelManager.LoadLevel(levelManager.indexLevel);
                levelManager.GenerateTextFromHint(levelManager.currentLevel.hintFormula, levelManager.currentLevel);
                turnCount = levelManager.currentLevel.hintFormula.Length - levelManager.visblaeOperators;
                hintstring = levelManager.currentLevel.hintFormula;
                uIManager.SetUpUI(levelManager.currentLevel);
                ResetHint();
                SetUpCLickCountForButton();
                ResetUI();
            }
        }
        public void HandleBackSpace()
        {
            calculateSoundManager.PlaySound(CalculateSoundName.RETRY, 0.2f);
            if (turnCount == 0) return;
            if (levelManager.changedIndex.Count > 0)
            {
                int lastIndex = levelManager.changedIndex[levelManager.changedIndex.Count - 1];
                string lastInput = levelManager.formulaSegments[lastIndex];
                levelManager.formulaSegments[lastIndex] = "<sprite name=\"question\">";
                levelManager.generatedTexts[lastIndex].text = "<sprite name=\"question\">";
                levelManager.tMPrefabs[lastIndex].StartBounceEffect();
                ButtonPrefab lastButton = levelManager.pressedButtons[levelManager.pressedButtons.Count - 1];
                turnCount++;
                if ("+-*/^!".Contains(lastInput))
                {

                    lastButton.ClickCountForRetry();
                    lastButton.UpdateClickCount();
                }
                else
                {
                    lastButton.ClickCountForRetry();
                    lastButton.UpdateClickCount();
                }
                levelManager.changedIndex.RemoveAt(levelManager.changedIndex.Count - 1);
                levelManager.pressedButtons.RemoveAt(levelManager.pressedButtons.Count - 1);
            }
            string updatedExpression = levelManager.BuildExpression(levelManager.formulaSegments);
            double? result = levelManager.EvaluateExpression(updatedExpression);
            if (result.HasValue)
            {
                uIManager.UpdateCurrentResult(result.Value);
            }
            if (levelManager.pressedButtons.Count == 0)
            {
                if (hintClickCount == 0)
                {
                    uIManager.hintButton.interactable = false;

                }
                else
                {
                    uIManager.hintButton.interactable = true;
                }
            }
        }
        public void ShowNextHintCharcater()
        {
            calculateSoundManager.PlaySound(CalculateSoundName.RETRY, 0.2f);
            if (hintClickCount == 0 || turnCount == 0) return;
            if (hintIndex < (levelManager.indexLevel <= levelManager.levelIndexForHard ? levelManager.currentLevel.hintFormula.Length : levelManager.automaticFormula.Length))
            {
                if (levelManager.formulaSegments[hintIndex] == "<sprite name=\"question\">")
                {
                    levelManager.tMPrefabs[hintIndex].KillDoTween();
                    SetDataAfterHint(hintIndex);
                }
                else
                {
                    while (levelManager.formulaSegments[hintIndex] != "<sprite name=\"question\">")
                    {
                        hintIndex++;
                    }
                    levelManager.tMPrefabs[hintIndex].KillDoTween();
                    SetDataAfterHint(hintIndex);
                }
            }
            string updatedExpression = levelManager.BuildExpression(levelManager.formulaSegments);
            double? result = levelManager.EvaluateExpression(updatedExpression);
            Debug.Log(updatedExpression);
            if (result.HasValue)
            {
                uIManager.UpdateCurrentResult(result.Value);
            }
            if (hintClickCount == 0)
            {
                uIManager.hintButton.interactable = false;
            }
            uIManager.hintCount.text = hintClickCount.ToString();
        }
        public void SetDataAfterHint(int hintIndex)
        {
            levelManager.formulaSegments[hintIndex] = hintstring[hintIndex].ToString();
            levelManager.tMPrefabs[hintIndex].textMesh.text = hintstring[hintIndex].ToString();
            levelManager.tMPrefabs[hintIndex].isOpenedByHint = true;
            levelManager.tMPrefabs[hintIndex].SetColor();
            ButtonPrefab hintVaule = buttonPrefabs.Find(x => x.vaule == hintstring[hintIndex].ToString());
            string lastInput = levelManager.formulaSegments[hintIndex];
            if ("+-*/^!".Contains(lastInput))
            {
                hintVaule.CheckClickCount();
                hintVaule.UpdateClickCount();
            }
            else
            {
                hintVaule.CheckClickCount();
                hintVaule.UpdateClickCount();
            }
            string updatedExpression = levelManager.BuildExpression(levelManager.formulaSegments);
            double? result = levelManager.EvaluateExpression(updatedExpression);
            Debug.Log(updatedExpression);
            if (result.HasValue)
            {
                uIManager.UpdateCurrentResult(result.Value);
            }
            hintClickCount--;
            turnCount--;
            hintIndex++;
        }
        private void ResetHint()
        {
            if (levelManager.indexLevel <= levelManager.levelIndexForHard)
            {
                hintClickCount = levelManager.currentLevel.hintCount;
            }
            else
            {
                if (InRange(levelManager.automaticFormula.Length, 4, 5))
                {
                    hintClickCount = 1;
                }
                else if (InRange(levelManager.automaticFormula.Length, 6, 8))
                {
                    hintClickCount = 2;
                }
                else if (InRange(levelManager.automaticFormula.Length, 9, 10))
                {
                    hintClickCount = 3;
                }
            }
            uIManager.hintCount.text = hintClickCount.ToString();
            hintIndex = 0;
            uIManager.hintButton.interactable = true;
        }
        public void SetAutoMaticLevel()
        {
            levelManager.randomDifficulty = gameConfig.difficultyConfigs[UnityEngine.Random.Range(0, 4)];
            levelManager.automaticFormula = automaticLevel.AutomaticGenerateLevel(levelManager.randomDifficulty);
            Debug.Log(levelManager.automaticFormula);
            levelManager.GenerateTextFromAutomatic(levelManager.automaticFormula, levelManager.randomDifficulty);
            LoadData();
            uIManager.SetUpUIForAutomatic(levelManager.automaticFormula, levelManager.indexLevel, automaticLevel.numberAllowence, automaticLevel.operatorAllowence);
            SetUpCLickCountForButton();
            ResetHint();
            ResetUI();
        }
        public void ResetAutoMaticLevel()
        {
            Debug.Log(levelManager.automaticFormula);
            levelManager.GenerateTextFromAutomatic(levelManager.automaticFormula, levelManager.randomDifficulty);
            LoadData();
            uIManager.SetUpUIForAutomatic(levelManager.automaticFormula, levelManager.indexLevel, automaticLevel.numberAllowence, automaticLevel.operatorAllowence);
            uIManager.hintButton.interactable = true;
            SetUpCLickCountForButton();
            ResetHint();
            ResetUI();
        }
        public void ResetUI()
        {
            uIManager.currentResult.color = Color.red;
            uIManager.SetBlankForCurrentResult();
            uIManager.SetInteractable();
            uIManager.ResetActive();
        }
        public void SetUpCLickCountForButton()
        {
            foreach (var item in buttonPrefabs)
            {
                if ("+-/*^!".Contains(item.vaule))
                {
                    item.SetClickCount(levelManager.indexLevel <= levelManager.levelIndexForHard ? levelManager.currentLevel.operationClickCount : automaticLevel.operatorAllowence);
                }
                else
                {
                    item.SetClickCount(levelManager.indexLevel <= levelManager.levelIndexForHard ? levelManager.currentLevel.clickCount : automaticLevel.numberAllowence);
                }
            }
        }
        public void SetActiveSound()
        {
            if (isSoundOn)
            {
                calculateSoundManager.gameObject.SetActive(false);
                isSoundOn = false;
                SaveBool("SoundOn", isSoundOn);
                uIManager.SetBoolForSound(isSoundOn);

            }
            else
            {
                calculateSoundManager.gameObject.SetActive(true);
                isSoundOn = true;
                SaveBool("SoundOn", isSoundOn);
                uIManager.SetBoolForSound(isSoundOn);
            }
        }
        void SaveBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        bool LoadBool(string key)
        {
            return PlayerPrefs.GetInt(key, 1) == 1;
        }
        public static bool InRange(int number, int min, int max)
        {
            return number >= min && number <= max;
        }
    }
}


