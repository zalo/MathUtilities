#! /usr/local/bin/python3
# Must have `numpy`, `tensorflow`, `keras`, and `zeromq` pip installed
from keras import optimizers
from keras import losses
from keras.models import Sequential
from keras.layers import Dense
from keras import backend as K
import numpy as np
import random
import zmq

numEpochs = 3000; epochsPerIteration = 10; curEpoch = 0
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
model.add(Dense(5, input_dim=2, activation="relu"))
model.add(Dense(5, activation="relu"))
model.add(Dense(2, activation="linear"))

def FKLoss(y_true, y_pred):
    tip_true = K.concatenate([K.sin(y_true[0]) + K.sin(y_true[0] + y_true[1]), K.cos(y_true[0]) + K.cos(y_true[0] + y_true[1])])
    tip_pred = K.concatenate([K.sin(y_pred[0]) + K.sin(y_pred[0] + y_pred[1]), K.cos(y_pred[0]) + K.cos(y_pred[0] + y_pred[1])])
    return losses.mean_squared_error(tip_true, tip_pred)

model.compile(loss=FKLoss, optimizer="adam", metrics=['accuracy'])

# the main training/simulation loop
while (curEpoch < numEpochs):
    # generate a series of uniform random positions to simulate
    output = "s "
    for i in range(1023):
         output += str(random.uniform(-2.0, 2.0)) + "," + str(random.uniform(-1.0, 2.0)) + ";"
    socket.send_string(output[:-1], 0)

    #split/parse received data into matrices and use as our current data
    data = str(socket.recv())[2:-1]
    dataStrings = data.split(";")
    XY[0] = []; XY[1] = []
    for i in range(len(dataStrings)-2):
        sampleStrings = dataStrings[i].split(",")
        XY[0].append([float(sampleStrings[0]), float(sampleStrings[1])])
        XY[1].append([float(sampleStrings[2]), float(sampleStrings[3])])

    #visualize the prediction from the last sample
    visualizationStrings = dataStrings[-1].split(",")
    prediction = model.predict(np.asarray([[float(visualizationStrings[0]), float(visualizationStrings[1])]]))
    socket.send_string("v " + str(prediction[0][0]) + "," + str(prediction[0][1]))

    #fit the model for 10 epochs
    model.fit(np.asarray(XY[0]), np.asarray(XY[1]), epochs=epochsPerIteration, batch_size=256, verbose=1)#, initial_epoch = curEpoch)
    curEpoch += epochsPerIteration
