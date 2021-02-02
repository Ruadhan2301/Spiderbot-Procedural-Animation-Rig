Hello Friends!

I have created here a general-case implementation of my Procedural Animation rig for spider-tanks.

I provide it here for your use.

There are however some things that may need explaining.

----------
Setup
----------

As my examples show, there's a standard setup.
Each leg consists of six critical parts in a heirarchy.
Hip Joint -> Thigh -> Knee Joint -> Shin -> Ankle Joint -> Foot

I provide the Joint Name inputs in the Inspector so that if you choose to name them something differently it will work, 
	however you still need to consistently call the other parts Thigh, Shin and Foot

The code uses the lengths of the Thigh, Shin and Foot to calculate its movements, so you will always need the part.
However it only uses the Z Scale.
For my own purposes, I treat them as a skeleton and simply turn off the Mesh Renderer when I attach 3D models to the rig.

Essentially, if you make the skeleton the right shape, the code will adapt to support it.

-------------
The Inspector
-------------

I've split up the inspector into appropriate sections for the various things that can be done.

- Parts -
Leg Roots - A list of the root parts of each leg.
Hip Joint Name - The name of the hip joint.
Knee Joint Name - The name of the knee joint.
Ankle Joint Name - The name of the ankle joint.

- Gait Pattern -
Walk Pattern - This is the sequence of the legs. The numbers are the indexes of the legs (check the Leg Roots list if you're not sure)
	You can put them together in groups, or you can simply have them in sequence.
	0,1,2,3 - Sequential
	03,12 - Paired opposing legs
	01,23 - Alternating front/back
	** Note: This cannot be altered at runtime.

Gait Overlap - Typically just leave this as zero, but sometimes you may want to have the steps overlap one another. 
	Basically this is a value from 0 to 1 governing how much overlap the steps have.
	Most animals will have some overlap though.
	** Note: This feature is not fully implemented. 
	While it can handle mid-cycle gait-overlaps, it doesn't overlap between cycles, 
		so each cycle you will always start with only one foot moving.
	
- Stance -
Stance - The vertical offset for the pelvis
Gait Spread - How far apart the leg stands from the pelvis.
	** Note: This cannot be altered at runtime.
Gait Length - How long the steps are.

- Motion Control -
Walk Speed - Animation speed.
Turn Speed - How fast the pelvis turns towards Face Direction

Move Direction - An input for the direction of travel in world-space
Turn Direction - An input for the facing direction, this does not have to align with move-direction. 


I have also provided a simple Wander script which hooks into the main control script.
Feel free to have a play with both.


----------
Crediting
----------

I don't much mind what you do with this project, but please credit me as appropriate!
