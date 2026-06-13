Collar and Leash System
=======================

Thank you for purchasing TapGhoul's Collar and Leash System!
Don't forget to leave a rating at https://gumroad.tapg.dev/l/collar-system, and join the discord via https://tapg.dev/ if you need any further help!

## Setup guide

To set this up, simply drop the given prefab under `Prefabs/DefaultCollar.prefab` into the scene.

## Customization

To switch out the collar material, simply drop the material under `Models/Set X Material` onto the collar. You are also free to make your own materials, or even models!

To add a leash handle model (for example, a coloured ball), simply add an object as a child of `DefaultCollar/LeashHandle`.
At the end of this, you should have something like `DefaultCollar/LeashHandle/MyCustomMesh`.
Make sure to remove any colliders on this mesh, or it could prevent being able to pick up the leash hande in desktop mode!

To add a leash material, either modify `Models/Leash Material`, or modify the `LineRenderer` component on the main collar system.
You can modify almost any property of the line renderer you want to modify its behaviour!

To modify the thickness of the leash, just right click on the dot on the "Width" graph and hit "Edit Key..." to set your width.

To change the behavior of the collar, you have a number of settings on the main collar system! See below for what they are:

## Behaviour Settings

| Option              | Purpose                                                                                                    |
|---------------------|------------------------------------------------------------------------------------------------------------|
| Leash Length        | How long the leash is                                                                                      |
| Handle Spawn Offset | Once you attach the leash to someone, how far away from their neck will the handle spawn?                  |
| Max Neck Distance   | How close you need to be to someones neck for the collar to attach to it                                   |
| Respawn To Unleash  | Allow someone to escape by hitting respawn - by default, the only way to get out on your own is to rejoin! |

Please do not modify the references - they are critical to having the system work!

## Important notes

To make sure everything works as intended, make sure the following is all set:
- The Line Renderer on the main system is disabled
- The LeashHandle is disabled
- The Line Renderer's Positions Size is at least 2 (they can be anything), but isn't too high. The default of 32 is probably fine!
