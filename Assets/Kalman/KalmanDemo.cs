using UnityEngine;

public class KalmanDemo : MonoBehaviour {
  KalmanFilter filter;

  //Initialize the filter
  void Start() {
    filter = new KalmanFilter(new float[,]
         {{0f}, //XPos
			    {0f}, //YPos
			    {0f}, //ZPos
			    {0f}, //XVel
			    {0f}, //YVel
			    {0f}, //ZVel
			    {0f}, //XAccel
			    {0f}, //YAccel
			    {0f}, //ZAccel
         },

         //Measurement Matrix
         //Tells the filter which of the above we're actually sensing
         //Turns out you can just straight up comment out the lines that 
         //you want the filter to solve for
         new float[,]
          {{1f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f}, //XPos
			  //{0f, 1f, 0f, 0f, 0f, 0f, 0f, 0f, 0f}, //YPos - Not Measured!
			  //{0f, 0f, 1f, 0f, 0f, 0f, 0f, 0f, 0f}, //ZPos - Not Measured!
			  //{0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 0f}, //XVel - Not Measured!
			  //{0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f}, //YVel - Not Measured!
			  //{0f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f}, //ZVel - Not Measured!
			    {0f, 0f, 0f, 0f, 0f, 0f, 1f, 0f, 0f} //XAccel
        //{0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 0f}, //YAccel - Not Measured!
        //{0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f} //ZAccel - Not Measured!
          },

         //Process Noise; how much each value will deviate from the predicted value
         //Quick gaussian distribution intuition
         //If the ProcessNoise is 0.1 then you're saying that
         //68% of the time, the actual system will deviate less than sqrt(0.1) *from the predicted value*
         //and 95% of the time, less than 2*sqrt(0.1) etc.
         //It's a measure of how good your StateTransitionMatrix is
         //Ours sucks since it doesn't factor in that a meddling sine wave is moving the tracker around
         //So our numbers need to be kind of high
         new float[,]
          {{.000025f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f}, //XPos
			    {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f}, //YPos
			    {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f}, //ZPos
			    {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f}, //XVel
			    {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f}, //YVel
			    {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f}, //ZVel
			    {0f, 0f, 0f, 0f, 0f, 0f, 0.025f, 0f, 0f}, //Constant XAccel
			    {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f}, //Constant YAccel
			    {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f} //Constant ZAccel
          },

         //Same as above, except now it's the measurement deviating from actual position (not the predicted position)
         new float[,]
          {{60f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f}, //XPos
			    {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f}, //YPos
			    {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f}, //ZPos
			    {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f}, //XVel
			    {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f}, //YVel
			    {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f}, //ZVel
			    {0f, 0f, 0f, 0f, 0f, 0f, 0.0005f, 0f, 0f}, //XAccel
			    {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f}, //YAccel
			    {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f} //ZAccel
          },

         //Initial Error measurement; 0f = slow initial convergence, 1f = fast initial convergence
         0.1f
     );
  }

  float noisyXPosition = 0f;
  void Update() {
    filter.PredictState(Time.deltaTime);

    //Dummy Data : Generate a very noisy position estimate
    noisyXPosition = Mathf.Sin(Time.time) + Random.Range(-0.5f, 0.5f);
    filter.UpdateState(new float[,]
     {{noisyXPosition}, //Noisy Position
			{0f},
      {0f},
      {0f},
      {0f},
      {0f},
      {-Mathf.Sin(Time.time)}, //Flawless acceleration
			{0f},
      {0f}});

    float oldPos = filter.StateMatrix[0, 0];
    for (int i = 1; i < 10; i++) {
      float[,] PredictedState = filter.SafePredictState(0.1f * i);
      Debug.DrawLine(new Vector3(oldPos, i * 0.02f, 0f), new Vector3(PredictedState[0, 0], (i + 1) * 0.02f, 0f), Color.green);
      oldPos = PredictedState[0, 0];

      Debug.DrawLine(new Vector3(Mathf.Sin((Time.time + (i * 0.1f))), i * 0.02f, 0f), 
                     new Vector3(Mathf.Sin((Time.time + ((i + 1) * 0.1f))), (i + 1) * 0.02f, 0f), Color.red);
    }
  }

  //Draw the state of the system and the filter
  void OnDrawGizmos() {
    if (Application.isPlaying) {
      Gizmos.color = Color.cyan;
      Gizmos.DrawSphere(new Vector3(noisyXPosition, 0f, 0f), 0.1f);
      Gizmos.color = Color.red;
      Gizmos.DrawSphere(new Vector3(Mathf.Sin(Time.time), 0f, 0f), 0.1f);
      Gizmos.color = Color.green;
      if (filter.StateMatrix != null) {
        Gizmos.DrawSphere(new Vector3(filter.StateMatrix[0, 0], 0f, 0f), 0.1f);
      }
    }
  }
}
