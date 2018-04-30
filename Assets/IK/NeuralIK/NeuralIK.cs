using UnityEngine;
using System.Text;

public class NeuralIK : ZeroMQBehaviour {
  public bool addTrainingData = true;
  protected override void HandleMessage(string message) {
    //If receive an "s " for "Simulation Sample", simulate the IK and send back
    if (message.StartsWith("s ")) {
      string[] trainingInputs = message.Substring(2).Split(';');
      StringBuilder trainingSamples = new StringBuilder(trainingInputs.Length + 1);
      for (int i = 0; i < (addTrainingData?trainingInputs.Length:16); i++) {
        string[] inputString = trainingInputs[i].Split(',');
        Vector3 input = new Vector3(float.Parse(inputString[0]), float.Parse(inputString[1]), 0f);
        float baseRot, elbowRot;
        solveIK(input, out baseRot, out elbowRot);
        trainingSamples.Append(input.x + "," + input.y + "," + baseRot + "," + elbowRot + ";");
      }

      //Append a mouse sample so we can see how the network is performing
      Vector3 mousePos = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
      trainingSamples.Append(mousePos.x + "," + mousePos.y);

      Send(trainingSamples.ToString());

    //If receive a "v " for "Visualization Input", visualize the prediction
    } else if (message.StartsWith("v ")) {
      string[] visualizationInput = message.Substring(2).Split(',');
      Vector3 elbowPosition = new Vector3(Mathf.Sin(float.Parse(visualizationInput[0])),
                                          Mathf.Cos(float.Parse(visualizationInput[0])), 0f);
      Vector3 tipPosition = new Vector3(Mathf.Sin(float.Parse(visualizationInput[0]) + float.Parse(visualizationInput[1])),
                                        Mathf.Cos(float.Parse(visualizationInput[0]) + float.Parse(visualizationInput[1])), 0f) + elbowPosition;
      Debug.DrawLine(Vector3.zero, elbowPosition, Color.green, 0.1f);
      Debug.DrawLine(elbowPosition, tipPosition, Color.green, 0.1f);
    }
  }

  void solveIK(Vector3 input, out float baseRot, out float elbowRot, bool draw = false) {
    baseRot = 0f; elbowRot = 0f;
    Vector3 elbowPosition = Vector3.right, tipPosition = Vector3.right * 2f;
    for (int i = 0; i < 10; i++) {
      //Solve elbow joint
      Vector3 elbowToTip = (tipPosition - elbowPosition).normalized, elbowToTarget = (input - elbowPosition).normalized;
      elbowRot += Mathf.Atan2(elbowToTip.y, elbowToTip.x) - Mathf.Atan2(elbowToTarget.y, elbowToTarget.x);
      tipPosition = new Vector3(Mathf.Sin(baseRot + elbowRot), Mathf.Cos(baseRot + elbowRot), 0f) + elbowPosition;

      //Solve base joint
      baseRot += Mathf.Atan2(tipPosition.normalized.y, tipPosition.normalized.x) - Mathf.Atan2(input.normalized.y, input.normalized.x);
      elbowPosition = new Vector3(Mathf.Sin(baseRot), Mathf.Cos(baseRot), 0f);
      tipPosition = new Vector3(Mathf.Sin(baseRot + elbowRot), Mathf.Cos(baseRot + elbowRot), 0f) + elbowPosition;
    }
    if (draw) {
      Debug.DrawLine(Vector3.zero, elbowPosition);
      Debug.DrawLine(elbowPosition, tipPosition);
    }
  }

  protected override void Update() {
    base.Update();

    //Draw the "Ground Truth" IK Arm
    float baseRot, elbowRot;
    solveIK(GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition), out baseRot, out elbowRot, true);
  }
}
