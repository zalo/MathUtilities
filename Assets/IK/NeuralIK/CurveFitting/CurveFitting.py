#! /usr/local/bin/python3
# Must have `numpy`, `tensorflow`, `keras`, and `zeromq` pip installed
from keras import optimizers
from keras.layers.advanced_activations import LeakyReLU, PReLU
from keras import losses
from keras.models import Sequential
from keras.layers import Dense
from keras import backend as K
import numpy as np
import random
import zmq

numEpochs = 10000; epochsPerIteration = 50; curEpoch = 0
context = zmq.Context()
socket = context.socket(zmq.PAIR)
socket.bind("tcp://*:12345")
socket.SNDTIMEO = 1000
socket.RCVTIMEO = 1000
socket.LINGER = 0

# fix random seed for reproducibility
np.random.seed(7)

# create and compile model
XY = [[], []]
model = Sequential()
model.add(Dense(16, input_dim=10, activation="linear"))
model.add(LeakyReLU(alpha=0.3))
model.add(Dense(16, activation="linear"))
model.add(LeakyReLU(alpha=0.3))
model.add(Dense(16, activation="linear"))
model.add(LeakyReLU(alpha=0.3))
model.add(Dense(10, activation="linear"))

model.compile(loss="mse", optimizer="adam", metrics=['accuracy'])

# the main training/simulation loop
while (curEpoch < numEpochs):
    #trigger Unity to generate a set of training samples
    socket.send_string("s")

    #split/parse received data into matrices and fit the network to it (skip if only received prediction query)
    data = str(socket.recv())[2:-1]
    dataStrings = data.split(";")
    if(len(dataStrings) > 1):
        XY[0] = []; XY[1] = []
        for i in range(len(dataStrings)-1):
            sampleStrings = dataStrings[i].split(",")
            XY[0].append([float(sampleStrings[0]),  float(sampleStrings[1]),  float(sampleStrings[2]),  float(sampleStrings[3]),  float(sampleStrings[4]),  float(sampleStrings[5]),  float(sampleStrings[6]),  float(sampleStrings[7]),  float(sampleStrings[8]),  float(sampleStrings[9])])
            XY[1].append([float(sampleStrings[10]), float(sampleStrings[11]), float(sampleStrings[12]), float(sampleStrings[13]), float(sampleStrings[14]), float(sampleStrings[15]), float(sampleStrings[16]), float(sampleStrings[17]), float(sampleStrings[18]), float(sampleStrings[19])])

        #fit the model for a few epochs
        model.fit(np.asarray(XY[0]), np.asarray(XY[1]), epochs=epochsPerIteration, batch_size=128, verbose=1)# initial_epoch = curEpoch)
        curEpoch += epochsPerIteration

    #visualize the prediction from the last sample
    visualizationStrings = dataStrings[-1].split(",")
    prediction = model.predict(np.asarray([[float(visualizationStrings[0]), float(visualizationStrings[1]), float(visualizationStrings[2]), float(visualizationStrings[3]), float(visualizationStrings[4]), float(visualizationStrings[5]), float(visualizationStrings[6]), float(visualizationStrings[7]), float(visualizationStrings[8]), float(visualizationStrings[9])]]))
    socket.send_string("v " + str(prediction[0][0]) + "," + str(prediction[0][1]) + "," + str(prediction[0][2]) + "," + str(prediction[0][3]) + "," + str(prediction[0][4]) + "," + str(prediction[0][5]) + "," + str(prediction[0][6]) + "," + str(prediction[0][7]) + "," + str(prediction[0][8]) + "," + str(prediction[0][9]))
