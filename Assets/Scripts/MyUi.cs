using UnityEngine;
using UnityEngine.UI;

public class MyUi : MonoBehaviour {
    
    public Text myStepText;
    public Slider chanceTreeGrowingSlider;
    public Slider chanceTreeBurningSlider;
    public Slider secondsBetweenStepsSlider;
    public GameObject panel;  // used to hide the UI

    private Board board;

    public int Step {
        set => myStepText.text = "Steps : " + value;
    }

    private void Start() {
        panel.SetActive(false);
        board = GameObject.Find("Board").GetComponent<Board>();
        chanceTreeGrowingSlider.value = board.chanceTreeGrowing;
        chanceTreeBurningSlider.value = board.chanceTreeBurning;
        secondsBetweenStepsSlider.value = board.secondsBetweenSteps;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            panel.SetActive(!panel.activeSelf);  // toggle panel active/hide
        }
    }

    public void OnChanceTreeGrowing(float chance) {
        board.chanceTreeGrowing = chance;
    }
    
    public void OnChanceTreeBurning(float chance) {
        board.chanceTreeBurning = chance;
    }
    
    public void OnsecondsBetweenSteps(float seconds) {
        board.secondsBetweenSteps = seconds;
    }
}