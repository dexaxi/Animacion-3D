using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TextHandler : MonoBehaviour
{
    public MassSpring myMassSpring; 
    public TextMeshProUGUI text;

    public string pausedString;
    public string dampingString;
    public string windString;

   
    private void Awake() {
        myMassSpring = FindObjectOfType<MassSpring>();
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update() {

        //"parser"
        if(myMassSpring.paused) pausedString = "True"; else pausedString = "False";
        if(myMassSpring.damping) dampingString = "True"; else dampingString = "False";
        if(myMassSpring.wind) windString = "True"; else windString = "False";
        
        //Text
        text.text = pausedString + "\n" + dampingString + "\n" + windString + "\n" + myMassSpring.subSteps;

    }


}
