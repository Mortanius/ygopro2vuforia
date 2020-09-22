# ygopro v2 in Unity with Augmented Reality using Vuforia

This branch was created in order to implement an Augmented Reality interface, using the core implementation of ygopro v2 in Unity (a Card Game in AR).
This feature was designed for learning purposes as a final project for a school subject (Virtual and Mixed Reality, on a Computer Science course).
The feature has been developed by using the Vuforia Augmented Reality SDK (along with the Unity Game Engine which the original project already used).

The original GitHub repository is located at https://github.com/lllyasviel/YGOProUnity_V2, but this repository was cloned from another that made compiling to Android possible.

# Current implementation capabilities
- Rendering the game field on an image target.
- A "virtual" card: a game objected consisting of two image targets (front and back). It is used to select a card in possession and place it on the field.
- The rest of the gameplay mechanics is yet to be thought out to work with AR.

# You have just found ygopro v2 in Unity 3D!

YGOPro v2 is a card simulation game created in Unity Engine, with same protocol to ygopro.

The game is now being tested in china now, with at least *100,000* users ( about *30%* active users ), which is calculated from the downloads of our weekly updating packages.

We use Yu-Gi-Oh card game only to test our engine, and the game is not for commercial use. When our card game engine is finally finished in about several years, all the contents about Yu-Gi-Oh will be deleted.

# How to compile the game?

1. Download Unity 5.6.7 (https://unity3d.com/get-unity/download/archive).

2. Clone the repository.

3. Double click Assets\main.unity to open the solution.

# How to compile the ocgcore.dll?

*In most case you do not need to care about the ocgcore.dll.*

1. Double click the **YGOProUnity_V2/AI_core_vs2017solution/core.sln**

2. build the c++ solution in x64 and release mode and you get the **ocgcore.dll**

3. copy it into **YGOProUnity_V2\Assets\Plugins**
