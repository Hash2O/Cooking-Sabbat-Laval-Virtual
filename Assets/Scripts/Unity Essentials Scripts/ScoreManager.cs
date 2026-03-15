using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{

    //Obviously, this script manages the player's score
    //while throwing balls inside the circles;

    //Score UI
    [SerializeField] TextMeshProUGUI scoreText;

    [SerializeField] List<TargetManager> targets;

    [SerializeField] private ItemCollection itemCollection;
    [SerializeField] private SecretBonusItem bonusItem;

    private int scoreValue;

    // Start is called before the first frame update
    void Start()
    {
        scoreValue = 0;
        string score = scoreValue.ToString();
        scoreText.SetText("Score : " + score);
    }

    // Update is called once per frame
    void Update()
    {
        scoreUpdate();
    }

    void scoreUpdate()
    {
        scoreValue = targets[0].targetScore + targets[1].targetScore + targets[2].targetScore;
        if(bonusItem.isCollected)
        {
            scoreValue += 500;
        }
        if(itemCollection.isLevelFinished)
        {
            scoreValue += 1000;
        }
        scoreText.SetText("Score : " + scoreValue);
    }
}
