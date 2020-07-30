using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class CounterManager : MonoBehaviour
{
    [SerializeField] float Timer = 120.0f;
    [SerializeField] TextMeshProUGUI CounterText;
    [SerializeField] Image CounterBar;
    [SerializeField] GameObject GameOverPanel;
    [SerializeField] GameObject TryAgainButton;
    [SerializeField] GameObject MenuButton;

    float currentTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        CounterBar.fillAmount = 1;
        currentTime = Timer;
        CounterText.text = currentTime.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= 1 * Time.deltaTime;
            if (currentTime < 0) currentTime = 0;
            CounterText.text = currentTime.ToString("00");
            CounterBar.fillAmount -= 1 * Time.deltaTime / Timer;
            if (CounterBar.fillAmount < 0) CounterBar.fillAmount = 0;
        }
        if (currentTime == 0) LevelFailed();
    }

    void LevelFailed()
    {
        GridManager.Instance.gemClickLock = true;
        GameOverPanel.SetActive(true);
        StartCoroutine(ActivateButtons());
    }

    public void ResetTimer()
    {
        CounterBar.fillAmount = 1;
        currentTime = Timer;
        CounterText.text = currentTime.ToString();
    }

    IEnumerator ActivateButtons()
    {
        yield return new WaitForSeconds(1.0f);
        TryAgainButton.SetActive(true);
        MenuButton.SetActive(true);
    }
}
