using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    GameManager inst;

    private void Start()
    {
        inst = GameManager.instance;
    }

    public void Easy()
    {
        inst.ChangeDifficulty(GameManager.Difficulty.Easy);
        inst.uiPromptText.text = "Current Difficulty: Easy";
    }

    public void Medium()
    {
        inst.ChangeDifficulty(GameManager.Difficulty.Medium);
        inst.uiPromptText.text = "Current Difficulty: Medium";
    }

    public void Hard()
    {
        inst.ChangeDifficulty(GameManager.Difficulty.Hard);
        inst.uiPromptText.text = "Current Difficulty: Hard";
    }
}
