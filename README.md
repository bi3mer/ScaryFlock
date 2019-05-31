# Game AI School 2019 Jam Game

## Main Menu

To make the main menu look atleast a bit polished I took spawning code I wrote for the main game and added a boolean that represents whether we are in the menu scene or the game scene. In the menu scene, colliders are turned off and predator spawning is turned off so the number of agents present on the screen is static. The text looks pretty bad but oh well.

![](./media/main_menu.gif)

## Problems

### Performance

One of the negatives is that we are running a bunch of agents that need to know things about everyone around them. This causes a performance bottleneck which can be seen below. The best fix is to move to ECS but as a first step I'm going to try to convert to using [Unity Jobs](https://docs.unity3d.com/Manual/JobSystem.html). Another improvement that can, and likely will, be used is to reduce the amount of calls for neighbors by setting the radius' used to one or two as opposed to the six that are currently called.

![](./media/pre_jobs.PNG)

### Insane Prey Spawn Rate

Agents are able to evolve and mate extremely quickly at this point in the game. The problems with this are partially due to size of agent, effectiveness of predator, and time till they can mate again. The first part of the list to focus on is the low hanging fruits. Agents can be reduced in size so they are less likely to touch and we can increase the time until they can mate again. The second part of fixing the predator movement will also fix a bug in general for movemnt. If you look at the predator, red square, on the bottom left of the screen, you'll see that it never moves. Instead it runs around in circles. This represents a bug in my flocking code and needs to be resolved since it not only effects the predators but also the prey.

![](./media/sample_flocking_without_player.gif)