using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI hpText;

    public void UpdatePoints(int points)
    {
        pointsText.text = "Score: " + points.ToString();
    }

    public void UpdateHP(int hp)
    {
        hpText.text = hp.ToString() + " HP";
    }
}
