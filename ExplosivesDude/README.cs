/* BUGS:
 * - server communication is garbage ( ͡° ͜ʖ ͡°)
 *      
 * minor:
 * - player doesn't continue moving if changing direction midway
 * - players connected/client count doesn't get updated if server/client leaves
 * - when server closes, client button still says connected
 */
/* TODO:
 * + mp: allow sending direct messages instead of broadcast (for e.g. newclient/player action)
 * + mp: make game restartable without server restarting
 * + game: make games end (game won if alive, lost if dead, draw/tie if no winner)
 * + mp: send player properties, not just actions (to reflect real player stats, even if cheating)
 * + mp: no new players after game start
 * + mp: status (waiting for players, active game, server full)
 *  
 * more ideas:
 * ~ mp: add gameclient, has properties like isready etc, display list of clients (like lobby?)
 * + ui: overhaul icons (32x32?), for high resolution (e.g. surface pro) (or use aliasing scaling?)
 * + game: more powerups (kick (bombs away), player cloak (with timer), go-through-wall-thingy, ...)
 * ~ game: cheat detection by calculating if previous/new value was/is correct (e.g. if diff of oldx and newx > 1)
 * ~ game: option for continuous movement (until blocked) (just restart animation until blocked)
 * ~ stats: health etc with color green-yellow-red? (circles around icon?)
 * ~ stats: change colors in playerstats per playercolor, let player choose color?
 * ~ player: different character icons (with diff internal names), chosen at random in mp?
 * ~ ui: more player images to mimmick movement, also in playerstats?
 * ~ ui: blast images: center, 1 direction, rotate for others?
 * ~ ui: different designs (emp, fire, water, acid, bomb) using different images, maybe external?
 * ~ game: set final x/y after half animation?
 * ~ mp: set x/y directly after starting animation (assume mvmnt only possible after animation completed)
 * ~ mp: let server keep track of gamestate and playerstates (authoritative server)
 * ~ mp: lobby (collection of players and games, players can join/leave lobby)?
 * ~ mp: up to 4 players / per server, players can create/join/leave/start games
 * ~ mp: let a player create a server instance and let him decide map/amount of players needed etc
 * ~ mp: choose map randomly? first player chooses? vote for map?
 * ~ game: ai enemys
 * ~ game: different game modes (find key for door (/axe for different design?))
 * ~ game: map selector (live view, server send id, etc.)
 * ~ game: teleportation module, floor booster direction thingy
 * ~ map: custom maps + external? create using map editor
 * ~ map: dynamic and bigger maps, only partly visible, minimap?
 */
