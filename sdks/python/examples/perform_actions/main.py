import asyncio
import datetime
import pathlib
import random
import sys

sys.path.append(
    str(pathlib.Path(__file__).parent.parent.parent.resolve()))

from src.novelcraft.sdk import *


async def main():
    await initialize()

    start_time = datetime.datetime.now()

    while True:
        # Must await to ensure that other coroutines can run
        await asyncio.sleep(0.1)

        if datetime.datetime.now() - start_time > datetime.timedelta(seconds=60):
            # Run for 60 seconds
            break

        agent = get_agent()
        if agent is None:
            continue

        possible_actions = [0, 1, 2, 3, 4]
        action = random.choice(possible_actions)
        print(f"Action: {action}")

        if action == 0:
            possible_interactions = [
                IAgent.InteractionKind.CLICK,
                IAgent.InteractionKind.HOLD_START,
                IAgent.InteractionKind.HOLD_END
            ]
            interaction = random.choice(possible_interactions)
            agent.attack(interaction)
        elif action == 1:
            agent.jump()
        elif action == 2:
            position = Position(
                random.random() * 10000 - 5000,
                random.random() * 10000 - 5000,
                random.random() * 10000 - 5000
            )
            agent.look_at(position)
        elif action == 3:
            possible_movements = [
                IAgent.MovementKind.FORWARD,
                IAgent.MovementKind.BACKWARD,
                IAgent.MovementKind.LEFT,
                IAgent.MovementKind.RIGHT,
                IAgent.MovementKind.STOPPED
            ]
            movement = random.choice(possible_movements)
            agent.set_movement(movement)
        elif action == 4:
            possible_interactions = [
                IAgent.InteractionKind.CLICK,
                IAgent.InteractionKind.HOLD_START,
                IAgent.InteractionKind.HOLD_END
            ]
            interaction = random.choice(possible_interactions)
            agent.use(interaction)

    await finalize()

if __name__ == '__main__':
    asyncio.run(main())
