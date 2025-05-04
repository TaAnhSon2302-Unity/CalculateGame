using System.Collections.Generic;
using CalculateLevelManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CalculateUIManager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance { get; private set; }
        public GameObject winLoseUI;
        public GameObject winUI;
        public GameObject LoseUI;
        public List<Button> listButton;
        public Button nextLevel;
        public Button backspace;
        public Button retry;
        public Button hintButton;
        public Button soundButton;
        public Image soundIcon;
        [Header("Level title")]
        public TextMeshProUGUI levelTitle;
        [Header("Hint")]
        public TextMeshProUGUI hintCount;
        [Header("RequirementBar")]
        [SerializeField] private TextMeshProUGUI requiremntResult;
        [SerializeField] public TextMeshProUGUI currentResult;
        [Header("LoseUIComponent")]
        [SerializeField] private TextMeshProUGUI resultText;
        [Header("RepeatUI")]
        [SerializeField] private TextMeshProUGUI numberRepeatCount;
        [SerializeField] private TextMeshProUGUI operationRepeatCount;
        [SerializeField] public Sprite soundOn;
        [SerializeField] public Sprite soundOff;

        public void SetActiveWinUI()
        {
            winLoseUI.SetActive(true);
            winUI.SetActive(true);
        }
        public void SetActiveLoseUI(string result)
        {
            winLoseUI.SetActive(true);
            LoseUI.SetActive(true);
            UpdateResult(result);
        }
        public void ResetActive()
        {
            winLoseUI.SetActive(false);
            LoseUI.SetActive(false);
            winUI.SetActive(false);
        }
        public void SetInteractable()
        {
            foreach (var item in listButton)
            {
                item.interactable = true;
            }
        }
        public void SetUpUI(LevelSO currentLevel)
        {
            requiremntResult.text = LevelManager.instance.CalculateRequirementResult(currentLevel.hintFormula).ToString();
            levelTitle.text = currentLevel.name;
            hintCount.text = currentLevel.hintCount.ToString();
            SetUpRepeatAllowance(currentLevel.clickCount, currentLevel.operationClickCount);
        }
        public void SetUpUIForAutomatic(string formula, int indexLevel, int numberAllow, int operatorAllow)
        {
            requiremntResult.text = LevelManager.instance.CalculateRequirementResult(formula).ToString();
            levelTitle.text = "Level " + indexLevel.ToString();
            SetUpRepeatAllowance(numberAllow, operatorAllow);
        }
        public void UpdateCurrentResult(double result)
        {
            currentResult.text = result.ToString();
        }
        public void SetBlankForCurrentResult()
        {
            currentResult.text = "0";
        }
        public void UpdateResult(string result)
        {
            resultText.text = $"Result: <color=#FF0000>{result}</color>";
        }
        public void SetUpRepeatAllowance(int number, int operation)
        {
            numberRepeatCount.text = number.ToString();
            operationRepeatCount.text = operation.ToString();
        }
        public void SetBoolForSound(bool isSoundOn)
        {
            if(isSoundOn)
            {
                soundIcon.sprite = soundOn;
            }
            else
            {
                soundIcon.sprite = soundOff;
            }
        }
        public void TurnOffHintButton()
        {
            hintButton.interactable = false;
        }
        public void TurnOnHintButton()
        {
            hintButton.interactable = true;
        }
    }
}
