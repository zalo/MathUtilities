MathUtilities
=====================

A grab bag of some of the neat math and physics tricks that I've amassed over the last few years, implemented in Unity's C#.  You're free to use the code here however you like.


## Kabsch
<img src="http://i34.photobucket.com/albums/d144/Zalo10/Kabsch_zpshxn8kz7p.gif">

An algorithm that can take in an arbitrary set of point-pairs and find the globally optimal translation and rotation to minimize the distance between those point pairs.  Incredibly useful, and very cheap.   Uses Matthias Muller's iterative optimal rotation solver in place of SVD, as outlined here: https://animation.rwth-aachen.de/media/papers/2016-MIG-StableRotation.pdf


## [Stereographic/Fisheye Camera](https://en.wikipedia.org/wiki/Stereographic_projection)
<img src="http://i34.photobucket.com/albums/d144/Zalo10/StereographicCamera2_zpsingx8wec.gif">

A prefab that concatenates and warps the images from four cameras into one 180 degree fisheye view, projected stereographically.


## Verlet Softbody, Rigidbody, and Chain
<img src="http://i34.photobucket.com/albums/d144/Zalo10/softSphere_zpsle1ca59w.gif"> <img src="http://i34.photobucket.com/albums/d144/Zalo10/Verlet_zpsvzicq1is.gif">

Numerous examples of using verlet integration ([a subset of Position Based Dynamics](http://matthias-mueller-fischer.ch/publications/posBasedDyn.pdf)) to simulate soft/rigid bodies, cloth, and particle linkage chains.


## [Kalman Filter](https://en.wikipedia.org/wiki/Kalman_filter#Details)
<img src="http://i34.photobucket.com/albums/d144/Zalo10/kalman2_zps4zqhnqcj.gif">

A textbook implementation of a Kalman filter (transcribed from wikipedia)   Kalman filters are a form of bayesian filtering; they are capable of taking in information from multiple sources and "fusing" them into a signal that is cleaner/more accurate than any of the constituent signals.  A properly tuned Kalman filter is the mathematically "optimal" technique for turning noisy data into clean data in real time (as long as the noise follows a gaussian distribution and the data varies linearly).  In practice one must deal with biases and non-linearly varying quantities.


## Matrix Class
A generic matrix class and a set of basic matrix operations (multiplication, addition, subtraction, inversion, cholesky decomposition, and more).  Written to support the implementation of the Kalman Filter.   Usage of any of these operations will allocate a new array (garbage), so be careful about using this on performance constrained systems.


## Constraints/[Inverse Kinematics](http://www.elysium-labs.com/robotics-corner/learn-robotics/introduction-to-robotics/kinematic-jacobian/)
<img src="http://i34.photobucket.com/albums/d144/Zalo10/LimitedJoint_zpslapag2ch.gif"> <img src="http://i34.photobucket.com/albums/d144/Zalo10/Finger_zps3cugukbj.gif">

A set of constraint functions that can be used to build [an iterative inverse kinematics solver.](https://makeshifted.itch.io/dexter-arm-ik)



## Other experiments:

### Linear Blend Skinning
A reference implementation that demonstrates how to apply bone motions to a model using the data contained within a skinned mesh renderer.   As they say, there is more than one way to skin a mesh.


### Per-Pixel Texture Reprojection
Demonstrates how to set up a shader to project a texture onto scene geometry using a (disabled) camera as the projector frustum.  Useful for illusions.


### Minkowski Difference Visualizer
Uses a compute shader to draw arbitrary 2D Minkowski "Differences" in real time.  The Minkowski Sum (and its modification, the "Minkowski Difference") is a core operation in collision detection.  This concept allows for a fully generalized way of determining whether any two objects are intersecting, and what the minimum translation is that separates them.  The key is determining whether the origin (of the coordinate system used in the operation) is inside of the resulting Minkowski Difference shape.   GJK is a collision detection technique that implements this check quickly for convex objects, with only a few samples of the implicit Minkowski Difference.


### Bidirectional Raycasting
Operates like a standard raycast, but returns both entry and exit information.  Useful for building wires that wrap around the environment and bullet entry/exit effects.


### Thick Tesellated Plane Generator
Useful for generating solid meshes that are essentially deformed planes (shapes that are common in optics).


### Capsule Mesh Generator
Useful for generating capsule meshes with an arbitrary length and radius.


### Raytraced Sphere
A little shader that raytraces a pixel-perfect sphere on your object/billboard, for when you want to be pixel-bound rather than vertex bound...


### [Haptic Probe](http://i.imgur.com/Ljy3U8y.gif)
An example implementing a haptic probe, where the force on the effector of the haptic controller is equal to the linear and angular displacement of the green cube to the gray one.


### Bundle Adjustment
My attempts at implementing [Bundle Adjustment](https://en.wikipedia.org/wiki/Bundle_adjustment) (an algorithm which attempts to solve for the relative motion between two camera images, given the motion of a set of feature-points between the images).  It will converge when either position or rotation adjustment is applied, but not when they are applied simultaneously (not sure why...)


### Torque Extension for HingeJoints
This adds a function to apply torque "through" HingeJoints and to Rigidbodies in general.


### Acceleration Dampened Rigidbodies
Attempts to simulate soft-spongy contact by damping the accelerations that are be applied to an object.  Phenomena like friction cause it to exhibit strange artifacts...


### 2D Platforming Character
<img src="http://i34.photobucket.com/albums/d144/Zalo10/platformer_zpsaszusawb.gif">

Uses a neat trick where, if the anchor of a spring joint is moved, both connected rigidbodies are physically affected. This allows one to easily simulate "muscles".


### Rolling Cubes
<img src="http://i34.photobucket.com/albums/d144/Zalo10/rolling_zpsw1tj8dks.gif">

Fun for the whole family!
