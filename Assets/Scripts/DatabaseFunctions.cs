using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface DatabaseFunctions
{
    IEnumerator LoadUserData();
    IEnumerator LoadScoreboardData();
    IEnumerator UpdateDeaths(int _deaths);
    IEnumerator UpdateKills(int _kills);
    IEnumerator UpdateXp(int _xp);
    IEnumerator UpdateUsernameDatabase(string _username);
}
