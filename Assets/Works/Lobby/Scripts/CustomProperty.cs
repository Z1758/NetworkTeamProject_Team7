using Photon.Realtime;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
public static class CustomProperty
{
    private static PhotonHashtable customProperty = new PhotonHashtable();
    public const string READY = "Ready";
    public const string NICK = "";
    public const string PASSWORD = "";
    public static void SetReady(this Player player, bool ready)
    {

        customProperty.Clear();
        customProperty[READY] = ready;
        //customProperty.Add("Ready", ready);
        player.SetCustomProperties(customProperty);
    }
    public static void SetNickName(this Player player, string nick)
    {

        customProperty.Clear();
        customProperty[NICK] = nick;
        player.SetCustomProperties(customProperty);
    }
    public static bool GetReady(this Player player)
    {
        PhotonHashtable customProperty = player.CustomProperties;
        if (customProperty.ContainsKey(READY))
        {
            return (bool)customProperty[READY];
        }
        else
        {
            return false;
        }

    }
    public static string GetNick(this Player player)
    {
        PhotonHashtable customProperty = player.CustomProperties;
        if (customProperty.ContainsKey(NICK))
        {
            return (string)customProperty[NICK];
        }
        else
        {
            return "";
        }

    }

    public const string LOAD = "Load";

    public static void SetLoad(this Player player, bool load)
    {

        customProperty.Clear();
        customProperty[LOAD] = load;
       
        player.SetCustomProperties(customProperty);
    }
    public static bool GetLoad(this Player player)
    {
        PhotonHashtable customProperty = player.CustomProperties;
        if (customProperty.ContainsKey(LOAD))
        {
            return (bool)customProperty[LOAD];
        }
        else
        {
            return false;
        }

    }



    public static void SetPassWord(this Player player, string password)
    {

        customProperty.Clear();
        customProperty[PASSWORD] = password;
        player.SetCustomProperties(customProperty);
    }

    public static string GetPassWord(this Player player)
    {
        PhotonHashtable customProperty = player.CustomProperties;
        if (customProperty.ContainsKey(PASSWORD))
        {
            return (string)customProperty[PASSWORD];
        }
        else
        {
            return "";
        }

    }


}
