using UnityEngine;

public class KalmanDemo : MonoBehaviour {
  KalmanFilter filter;

  //Initialize the filter
  void Start() {
    filter = new KalmanFilter(new double[,]
         {{0.0}, //XPos
			    {0.0}, //YPos
			    {0.0}, //ZPos
			    {0.0}, //XVel
			    {0.0}, //YVel
			    {0.0}, //ZVel
			    {0.0}, //XAccel
			    {0.0}, //YAccel
			    {0.0}, //ZAccel
         },

         //Measurement Matrix
         //Tells the filter which of the above we're actually sensing
         //Turns out you can just straight up comment out the lines that 
         //you want the filter to solve for
         new double[,]
          {{1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}, //XPos
			    {0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}, //YPos - Not Measured!
			    {0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}, //ZPos - Not Measured!
			    {0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0}, //XVel - Not Measured!
			    {0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0}, //YVel - Not Measured!
			    {0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0}, //ZVel - Not Measured!
			    {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0}, //XAccel
          {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0}, //YAccel - Not Measured!
          {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0} //ZAccel - Not Measured!
          },

         //Process Noise; how much each value will deviate from the predicted value
         //Quick gaussian distribution intuition
         //If the ProcessNoise is 0.1 then you're saying that
         //68% of the time, the actual system will deviate less than sqrt(0.1) *from the predicted value*
         //and 95% of the time, less than 2*sqrt(0.1) etc.
         //It's a measure of how good your StateTransitionMatrix is
         //Ours sucks since it doesn't factor in that a meddling sine wave is moving the tracker around
         //So our numbers need to be kind of high
         new double[,]
          {{.000025, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}, //XPos
			    {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}, //YPos
			    {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}, //ZPos
			    {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}, //XVel
			    {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}, //YVel
			    {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}, //ZVel
			    {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.025, 0.0, 0.0}, //Constant XAccel
			    {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}, //Constant YAccel
			    {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0} //Constant ZAccel
          },

         //Same as above, except now it's the measurement deviating from actual position (not the predicted position)
         new double[,]
          {{60.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}, //XPos
			  //{0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}, //YPos
			  //{0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}, //ZPos
			  //{0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}, //XVel
			  //{0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}, //YVel
			  //{0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}, //ZVel
			    {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0005, 0.0, 0.0} //XAccel
			  //{0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}, //YAccel
			  //{0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0} //ZAccel
          },

         //Initial Error measurement; 0.0 = slow initial convergence, 1.0 = fast initial convergence
         0.1
     );
  }

  double noisyXPosition = 0.0;
  void Update() {
    filter.PredictState(Time.deltaTime);

    //Dummy Data : Generate a very noisy position estimate
    noisyXPosition = Mathf.Sin(Time.time) + Random.Range(-0.5f, 0.5f);
    filter.UpdateState(new double[,]
     {{noisyXPosition}, //Noisy Position
			{0.0},
      {0.0},
      {0.0},
      {0.0},
      {0.0},
      {-Mathf.Sin(Time.time)}, //Flawless acceleration
			{0.0},
      {0.0}});

    double oldPos = filter.StateMatrix[0, 0];
    for (int i = 1; i < 10; i++) {
      double[,] PredictedState = filter.SafePredictState(0.1 * i);
      Debug.DrawLine(new Vector3((float)oldPos, i * 0.02f, 0f), new Vector3((float)PredictedState[0, 0], (i + 1) * 0.02f, 0f), Color.green);
      oldPos = PredictedState[0, 0];

      Debug.DrawLine(new Vector3(Mathf.Sin((Time.time + (i * 0.1f))), i * 0.02f, 0f), 
                     new Vector3(Mathf.Sin((Time.time + ((i + 1f) * 0.1f))), (i + 1) * 0.02f, 0f), Color.red);
    }
  }

  //Draw the state of the system and the filter
  void OnDrawGizmos() {
    if (Application.isPlaying) {
      Gizmos.color = Color.cyan;
      Gizmos.DrawSphere(new Vector3((float)noisyXPosition, 0f, 0f), 0.1f);
      Gizmos.color = Color.red;
      Gizmos.DrawSphere(new Vector3(Mathf.Sin(Time.time), 0f, 0f), 0.1f);
      Gizmos.color = Color.green;
      if (filter.StateMatrix != null) {
        Gizmos.DrawSphere(new Vector3((float)filter.StateMatrix[0, 0], 0f, 0f), 0.1f);
      }
    }
  }
}
