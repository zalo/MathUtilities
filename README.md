MathUtilities
=====================

A grab bag of some of the neat math and physics tricks that I've amassed over the last few years, implemented in Unity's C#.  You're free to use the code here however you like.


##Kabsch
<img src="http://i34.photobucket.com/albums/d144/Zalo10/Kabsch_zpshxn8kz7p.gif">

An algorithm that can take in an arbitrary set of point-pairs and find the globally optimal translation and rotation to minimize the distance between those point pairs.  Incredibly useful, and very cheap.   Uses Matthias Muller's iterative optimal rotation solver in place of SVD, as outlined here: https://animation.rwth-aachen.de/media/papers/2016-MIG-StableRotation.pdf


##[Stereographic/Fisheye Camera](https://en.wikipedia.org/wiki/Stereographic_projection)
<img src="http://i34.photobucket.com/albums/d144/Zalo10/StereographicCamera2_zpsingx8wec.gif">

A prefab that concatenates and warps the images from four cameras into one 180 degree fisheye view, projected stereographically.


##[Kalman Filter](https://en.wikipedia.org/wiki/Kalman_filter#Details)
<img src="http://i34.photobucket.com/albums/d144/Zalo10/kalman2_zps4zqhnqcj.gif">

A textbook implementation of a Kalman filter (transcribed from wikipedia)   Kalman filters are a form of bayesian filtering; they are capable of taking in information from multiple sources and "fusing" them into a signal that is cleaner/more accurate than any of the constituent signals.  A properly tuned Kalman filter is the mathematically "optimal" technique for turning noisy data into clean data in real time (as long as the noise follows a gaussian distribution and the data varies linearly).  In practice one must deal with biases and non-linearly varying quantities.


##Matrix Class
A generic matrix class and a set of basic matrix operations (multiplication, addition, subtraction, inversion, cholesky decomposition, and more).  Written to support the implementation of the Kalman Filter.   Usage of any of these operations will allocate a new array (garbage), so be careful about using this on performance constrained systems.


##Constraints/Inverse Kinematics
<img src="http://i34.photobucket.com/albums/d144/Zalo10/LimitedJoint_zpslapag2ch.gif"> <img src="http://i34.photobucket.com/albums/d144/Zalo10/Finger_zps3cugukbj.gif">

A set of constraint functions that can be used to build an iterative inverse kinematics solver.


##Verlet Rigidbody
<img src="http://i34.photobucket.com/albums/d144/Zalo10/Verlet_zpsvzicq1is.gif">

A minimal script for simulating a rigid body's motion using time-corrected verlet integration (without using a Rigidbody component).


##Other experiments:
###Linear Blend Skinning
A reference implementation that demonstrates how to apply bone motions to a model using the data contained within a skinned mesh renderer.   As they say, there is more than one way to skin a mesh.

###Thick Tesellated Plane Generator
Useful for generating solid meshes that are essentially deformed planes (shapes that are common in optics).

###Bundle Adjustment
My attempts at implementing [Bundle Adjustment](https://en.wikipedia.org/wiki/Bundle_adjustment) (an algorithm which attempts to solve for the relative motion between two camera images, given the motion of a set of feature-points between the images).  It will converge when either position or rotation adjustment is applied, but not when they are applied simultaneously (not sure why...)

###Torque Extension for HingeJoints
This adds a function to apply torque "through" HingeJoints and to Rigidbodies in general.

###Acceleration Dampened Rigidbodies
Attempts to simulate soft-spongy contact by damping the accelerations that are be applied to an object.  Phenomena like friction cause it to exhibit strange artifacts...

###2D Platforming Character
<img src="http://i34.photobucket.com/albums/d144/Zalo10/platformer_zpsaszusawb.gif">

Uses a neat trick where, if the anchor of a spring joint is moved, both connected rigidbodies are physically affected. This allows one to easily simulate "muscles".

###Rolling Cubes
<img src="http://i34.photobucket.com/albums/d144/Zalo10/rolling_zpsw1tj8dks.gif">

Fun for the whole family!