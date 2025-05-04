using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class ButtonPrefab : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;
    public static event Action<string, ButtonPrefab> OnButtonPressed;
    [SerializeField] public string vaule;
    [SerializeField] public TextMeshProUGUI vauleText;
    [SerializeField] public TextMeshProUGUI clickTime;
    [SerializeField] public int clickCount;
    [SerializeField] public ButtonEffect buttonEffect;
    [SerializeField] public Button buttonComp;
    private void Awake()
    {
        buttonComp = GetComponent<Button>();
        vaule = vauleText.text;
        buttonComp.onClick.AddListener(() =>
        {
            OnButtonPressed?.Invoke(vaule, this);
        });
        buttonEffect = GetComponent<ButtonEffect>();
    }
    public void StartVFX()
    {
        //StartCoroutine(VFXduration());
    }
    public void SetClickCount(int click)
    {
        clickCount = click;
        clickTime.text = clickCount.ToString();
    }
    IEnumerator VFXduration()
    {
        particleSystem.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        particleSystem.gameObject.SetActive(false);
    }
    public void CheckClickCount()
    {
        clickCount--;
        if (clickCount == 0)
        {
            buttonComp.interactable = false;
        }
    }
    public void UpdateClickCount()
    {
        clickTime.text = clickCount.ToString();
    }
    public void ClickCountForRetry()
    {
        clickCount++;
        if (clickCount > 0)
        {
            buttonComp.interactable = true;
        }
    }
}


