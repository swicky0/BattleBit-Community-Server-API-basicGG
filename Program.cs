using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Server;
using System.Threading.Channels;
using System.Xml;

class Program
{
    static void Main(string[] args)
    {
        var listener = new ServerListener<MyPlayer, MyGameServer>();
        listener.Start(29294);

        Thread.Sleep(-1);
    }
}
class MyPlayer : Player<MyPlayer>
{
    public int weaponLevel = 1;

    public void increaseWeaponLevel() {
        weaponLevel += 1;
    }

    public void decreaseWeaponLevel() {


        if (weaponLevel > 1) { weaponLevel -= 1; }
        
    }   
}
class MyGameServer : GameServer<MyPlayer>
{

/*TODO:
 * Expand the datastructure to include information describing if the item is primary or secondary weapon
 * This is needed to invoke either player.PrimaryWeapon or player.Secondaryweapn
 */

    //TODO: Make a sensible list of weapons
    private static List<Weapon> progressionMap = new List<Weapon> { 
    Weapons.MP7,
    Weapons.AK15,
    Weapons.P90,
    Weapons.MP7,
    Weapons.AK15,
    Weapons.P90,
    Weapons.MP7,
    Weapons.AK15,
    Weapons.P90,

    };
        

    public override async Task OnRoundStarted()
    {
    }
    public override async Task OnRoundEnded()
    {
    }

    public override async Task OnPlayerConnected(MyPlayer player)
    {

       /*
        * TODO Implement some QoL logic so new players dont start at level 0, but maybe the average of the server
        */

 
    }

    public override async Task OnAPlayerKilledAnotherPlayer(OnPlayerKillArguments<MyPlayer> args)
    {

        args.Killer.increaseWeaponLevel();

        // If killed by melee weaon, decrease level of victim
        if (args.KillerTool == Gadgets.Pickaxe.Name || args.KillerTool == Gadgets.PickaxeIronPickaxe.Name) 
        {
            args.Victim.decreaseWeaponLevel();
            AnnounceLong("MELEEE KIIIIL");
        }


        /*TODO
       * Check a win condition, i.e. max level reached and end the round by changing map
       * For now just print it 
       * 
       */
        if(args.Killer.weaponLevel == progressionMap.Count) {
            AnnounceLong("Winner, winner, chicken dinner");
        }



    }


    public override async Task<OnPlayerSpawnArguments> OnPlayerSpawning(MyPlayer player, OnPlayerSpawnArguments request)
    {

        // Give the player the weapon they reached

        request.Loadout.PrimaryWeapon.Tool = progressionMap[player.weaponLevel];


        // TODO: Find out if Secondary weapon can be removed. If not, implement logic to ignore kills with this

        request.Loadout.SecondaryWeapon = default; 
            request.Loadout.LightGadget = null;
            request.Loadout.HeavyGadget = Gadgets.SledgeHammer;
            request.Loadout.Throwable = null;
        

        return request;
    }
    public override async Task OnPlayerSpawned(MyPlayer player)
    {
   
    }



    public override async Task OnConnected()
    {
        await Console.Out.WriteLineAsync("Current state: " + RoundSettings.State);

    }
    public override async Task OnGameStateChanged(GameState oldState, GameState newState)
    {
        await Console.Out.WriteLineAsync("State changed to -> " + newState);
    }
}