/* Changelog:
 * 
 * 2018.12.08: 0.44.3
 * - Remove IPowerup
 * - Log which item was picked up
 * - Remove playerstates, back to IsDead and IsReady
 * 2018.11.30: 0.44.2
 * - Playerstats now get reset when player gets removed/disposed
 * 2018.11.29: 0.44.1
 * - All objects now in one Dictionary instead of many (uses new id system)
 * 2018.11.25:
 * - network enhancements
 * 2018.11.11:
 * - network changes
 * 2018.11.09: 0.44.0
 * - fix countdown continues after another player leaves in mp
 * - fix when player leaves in mp, client doesn't remove (correct) player from game or mark as absent
 * - other small changes
 * 2018.11.05: 0.43.2
 * - fix addclient leads to crash
 * - check if key exists before adding in objectmanager
 * - add update function to objectmanager
 * - stop countdown before starting
 * 2018.11.03/04/05: 0.43.1
 * - code optimizations
 * 2018.11.02: 0.43.0
 * - changed map a bit, more space in the center
 * - disabled remote and power (for now)
 * - add hasID for checking if id has been set
 * - fix previous players disconnect if new player joins after one player changed id
 * - remove addplayer functions due to starting position arrays
 * - use port as clientId, independent of playerId
 * - add newplayer action for sending own starting position to new player (if hasId)
 * 2018.10.31: 0.42.0
 * - Range based damage (full damage above/near bomb, at the end a third damage, linear in between)
 * - Health now in percentage, healthkit always heals up to 100%
 * - Add Shield, consumes damage before health, shieldboost adds 20% per item
 * - Add RemoteTrigger, enabled through item, activated by pressing space after placing bomb (like double click)
 * - Fix TooltipMessage not updating
 * - Minor UI changes
 * - Fix mp bug where a blast hit would damage the wrong player, clients would not be in sync anymore
 * 2018.10.26: 0.41.6
 * - Switch a few things to Binding
 * - Remove a few things in favor of Binding (eg. UpdateTooltip)
 * - Number of players are now displayed in clients in multiplayer mode
 * 2018.06.15: 0.41.5
 * - move countdown code to mainwindow
 * - fix countdown continues in background by using dispatchertimer instead of async await
 * - fix being unable to unselect player and thus stopping the countdown
 * - extend uioperationprovider and reduce usage of static
 * 2018.06.12: 0.41.4
 * - removed images enum
 * - removed uimanager class, incorporate functions in other classes where needed
 * - removed statuspair
 * 2018.06.07: 0.41.3
 * - move gameSizeX/Y back to Game class
 * - move playerstats to game class, events included
 * - optimizations
 * 2018.06.05: 0.41.2
 * - deprecate playerdirection in favor of dx/dy
 * 2018.06.04: 0.41.1
 * - fixed bug where blastwave would continue after hitting a wall with power > 5
 * - remove bombtype
 * - disable c4 in favor of wip remotetrigger
 * 2018.06.03: 0.41.0
 * - added power to player and bomb, high enough power destroys walls
 * - added tooltips to playerstats
 * - disabled dynamite in favor of power
 * 2018.05.28: 0.40.3
 * - add powerIncreaser and remoteTrigger classes / images
 * 2018.05.14: 0.40.2
 * - let player unselect stats, stops countdown
 * - remove ready button due to ready state now selected via stats
 * 2018.05.12: 0.40.1
 * - playerstats now reset after a game
 * 2018.05.11: 0.40.0
 * - merged blastwave and dynamiteblast
 * - start replacing direction with dx/dy
 * - code cleanup
 * - fixed player receives double the c4 if triggered by remote and by other explosive simultaneously
 * 2018.05.09: 0.39.3
 * - remove isready and isdead in favor of states.ready/dead
 * - playerstates now get sent to other players
 * 2018.05.08: 0.39.2
 * - fixed network spam bug due to unchecked playerid before sending commands
 * - changed adding boxes to exclude player spawnpoints instead of removing boxes on newplayer creation
 * - add boxes after everyone ready / right before game starts
 * 2018.05.06: 0.39.1
 * - removed selection grid
 * - remerged gameclient into game
 * 2018.05.05: 0.39.0
 * - new selection grid
 * - removed start button in favor of ready button
 * 2018.05.04: 0.38.0
 * - removed auto bomb switch feature
 * - removed bomb switch feature, different bombamounts
 * - add changeplayerid option
 * - playerstats / start position selection now in multiplayer as well
 * - player now loses health when walking into blastwave
 * - fixed crossing blastwave disappear too soon
 * - fixed wrong playericon in stats
 * 2018.05.03: 0.37.1
 * - added broadcastexcept to simplify brodcasts which do not need to be sent back
 * - no more unnecessary id check in gameclient due to broadcastexcept
 * 2018.04.30: 0.37.0
 * - auto switch to bomb if c4/dynamite empty
 * - ui changes
 * - removed coords, playerid and name in playerstats
 * - removed stopwatch
 * - removed chat feature
 * 2018.04.13: 0.36.0
 * - start position / playerstats now selectable in singleplayer
 * 2018.04.11: 0.35.2.1
 * - split client and playerid
 * 2018.04.10:
 * - make playerstats selectable
 * 0.35.2
 * - fixed itempickup not syncing correctly
 * - fixed wrong playerstats used for players 1 and 3
 * 0.35.1.1
 * - don't check for bomb amount if addexplosive for other player
 * - experimental change for adding/removing c4 amount in player
 * 2018.04.08: 0.35.1
 * - remove unnecessary version/buildtime strings
 * 2018.04.08: 0.35.0
 * - server selection via combobox
 * 2018.04.06: 0.34.5
 * - new trigger mechanism for c4, now via event
 * - ui adjustments
 * 2018.04.04: 0.34.4
 * - code cleanup
 * - removed more variables
 * 2018.04.03: 0.34.3
 * - better player hit by blast handling
 * - removed some unnecessary variables (next/currentdir)
 * 2018.04.03: 0.34.2
 * - added new domain
 * - changed icon for health
 * 2018.04.03: 0.34.1
 * - fixed player movement while chatting
 * - fixed client crash when clicking ready
 * - button ready now disabled when not connected
 * - bt ready now disabled after restarting mp game
 * - fixed player movement update in mp
 * - better itempickedup handling
 * 2018.04.02: 0.34.0
 * - seperated gameclient from game
 * - added temp methods for controlling game
 * - serveraddress and port now as setting
 * - fixed a few console writelines in commandsreceived event
 * - changed message encoding to utf8
 * 2018.04.02: 0.33.3
 * - fixed bug where mp game was impossible due to ready being processed incorrectly
 * - fixed focus bug
 * - chatbox now scrolls to bottom automatically
 * 2018.04.02: 0.33.2
 * - add player states
 * 2018.04.01: 0.33.1
 * - use full int for seed
 * - add version to mainwindow
 * 2018.03.31: 0.33.0
 * - added ready/waiting status
 * - mp game now waits for everyone to be ready before starting
 * - playerstats now marks active player using bordercolor
 * - implemented simple chat
 * 2018.03.30: 0.32.1
 * - use random only at creation time, not during runtime
 * - fixed bug where walls didn't get reset properly
 * - add boxes before players, remove boxes on NewPlayer
 * - add more dynamite icons
 * - seperated gameserver from game
 * - changed how seed works
 * 2018.01.04: 0.32.0
 * - dynamite now destroys walls
 * - disable auto-switch bombtype on pickup
 * - only allow keyboard input if game is running
 * - add stopwatch; currently controlled by mainwindow
 * 2018.01.03: 0.31.0
 * - moved animation from player into extra class
 * - added program icon
 * - changed some mainwindow xaml static stuff
 * - fixed server connected players label
 * - removed unnecessary mpflag
 * - ui changes
 * - added explosive switching using numbers 1-3
 * - start work on dynamite
 * - limit number of clients on server / add serverfull message
 * 2017.11.04: 0.30.1
 * - small changes
 * 2017.10.31: 0.30.0
 * - add objectmanager
 * - code refactoring (remove newimmage from parameters)
 * - add c4, explosives, etc
 * 2017.10.30: 0.29.1
 * - remove floor, no longer necessary
 * 2017.10.29: 0.29.0
 * - added removeall* for dynamic object
 * - fixes resetgame not working correctly
 * - removed static arrays for objects entirely
 * - changed function of map class to uimanager
 * - moved stuff out of uimanager
 * - code refactoring
 * 2017.10.26: 0.28.0
 * - add remove methods for dynamic objects
 * 2017.10.25: 0.27.0
 * - added z-index for player, fixes bomb appears on top of player
 * - made mapobject abstract
 * - changed some functions for point usage
 * - deprecate boxallowed
 * - make powerups dynamic: abstract powerup class, remove bomballowed, remove powerups from mapgrid
 * - changed addnewpowerup
 * - fixed blast doesnt remove powerups
 * - reverted point usage
 * - removed mapobject booleans, refactored code for compatibility
 * - fixed boxes can replace walls
 * - moved action to powerup class
 * - removed onactionended and related eventargs
 * 2017.10.14: 0.26.0
 * - made map.blocksize non static, map.newimage also, player/bomb/blast updated to not use these functions anymore
 * 2017.10.13: 0.25.1
 * - fixed blastrange
 * - fixed bombs can be placed on bombs
 * - fixed player can walk through bombs
 * 2017.10.12: 0.25.0
 * - code refactoring
 * - no more public static elements
 * - moved functions out of mapobject into game
 * 2017.10.11: 0.24.0
 * - changed updatetip to updatetooltip
 * - networkmanager now sends header first, contains int for number of bytes coming afterwards
 * - changed imagetosource to use index for multiple e.g. player images
 * - changed speedup formula, player can now move faster
 * - changed a few icons
 * 2017.10.10: 0.23.0
 * - added movementcomplete event to player
 * - fixed player movement and bomb placement in mp
 * 2017.10.08: 0.22.0
 * - merging client/server to networkmanager
 * - add eventargs for networkmanager
 * - add imagetosource function in map, no more strings from outside, just enum
 * - remove unnecessary variables (multi in game, bools in mainwindow)
 * - added playerid to playerstats
 * - simplified addplayer
 * 2017.10.07: 0.21.0
 * - code cleanup
 * - change builddate to last file change, removed pre build txt file
 * - code refactoring using stylecop
 * - change how seed works
 * - changed a few names
 * - move mapobject child classes to their own files
 * 2017.10.03: 0.20.1
 * - revert to non coordinate mp model as there were even more problems
 * - fixed getting wrong playerid after connecting to new server in some cases (where no client in server process e.g.)
 * 2017.10.02: 0.20.0
 * - fixed button focus bug
 * - fixed singeplayer not resetting bug
 * - change mp movement to send coordinates
 * - change moveanimation in player to trigger using sent data
 * 2017.10.01: 0.19.0
 * - refactor server, now independent of player/client
 * - player movement now before sending command to server, should fix input lag, not sure about server syncing
 * - make client/server stoppable
 * - add resetgame function
 * - client / server are now stoppable AND restartable
 * 2017.09.30: 0.18.1
 * - player movement methods now called from game class, broke mp
 * 2017.09.29: 0.18.0
 * - fixed movement animation bug (due to unchanged coords)
 * - added random seed for mp (0-255)
 * - add option to connect to localhost
 * - add debugmode for playerstats
 * - boxallowed removed from players
 * 2017.09.28: 0.17.1
 * - add compilation date
 * - reverse fix for network spam, movement was unstable
 * 2017.09.27: 0.17.0
 * - change playerstats
 * - code cleanup
 * - fix network spam
 * - network now sends/receives 3 bytes, fixed length
 * 2017.09.26: 0.16.0
 * - try different playerstats
 * - add statuspair for better playerstats handling
 * 2017.09.25: 0.15.0
 * - add new powerup healthkit
 * - player can now die
 * - fixed random box generation seed thingy
 * - changed playerstats to icons
 * - added more playercolors
 * - added playerstats for second player
 * - fixed actions if player is dead
 * - added playerimage to stats
 * 2017.09.24: 0.14.0
 * - add enum for server commands
 * - set player id in game, not server class
 * - move players into list, array and playercount no longer needed
 * - add bomb in player class
 * 2017.09.12: 0.13.0
 * - don't use image in oop classes, use imagegrid instead
 * - fixed addplayer for mp
 * - move map stuff to extra class
 * - move direction enum to game class
 * - move classes to their corresponding files
 * - create external readme file
 * - code cleanup
 * 2017.03.18: 0.12.0
 * - start implementing multiplayer
 * 2017.03.17: 0.11.1
 * - small improvements
 * 2017.03.13: 0.11.0
 * - fixed movement (again)
 * 2017.03.10: 0.10.0
 * - switched to OOP to simplify methods/properties/classes etc
 * 2017.03.09: 0.9.0
 * - added playerstats element
 * - added new properties to player
 * - linked player props to playerstats props
 * - broke movement partially again (silly me)
 * - added new baseobject (rly necessary!?)
 * 2017.03.08: 0.8.0
 * - changed speed values
 * - added second player
 * - fixed blast dissapearing bug
 * - added new power-up: bombs
 * - bomb amount now limited, use new power-up for more
 * - small design changes
 * - changed power-up frequency
 * 2017.03.07: 0.7.0
 * - changed movement, seems stable
 * - fixed movement boundaries
 * 2017.03.07: 0.6.0
 * - changed movement, still wip
 * - code cleanup & other improvements
 * 2017.03.06: 0.5.0
 * - fixed direction causing illegal moves
 * - fixed bomb chain reaction
 * 2017.03.05: 0.4.0
 * - add range to bombs/blast
 * - add boxes (more like planks)
 * - changed blast behaviour
 * - added new wall texture
 * 2017.03.05: 0.3.0
 * - change to new gameobject
 * 2017.03.05: 0.2.0
 * - add bomb and explosion
 * - change player behaviour
 * - added wall around grid
 * 2017.03.04 and before: 0.1.0
 * - add grid_game
 * - create rectangles, with diff. colors for floor/wall
 * - add debug labels
 * - add player1 as image
 * - move player1 using animations
 * - sort of add hitbox check
 * - everything else
 */