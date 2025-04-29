using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    public int stepIndex = 0;

    void Update()
    {
        // Tutorial Steps
        switch (stepIndex) {
            case 0:
                tutorialText.text = "Use WASD to move.";
                if (Input.GetKeyDown(KeyCode.W)) {
                    CompleteStep();
                }
                break;
            case 1:
                tutorialText.text = "Press Space to Jump.";
                if (Input.GetKeyDown(KeyCode.Space)) {
                    CompleteStep();
                }
                break;
            case 2:
                tutorialText.text = "Walk into the floating orange cube.";
                break;
            case 3:
                tutorialText.text = "Great, You just picked up an ability! Now use your right mouse button to use the ability.";
                if (Input.GetKeyDown(KeyCode.Mouse1)) {
                    CompleteStep();
                }
                break;
            case 4:
                tutorialText.text = "You can use the left mouse button to shoot, kill the enemy in front of you.";
                if (Input.GetKeyDown(KeyCode.Mouse0)) {
                    CompleteStep();
                }
                break;
            case 5:
                tutorialText.text = "You can hold hold up to 2 abilities, Use Q to swap between them and use the slide to go under the wall.";
                if (Input.GetKeyDown(KeyCode.Mouse1)) {
                    CompleteStep();
                }
                break;
            case 6:
                tutorialText.text = "Great now use your Double Jump to get up to the higher platform";
                if (Input.GetKeyDown(KeyCode.Mouse1)) {
                    CompleteStep();
                }
                break;
            case 7:
                tutorialText.text = "You need to kill all the enemies in the level for the exit to open, for now press K to finish this step";
                if (Input.GetKeyDown(KeyCode.K)) {
                    CompleteStep();
                }
                break;
            case 8:
                tutorialText.text = "Well done you have completed the tutorial! Now walk into the green object to finish the level!";
                break;
        }
    }

    public void CompleteStep() {
        stepIndex++;
    }
}
