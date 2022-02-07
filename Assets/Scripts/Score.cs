using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {
    // Start is called before the first frame update
    private int score = 0;
    private int combo = 0;
    public Text scoreText;
    public Text comboText;

    void Start() {
        SCUpdate();
    }

    void SCUpdate() {
        scoreText.text = score.ToString();
        comboText.text = combo.ToString();
    }
    public void initSC() {
        score = 0;
        combo = 0;
        SCUpdate();
    }
    
    public void initScore() {
        score = 0;
        SCUpdate();
    }
    public void addScore(int s) {
        score += (int) (s * (1.0f + 0.1 * combo));
        SCUpdate();
    }
    public void addLineScore(int line) {
        addScore(line * line * 1000);
    }
    public int getScore() {
        return score;
    }

    public void initCombo() {
        combo = 0;
        SCUpdate();
    }
    public void addCombo() {
        combo += 1;
        SCUpdate();
    }
}
