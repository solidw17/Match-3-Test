using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ScoreText;
    [SerializeField] TextMeshProUGUI TargetText;
    [SerializeField] GameObject VictoryPanel;
    [SerializeField] GameObject NextLevelButton;
    [SerializeField] GameObject MenuButton;

    CounterManager counterManager;
    int score = 0;
    int target = 1000;

    // Start is called before the first frame update
    void Start()
    {
        counterManager = FindObjectOfType<CounterManager>();
        ScoreText.text = score.ToString();
        TargetText.text = target.ToString();
    }

    public void AwardPoints(int gemsMatched)
    {
        score += gemsMatched * 100;
        ScoreText.text = score.ToString();
        if(score > target)
        {
            Victory();
        }
    }

    void Victory()
    {
        GridManager.Instance.gemClickLock = true;
        VictoryPanel.SetActive(true);
        StartCoroutine(ActivateButtons());
    }

    public void NextLevel()
    {
        VictoryPanel.SetActive(false);
        NextLevelButton.SetActive(false);
        MenuButton.SetActive(false);
        target += 1000;
        score = 0;
        ScoreText.text = score.ToString();
        TargetText.text = target.ToString();
        counterManager.ResetTimer();
        GridManager.Instance.gemClickLock = false;
    }

    IEnumerator ActivateButtons()
    {
        yield return new WaitForSeconds(1.0f);
        NextLevelButton.SetActive(true);
        MenuButton.SetActive(true);
    }
}
