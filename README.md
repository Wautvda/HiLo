# Hi-Lo Game
This is a simple High-low game where the player has to guess a random number.

## How to Play
Start the application that will autostart a scalar UI.

1. Register your name using the player/create endpoint.
2. Start a new game using the game/create endpoint.
3. Guess a number using the game/guess/{sessionId} endpoint using the session guid received in previous step.
4. Repeat step 3 until you win

## Multiplayer
1. Register your name using the player/create endpoint.
2. Join an existing game using the game/join/{sessionId} endpoint using the session guid.

## Possible extensions
- Add domain events and SignalR so frontend application can subscribe to them and display ongoing games and results.
- Track all guesses.
- Finalise the game when the player guesses the number.
- Add paging
- Add sql docker container
- Add different game modes (Ef inheritance)
- Add UI (angular, react, vue) To use all endpoints and SignalR.