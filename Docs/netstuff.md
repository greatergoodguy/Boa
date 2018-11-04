GameStateMachine
	Menu
	Lobby
	Playing
	GameOver


ServerStateMachine
	Match
		Lobby
		Playing
		GameOver
	


ServerStart
	start tick

ClientStart
	ask server for safe tick, game state at safe tick, commands from safe tick + 1 forward
	get safe tick and set it in simulation
	get gameState, simulation will put in previous game states, presenter will present it
	start ticking and listening for new commands
	ask server to spawn snake
	server creates spawn snake command at future tick and sends to clients
