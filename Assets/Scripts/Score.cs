using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {
    // Start is called before the first frame update
    private int score = 0;
    public Text scoreText;
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        scoreText.text = score.ToString();
    }
    
    public void addScore(int line) {
        switch(line) {
            case 1:
                score += 100;
                break;
            case 2:
                score += 400;
                break;
            case 3:
                score += 900;
                break;
            case 4:
                score += 1600;
                break;
        }
    }
    
    public void subtractScore() {
        score -= 500;
    }
    public int getScore() {
        return score;
    }
    public void initScore() {
        score = 0;
    }
}
