using UnityEngine;
using UnityEngine.UI;

public class MyUi : MonoBehaviour {
    
    public Text myStepText;
    public Slider chanceTreeGrowingSlider;
    public Slider chanceTreeBurningSlider;
    public Slider secondsBetweenStepsSlider;
    public Slider cameraSpeedSlider;
    public GameObject panel;  // used to hide the UI
    public GameObject tip;

    private Board board;
    private MyCamera myCamera;

    public int Step {
        set => myStepText.text = "Steps : " + value;
    }

    private void Start() {
        panel.SetActive(false);
        tip.SetActive(true);
        board = GameObject.Find("Board").GetComponent<Board>();
        myCamera = GameObject.Find("Main Camera").GetComponent<MyCamera>();
        chanceTreeGrowingSlider.value = board.chanceTreeGrowing;
        chanceTreeBurningSlider.value = board.chanceTreeBurning;
        secondsBetweenStepsSlider.value = board.secondsBetweenSteps;
        cameraSpeedSlider.value = myCamera.speed;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            panel.SetActive(!panel.activeSelf);  // toggle panel active/hide
            tip.SetActive(!panel.activeSelf);  // toggle panel active/hide
        } else if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public void OnChanceTreeGrowingChange(float chance) {
        board.chanceTreeGrowing = chance;
    }
    
    public void OnChanceTreeBurningChange(float chance) {
        board.chanceTreeBurning = chance;
    }
    
    public void OnsecondsBetweenStepsChange(float seconds) {
        board.secondsBetweenSteps = seconds;
    }
    
    public void OnCameraSpeedChange(float speed) {
        myCamera.speed = speed;
    }
}