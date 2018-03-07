using SimpleMatrices;
public class KalmanFilter {
  //The current state of the system as seen by the filter. x_k
  public double[,] StateMatrix;
  //How fucked up the state probably is. P_k
  public double[,] ErrorCovarianceMatrix;

  //The Kinematic Equations, predicts how the state would evolve naturally without input. F_k
  public double[,] StateTransitionMatrix;
  //Which things in the state matrix we're actually measuring. H_k
  public double[,] MeasurementMatrix;
  //How bad our prediction model is at telling us what is actually going on. Q_k
  public double[,] ProcessNoiseMatrix;
  //How much noise is inherent to the measurement (not what is actually happening). R_k
  public double[,] MeasurementNoiseMatrix;

  public KalmanFilter(double[,] InitialState, double[,] Measurement, double[,] ProcessNoise, double[,] MeasurementN, double initialError) {
    StateMatrix = InitialState;
    MeasurementMatrix = Measurement;
    ProcessNoiseMatrix = ProcessNoise;
    MeasurementNoiseMatrix = MeasurementN;

    //Initial Error measurement; 0.0 = slow initial convergence, 1.0 = fast initial convergence
    ErrorCovarianceMatrix = MatrixExtensions.identity(9).mult(initialError);
  }

  public void PredictState(double deltaTime) {
    StateTransitionMatrix = MakeStateTransitionMatrix(deltaTime);
    //Predict the State according to the Kinematic Equations in the StateTransition Matrix
    StateMatrix = StateTransitionMatrix.mult(StateMatrix);
    //Predict the Error according to the Kinematic Equations and the Process Noise
    ErrorCovarianceMatrix = (StateTransitionMatrix.mult(ErrorCovarianceMatrix).mult(StateTransitionMatrix.transpose())).add(ProcessNoiseMatrix);
  }

  //Predict the State according to the Kinematic Equations in the StateTransition Matrix without changing the state of the filter!
  public double[,] SafePredictState(double deltaTime) {
    return MakeStateTransitionMatrix(deltaTime).mult(StateMatrix);
  }

  public void UpdateState(double[,] newMeasurements) {
    //The Current Measurement
    double[,] CurrentObservation = MeasurementMatrix.mult(newMeasurements).add(MeasurementNoiseMatrix);
    //The difference (or "residual") between the predicted state and the measured state
    double[,] MeasurementResidualeMatrix = CurrentObservation.sub(MeasurementMatrix.mult(StateMatrix));
    //How fucked up that residual probably is
    double[,] ResidualCovarianceMatrix = ((MeasurementMatrix.mult(ErrorCovarianceMatrix)).mult(MeasurementMatrix.transpose())).add(MeasurementNoiseMatrix);
    //THE OPTIMAL KALMAN GAIN "choir of cherubs*
    double[,] OptimalKalmanGain = (ErrorCovarianceMatrix.mult(MeasurementMatrix.transpose())).mult(ResidualCovarianceMatrix.invert()); //oh shit need to invert a matrix

    //wait... all it's doing here is adding the measurement residual multiplied by the gain to the state
    //WHAT A FUCKIN GYP, ALL THIS TROUBLE, ALL THESE MATRICES... GAH.
    //Should have just used some for-loops
    StateMatrix = StateMatrix.add(OptimalKalmanGain.mult(MeasurementResidualeMatrix));

    //Update the fucked-upitude of the state
    double[,] GainTimesMeasurement = OptimalKalmanGain.mult(MeasurementMatrix);
    ErrorCovarianceMatrix = MatrixExtensions.identity(GainTimesMeasurement.GetLength(1)).sub(GainTimesMeasurement).mult(ErrorCovarianceMatrix);
  }

  public double[,] MakeStateTransitionMatrix(double deltaTime) {
    return new double[,]
     {{1.0, 0.0, 0.0, deltaTime, 0.0, 0.0, 0.5*deltaTime*deltaTime, 0.0, 0.0}, //XPos
			{0.0, 1.0, 0.0, 0.0, deltaTime, 0.0, 0.0, 0.5*deltaTime*deltaTime, 0.0}, //YPos
			{0.0, 0.0, 1.0, 0.0, 0.0, deltaTime, 0.0, 0.0, 0.5*deltaTime*deltaTime}, //ZPos
			{0.0, 0.0, 0.0, 1.0, 0.0, 0.0, deltaTime, 0.0, 0.0}, //XVel
			{0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, deltaTime, 0.0}, //YVel
			{0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, deltaTime}, //ZVel
			{0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0}, //Constant XAccel
			{0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0}, //Constant YAccel
			{0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0}}; //Constant ZAccel
  }
}
