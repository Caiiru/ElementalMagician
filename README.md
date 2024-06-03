# Branch Development - Caiiru

## Game Design Document

- **Version** _1.0_


## Tasks:

- [X] Remodulate player movement
- [X] Create Entity
- [X] Create player Aim
- [ ] Create Skill System:
    - [X] Input Element from Keyboard
    - [X] Display Selected Element <- WIP
    - [X] Display 2 Elements Selected
    - [X] Create The List With Recipes For Each Skill
    - [X] Use Skill when player input the shot button
- [X] First Skill: Fireball   



## How Player Work:

 In godot, I'm trying to use a pattern that use in unity: Separate all player's skills and movement in different scripts. 
 Like:
- Player (vertical) Movement = one script
- Player Jump = another script

But, godot isnt like Unity in the way that I can add one or more scripts at the same object. Because of that godot behaviour, the player was made in this way: 

The scripts for each player skills/controller is one node that is child of Player's CharacterBody2D and each Node can modify the actor velocity.

So, player is like an "scene" and has childs: 
- PlataformerMovement
- PlayerJump
- PlayerEntity
- Player Aim


---------------

### Plataformer Movement 

This script is very simple: 

    - In _process (update) I have the function "GatherInput" thats pick the player inputs based on the Axis and set the last direction that player walked. The "Move" func just move the player around based on the "stats resource" speed 

### Player Jump
This one script has more variables and functions that movement script. The resume of how it works is:

    One func handle the inputs (like the movement script). HandleJump func checks if the player can jump 
    ExecuteJump do what the name means to do and reset the conditional variables for player jump
    HandleGravity func handles the gravity and the player's "fall" velocity

### Entitys and Player Entity
Entity Script is the script that all things alive in the game will have in common. This is an abstract script that objects, player, enemys will be a child.

The Entity Script need an "Entity Resource" (Like Scriptable Object in Unity)

So, entity resource is a data class that have the current HP and the max HP of an entity

#### Entity functionalities: 
- TakeDamage
- isDead
- Cure
- Die
- Get Health

*Every functionality do what the name means to do*

### Player Aim
This script works getting the direction in "movement stats resource" and sync with a local var "aiming direction"

Then, when player press the "aiming button" (Z - placeholder), the actor will stop the inputs and the player cant move anymore. In the aiming state, the player can be aim to the 8 directions with the movement input (arrows)

The "aiming object" shows were the player is aiming (placeholder? ) 


### Player Skill System

Skills work like the entitys. So, "SkillResource" is the main/father script. His variables are: 
- Name

    _Name means what means to do_

- SkillType (Is Enum)

    _Is the type of skill, Offensive, Defensive or Utility_

- First and Second Element 

    _Is the elements for recipe_ 

- SkillElement (Enum)
 
    _Is the enum for First And Second Element_ 


Each type of skill will have a resource child with custom settings:

Offensive Resource:
 - Damage
    - _The value of damage, integer number_
 - Fire Rate 
 - Damage Skill Type
    - _Enum with the two types of damage skill: Ball or Laser. Each type will work and process the damage in different way_

Offensive Script:
 - Just check the resource variables and process the skill based on the type 


 ### Player Skill Manager

This is script have an array for the recipes and get the inputs from "GatherInput" func 




--------------------------------------------------------------------------------------------------------


I forget to update this readme, so after two months, a lot of things have changed, so, for now, Im actually working in this things: 

- [ ] Enemy Movement:
    - [X] Horizontal Movement
    - [ ] Jump
    - [ ] Flying Movement
    - [ ] Enemy Pathfind and See Objects
        - [ ] Enemy cant see player if has a wall between then
        - [ ] Enemy identifies if he can jump through platforms 
- [ ] Enemy Attack:
    - [ ] Enemy Attack if its in range
    - [ ] Enemy Attack Cooldown


#### *Date: 03/06/2024*

