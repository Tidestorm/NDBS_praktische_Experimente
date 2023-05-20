using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;

public class RealtimeDatabaseManager : MonoBehaviour, DatabaseFunctions
{
    FirebaseManager firebaseManager;
    public DatabaseReference DBreference;

    void Awake()
    {
        firebaseManager = GameObject.FindObjectOfType<FirebaseManager>();
    }

    public void InitializeFirebase()
    {
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public IEnumerator LoadScoreboardData()
    {
        //Get all the users data ordered by kills amount
        var DBTask = DBreference.Child("users").OrderByChild("kills").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            //Destroy any existing scoreboard elements
            foreach (Transform child in firebaseManager.scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }

            //Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string username = childSnapshot.Child("username").Value.ToString();
                int kills = int.Parse(childSnapshot.Child("kills").Value.ToString());
                int deaths = int.Parse(childSnapshot.Child("deaths").Value.ToString());
                int xp = int.Parse(childSnapshot.Child("xp").Value.ToString());

                //Instantiate new scoreboard elements
                GameObject scoreboardElement = Instantiate(
                    firebaseManager.scoreElement,
                    firebaseManager.scoreboardContent
                );
                scoreboardElement
                    .GetComponent<ScoreElement>()
                    .NewScoreElement(username, kills, deaths, xp);
            }

            //Go to scoareboard screen
            UIManager.instance.ScoreboardScreen();
        }
    }

    public IEnumerator UpdateDeaths(int _deaths)
    {
        //Set the currently logged in user deaths
        var DBTask = DBreference
            .Child("users")
            .Child(firebaseManager.User.UserId)
            .Child("deaths")
            .SetValueAsync(_deaths);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Deaths are now updated
        }
    }

    public IEnumerator UpdateKills(int _kills)
    {
        //Set the currently logged in user kills
        var DBTask = DBreference
            .Child("users")
            .Child(firebaseManager.User.UserId)
            .Child("kills")
            .SetValueAsync(_kills);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Kills are now updated
        }
    }

    public IEnumerator UpdateXp(int _xp)
    {
        //Set the currently logged in user xp
        var DBTask = DBreference
            .Child("users")
            .Child(firebaseManager.User.UserId)
            .Child("xp")
            .SetValueAsync(_xp);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Xp is now updated
        }
    }

    public IEnumerator UpdateUsernameDatabase(string _username)
    {
        //Set the currently logged in user username in the database
        var DBTask = DBreference
            .Child("users")
            .Child(firebaseManager.User.UserId)
            .Child("username")
            .SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    public IEnumerator LoadUserData()
    {
        //Get the currently logged in user data
        var DBTask = DBreference.Child("users").Child(firebaseManager.User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            //No data exists yet
            firebaseManager.xpField.text = "0";
            firebaseManager.killsField.text = "0";
            firebaseManager.deathsField.text = "0";
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            firebaseManager.xpField.text = snapshot.Child("xp").Value.ToString();
            firebaseManager.killsField.text = snapshot.Child("kills").Value.ToString();
            firebaseManager.deathsField.text = snapshot.Child("deaths").Value.ToString();
        }
    }
}
