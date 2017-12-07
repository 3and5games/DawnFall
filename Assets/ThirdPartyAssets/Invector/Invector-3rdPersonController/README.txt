Thank you for support our asset!

*IMPORTANT* This asset requires Unity 5.2.0 or higher.

If you have any question about how it works or if you are experiencing any trouble, 
feel free to email us at: inv3ctor@gmail.com
Please do not Upload or share this asset as a Package without permission.

If you downloaded this asset illegally for studies or prototype purposes, 
please reconsider purchase if you want to publish your work, you can buy on the AssetStore 
or send us a email and we can figure something out, you can even post your work on the Oficial Thread, 
will be happy to help and advertise your game.

ASSETSTORE: https://www.assetstore.unity3d.com/en/#!/content/44227
FORUM: http://forum.unity3d.com/threads/third-person-controller-template-by-invector.349124/
YOUTUBE: https://www.youtube.com/channel/UCSEoY03WFn7D0m1uMi6DxZQ
WEBSITE: http://inv3ctor.wix.com/invector

Invector Team - 2015

Changelog v1.1d small HOTFIX 17/01/2016

Fixed:
- Character Template missing Animator 

-----------------------------------------------------------------------------------------------------

Changelog v1.1c HOTFIX 12/01/2016

Fixed:
- 2 Actions been activate at the same time, causing the Player to glitch between states

Changes:
- QuickStop and QuickTurn now are actions and are located in the Action State on the Animator
- Grounded State remade to handle Strafe animations 
- Minor changes at the TPMotor, TPAnimator and TPController to prepare the AI on the next update
- Better transition from Strafe to Free locomotion, and the a smoother Quickturn 
- ICharacter Interface created to handle the Ragdoll behaviour for both Player and AI 
- Improve the verification for quickStop and quickTurn

Add:
- Locomotion Type, now you can choose between Strafe Only, Free Only or Free with Strafe
- Crouch Strafe locomotion
- New Layer for Melee 3 Attack Combo, this layers comes without animations at this version, but we will add some example animations on the next version, along with the AI

-----------------------------------------------------------------------------------------------------

Changelog v1.1b 30/12/2015

Fixed:
- Message displaying change of input between mobile/pc when clicking 
- Fixed the error about bounds on the FootStep when walking between Terrain and Mesh 

Changes:
- TPCamera & HUD isoleted from the controller, now this components are modular and work individually
- Controller now works as a Prefab, can be Instantiated and will automatically find a Camera or the HUD component on the scene
- Culling fade now goes into the character instead of the Camera, also modular just like the Ragdoll of Footstep component.
- Strafe animations re-configured with better transitions
- Several TPCamera improvements of CameraState transitions and Culling 
- Improvements and changes into the TPAnimator and TPMotor, now we separated on methods that controls animations and locomotion behaviour from each other

Add: 
- GameController with SpawnPoint system 
- Non Root-motion movement add, with separated Directional Movement Speed and Strafing Speed
- Rotate by World bool add into the Controller for better locomotion when playing on Isometric Mode
- List of Ignore Tags for the Ragdoll Component, keep Weapons or Acessories that are children of the Player with the correctly rotation
- New Camera Feature - Fixed Point or Multiple Points (Resident Evil oldschool camera's style)
- Trigger System to change CameraPoints
- New Demo Scene with the V Mansion showing the new CameraMode Fixed Point
- Simple Door example

-----------------------------------------------------------------------------------------------------

Changelog HOTFIX v1.1a 10/11/2015

Fix Bugs:
- On the MobileScene the gameObject MobileControls need to be below the HUD gameObject on the Hierarchy (otherwise it will stop respond after the ragdoll turn on)
- Fix the Jump behaviour if you jump right on the edge, the character falls and jump again (mecanim issue)
- Fix the trajectory of the jump in case of high value of height and force forward 
- v1.1 shipped with a aditional XInput plugin, we removed and now you can export Mobile builds normally

Add: 
- The character now can jump while Aiming (just a condition add in the Animator)
- Add 'quick change scene' buttons on the demo scenes
- Add a Camera Fade effect to hide the character when is too close
- Add Clip Plane Margin for better clipping planes to the Camera
- Add InvectorJoystick.cs is just a quickfix to the Square touch area from the CrossPlataformInput to a Circular touch area
- Add the original Standard Assets CrossPlataformInput to improve compatibility with other assets and avoid errors of scripts duplicity
- Add back the AntiAliasing in the Camera

-----------------------------------------------------------------------------------------------------

Changelog v1.1 25/10/2015

Fix Bugs:
- Fix the vibrating upperbody animation when in Aiming mode after the ragdoll was activated (bug found by Chrisb3D, thanks!)
- Fix lockPlayer after roll through the Crouch Area and rarely lock the player on the exit (bug found by tmmandk, thank you sir!)
- Fix minor bug holding shift and enter on Aim mode still drains stamina (reported by Steel Grin, thanks!)

Changes:
- Script "FindTarget" changed to "TriggerAction" and add AutoActions bool
- Major changes on TPCamera script
- Update XInput plugins to the last version with x86 and x86_x64 support
- change CheckForwardAction() and create a CheckActionObject on ThirdPersonMotor
- remove CheckAutoCrouch from the ThirdPersonController and put on ThirdPersonMotor
- improved Culling of the camera 
- improved Ragdoll transition when back to player 
- improved footstep system syncronization
- improved Ground Detection
- now users need to set up layers manually to improve compatibility with others projects

Add:
- Add float spine curvature to set up how much the spine will curve while on Aiming mode
- Add support to realtime change input to Mobile (thanks to Xander Davis)
- Add support to MFi iOS gamepad (thanks to Xander Davis)
- Add Aiming button on Mobile Touch Controls
- Add AutoActions for TriggerActions automatically do an Action without the need of input
- Add Tag for AutoCrouch (use on a simple trigger object)
- Add FootStep support to Play a specific material on objects with multiple materials
- Add feature Jump 
- Add feature Camera Scroll Zoom (Mouse only)
- Add feature FixedAngle on CameraState
- Add DrawGizmos for Debug to verify the Raycasts on Motor (head, actions, stepup, stopmove)
- Add 2.5D Scene Demo
- Add Topdown Scene Demo
- Add Isometric Point&Click Scene Demo


