public class KalmanFilter {
  //The current state of the system as seen by the filter. x_k
  public float[,] StateMatrix;
  //How fucked up the state probably is. P_k
  public float[,] ErrorCovarianceMatrix;

  //The Kinematic Equations, predicts how the state would evolve naturally without input. F_k
  public float[,] StateTransitionMatrix;
  //Which things in the state matrix we're actually measuring. H_k
  public float[,] MeasurementMatrix;
  //How bad our prediction model is at telling us what is actually going on. Q_k
  public float[,] ProcessNoiseMatrix;
  //How much noise is inherent to the measurement (not what is actually happening). R_k
  public float[,] MeasurementNoiseMatrix;

  public KalmanFilter(float[,] InitialState, float[,] Measurement, float[,] ProcessNoise, float[,] MeasurementN, float initialError) {
    StateMatrix = InitialState;
    MeasurementMatrix = Measurement;
    ProcessNoiseMatrix = ProcessNoise;
    MeasurementNoiseMatrix = MeasurementN;

    //Initial Error measurement; 0f = slow initial convergence, 1f = fast initial convergence
    ErrorCovarianceMatrix = MatrixExtensions.identity(9).mult(initialError);
  }

  public void PredictState(float deltaTime) {
    StateTransitionMatrix = MakeStateTransitionMatrix(deltaTime);
    //Predict the State according to the Kinematic Equations in the StateTransition Matrix
    StateMatrix = StateTransitionMatrix.mult(StateMatrix);
    //Predict the Error according to the Kinematic Equations and the Process Noise
    ErrorCovarianceMatrix = (StateTransitionMatrix.mult(ErrorCovarianceMatrix).mult(StateTransitionMatrix.transpose())).add(ProcessNoiseMatrix);
  }

  //Predict the State according to the Kinematic Equations in the StateTransition Matrix without changing the state of the filter!
  public float[,] SafePredictState(float deltaTime) {
    return MakeStateTransitionMatrix(deltaTime).mult(StateMatrix);
  }

  public void UpdateState(float[,] newMeasurements) {
    //The Current Measurement
    float[,] CurrentObservation = MeasurementMatrix.mult(newMeasurements).add(MeasurementNoiseMatrix);
    //The difference (or "residual") between the predicted state and the measured state
    float[,] MeasurementResidualeMatrix = CurrentObservation.sub(MeasurementMatrix.mult(StateMatrix));
    //How fucked up that residual probably is
    float[,] ResidualCovarianceMatrix = ((MeasurementMatrix.mult(ErrorCovarianceMatrix)).mult(MeasurementMatrix.transpose())).add(MeasurementNoiseMatrix);
    //THE OPTIMAL KALMAN GAIN "choir of cherubs*
    float[,] OptimalKalmanGain = (ErrorCovarianceMatrix.mult(MeasurementMatrix.transpose())).mult(ResidualCovarianceMatrix.invert()); //oh shit need to invert a matrix

    //wait... all it's doing here is adding the measurement residual multiplied by the gain to the state
    //WHAT A FUCKIN GYP, ALL THIS TROUBLE, ALL THESE MATRICES... GAH.
    //Should have just used some for-loops
    StateMatrix = StateMatrix.add(OptimalKalmanGain.mult(MeasurementResidualeMatrix));

    //Update the fucked-upitude of the state
    float[,] GainTimesMeasurement = OptimalKalmanGain.mult(MeasurementMatrix);
    ErrorCovarianceMatrix = MatrixExtensions.identity(GainTimesMeasurement.GetLength(1)).sub(GainTimesMeasurement).mult(ErrorCovarianceMatrix);
  }

  public float[,] MakeStateTransitionMatrix(float deltaTime) {
    return new float[,]
     {{1f, 0f, 0f, deltaTime, 0f, 0f, 0.5f*deltaTime*deltaTime, 0f, 0f}, //XPos
			{0f, 1f, 0f, 0f, deltaTime, 0f, 0f, 0.5f*deltaTime*deltaTime, 0f}, //YPos
			{0f, 0f, 1f, 0f, 0f, deltaTime, 0f, 0f, 0.5f*deltaTime*deltaTime}, //ZPos
			{0f, 0f, 0f, 1f, 0f, 0f, deltaTime, 0f, 0f}, //XVel
			{0f, 0f, 0f, 0f, 1f, 0f, 0f, deltaTime, 0f}, //YVel
			{0f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, deltaTime}, //ZVel
			{0f, 0f, 0f, 0f, 0f, 0f, 1f, 0f, 0f}, //Constant XAccel
			{0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 0f}, //Constant YAccel
			{0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f}}; //Constant ZAccel
  }
}
