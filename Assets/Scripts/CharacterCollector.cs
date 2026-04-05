using UnityEngine;
using UnityEngine.UI;

public class CharacterCollector : MonoBehaviour
{
    public Transform target;
    public Text scoreText;
    private int score = 0;

    public void AddScore()
    {
        score++;
        if (scoreText != null)
            scoreText.text = "Collected: " + score;
    }
}