# SERVER
- DoTick 348 -> 349
- player 3 connect
- GOT MESSAGE PlayerJoin from player 3
- GetGameStateAndCommandsAndAddPlayer safeTick: 349
- SENDING: {"safeTick":349,"playerStartTick":351,"playerId":3,
- A client was disconnected due to error: Timeout
- RemovePlayer safeTick: 349 | playerId: 1
- OnPlayerCommand safeTick: 349 player: 1 tick: 350
- SENDING MESSAGE ServerCommand: {"tick":350,"commands":{"complete":true,"newPlayerIds":[3],"leftPlayerIds":[1]}}
- DoTick 349 -> 350                              
- (ends up waiting for client commands for tick 251 from player 3)

# Player 1/3
- DoTick 347 -> 348
- SENDING MESSAGE PlayerCommand: {"playerId":1,"tick":349,"commands":{"complete":true,"changeDirection":1}}
- GOT MESSAGE PlayerJoin: {"safeTick":349,"playerStartTick":351,"playerId":3,
- Go safeTick: 349
- GOT MESSAGE PlayerCommand: {"playerId":1,"tick":350,"commands":{"complete":true,"changeDirection":0}}
- OnPlayerCommand safeTick: 349 player: 1 tick: 350
- GOT MESSAGE ServerCommand: {"tick":350,"commands":{"complete":true,"newPlayerIds":[3],"leftPlayerIds":[1]}}
- GOT MESSAGE PlayerCommand: {"playerId":2,"tick":351,"commands":{"complete":true,"changeDirection":0}}
- OnPlayerCommand safeTick: 349 player: 2 tick: 351
- (ends up waiting for client commands for tick 350 for players 1 and 2)


# Solution
commandHistory wasn't getting serialized because I made the inner dictionary private
