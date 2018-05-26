using UnityEngine;
using System.Text;

public class CurveFitting : ZeroMQBehaviour {
  public bool addTrainingData = true;
  float currentTime = 0f;
  float rangeScalar = 0.5f;
  protected override void HandleMessage(string message) {
    //If receive an "s " for "Simulation Sample", simulate the function and send back
    if (message.Equals("s")) {
      StringBuilder trainingSamples = new StringBuilder(2048 + 1);
      for (int i = 0; i < (addTrainingData ? 2048 : 0); i++) {
        StringBuilder trainingSample = new StringBuilder(20);
        float trainingTime = Random.value * 10000f; float offset = sumOfSines(trainingTime);
        for (int j = 0; j < 10; j++) {
          trainingSample.Append(sumOfSines(trainingTime - (j*rangeScalar)) - offset);
          trainingSample.Append(",");
        }
        for (int j = 0; j < 10; j++) {
          trainingSample.Append(sumOfSines(trainingTime + ((j + 1)* rangeScalar)) - offset);
          trainingSample.Append(",");
        }
        trainingSample.Remove(trainingSample.Length - 1, 1);
        trainingSample.Append(";");

        trainingSamples.Append(trainingSample.ToString());
      }

      //Append a mouse sample so we can see how the network is performing
      StringBuilder evaluationSample = new StringBuilder(10);
      for (int i = 0; i < 10; i++) {
        evaluationSample.Append(sumOfSines(currentTime - (i * rangeScalar)));
        evaluationSample.Append(",");
      }
      evaluationSample.Remove(evaluationSample.Length - 1, 1);
      trainingSamples.Append(evaluationSample.ToString());

      Send(trainingSamples.ToString());

      //If receive a "v " for "Visualization Input", visualize the prediction
    } else if (message.StartsWith("v ")) {
      string[] visualizationInput = message.Substring(2).Split(',');
      for (int i = 0; i< visualizationInput.Length-1; i++) {
        Debug.DrawLine((Vector3.right * ((i + 1) * rangeScalar)) + (Vector3.up * float.Parse(visualizationInput[i])),
                       (Vector3.right * ((i + 2) * rangeScalar)) + (Vector3.up * float.Parse(visualizationInput[i+1])), Color.green);
      }
    }
  }

  //Faux Market Data
  float sumOfSines(float time) {
    return ((Mathf.Sin(time * 1f) * 2f) + 
            (Mathf.Sin(time * .5f) * 4f) + 
            (Mathf.Sin(time * 3f) * .5f) + 
            (Mathf.Sin(time * .2f) * .3f) + 
            (Mathf.Sin(time * 2f) * 3f) + 
            (Mathf.Sin(time / 9f) * 6f));
  }

  //Draw the now-centered market data line
  void OnDrawGizmos() {
    float offset = sumOfSines(currentTime);
    for (float i = -10f; i < 10f; i += 0.1f) {
      Gizmos.DrawLine((Vector3.right * (i* rangeScalar)) +        (Vector3.up * (sumOfSines(currentTime + (i* rangeScalar)) - offset)),
                      (Vector3.right * ((i + 0.1f)* rangeScalar)) + (Vector3.up * (sumOfSines(currentTime + ((i + 0.1f)* rangeScalar))- offset)));
    }
  }

  protected override void Update() {
    base.Update();

    currentTime += Time.unscaledDeltaTime;

    if (Input.GetMouseButtonDown(0)) {
      addTrainingData = !addTrainingData;
    }
  }
}
