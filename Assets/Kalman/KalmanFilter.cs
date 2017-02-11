using UnityEngine;

public class KalmanFilter {
	//The current state of the system as seen by the filter. x_k
	public Matrix StateMatrix;
	//How fucked up the state probably is. P_k
	public Matrix ErrorCovarianceMatrix;

  //The Kinematic Equations, predicts how the state would evolve naturally without input. F_k
  public Matrix StateTransitionMatrix;
  //Which things in the state matrix we're actually measuring. H_k
  public Matrix MeasurementMatrix;
  //How bad our prediction model is at telling us what is actually going on. Q_k
  public Matrix ProcessNoiseMatrix;
  //How much noise is inherent to the measurement (not what is actually happening). R_k
  public Matrix MeasurementNoiseMatrix;

  public KalmanFilter(Matrix InitialState, Matrix Measurement, Matrix ProcessNoise, Matrix MeasurementN, float initialError) {
    StateMatrix = InitialState;
    MeasurementMatrix = Measurement;
    ProcessNoiseMatrix = ProcessNoise;
    MeasurementNoiseMatrix = MeasurementN;

    //Initial Error measurement; 0f = slow initial convergence, 1f = fast initial convergence
    ErrorCovarianceMatrix = new Matrix(9).mult(initialError);
  }

  public void PredictState(float deltaTime){
		StateTransitionMatrix = MakeStateTransitionMatrix(deltaTime);
		//Predict the State according to the Kinematic Equations in the StateTransition Matrix
		StateMatrix = StateTransitionMatrix.mult (StateMatrix);
		//Predict the Error according to the Kinematic Equations and the Process Noise
		ErrorCovarianceMatrix = (StateTransitionMatrix.mult (ErrorCovarianceMatrix).mult(StateTransitionMatrix.transpose())).add (ProcessNoiseMatrix);
	}

  //Predict the State according to the Kinematic Equations in the StateTransition Matrix without changing the state of the filter!
  public Matrix SafePredictState(float deltaTime) {
    return MakeStateTransitionMatrix(deltaTime).mult(StateMatrix);
  }

  public void UpdateState(Matrix newMeasurements){
		//The Current Measurement
		Matrix CurrentObservation = MeasurementMatrix.mult(newMeasurements).add (MeasurementNoiseMatrix);
		//The difference (or "residual") between the predicted state and the measured state
		Matrix MeasurementResidualeMatrix = CurrentObservation.sub (MeasurementMatrix.mult (StateMatrix));
		//How fucked up that residual probably is
		Matrix ResidualCovarianceMatrix = ((MeasurementMatrix.mult (ErrorCovarianceMatrix)).mult(MeasurementMatrix.transpose())).add (MeasurementNoiseMatrix);
		//THE OPTIMAL KALMAN GAIN "choir of cherubs*
		Matrix OptimalKalmanGain = (ErrorCovarianceMatrix.mult (MeasurementMatrix.transpose())).mult(ResidualCovarianceMatrix.invert()); //oh shit need to invert a matrix

		//wait... all it's doing here is adding the measurement residual multiplied by the gain to the state
		//WHAT A FUCKIN GYP, ALL THIS TROUBLE, ALL THESE MATRICES... GAH.
		//Should have just used some for-loops
		StateMatrix = StateMatrix.add (OptimalKalmanGain.mult (MeasurementResidualeMatrix));

		//Update the fucked-upitude of the state
		Matrix GainTimesMeasurement = OptimalKalmanGain.mult (MeasurementMatrix);
		ErrorCovarianceMatrix = (new Matrix(GainTimesMeasurement.width).sub (GainTimesMeasurement)).mult(ErrorCovarianceMatrix);
	}

	public Matrix MakeStateTransitionMatrix(float deltaTime){
    return new Matrix(new float[,]
		 {{1f, 0f, 0f, deltaTime, 0f, 0f, 0.5f*deltaTime*deltaTime, 0f, 0f}, //XPos
			{0f, 1f, 0f, 0f, deltaTime, 0f, 0f, 0.5f*deltaTime*deltaTime, 0f}, //YPos
			{0f, 0f, 1f, 0f, 0f, deltaTime, 0f, 0f, 0.5f*deltaTime*deltaTime}, //ZPos
			{0f, 0f, 0f, 1f, 0f, 0f, deltaTime, 0f, 0f}, //XVel
			{0f, 0f, 0f, 0f, 1f, 0f, 0f, deltaTime, 0f}, //YVel
			{0f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, deltaTime}, //ZVel
			{0f, 0f, 0f, 0f, 0f, 0f, 1f, 0f, 0f}, //Constant XAccel
			{0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 0f}, //Constant YAccel
			{0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f}}); //Constant ZAccel
	}
}
