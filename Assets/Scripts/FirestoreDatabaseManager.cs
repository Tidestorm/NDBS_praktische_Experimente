using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;

public class FirestoreDatabaseManager : MonoBehaviour, DatabaseFunctions
{
    FirebaseManager firebaseManager;
    public static FirebaseFirestore DBreference;

    void Awake()
    {
        firebaseManager = GameObject.FindObjectOfType<FirebaseManager>();
    }

    public void InitializeFirebase()
    {
        DBreference = FirebaseFirestore.DefaultInstance;
    }

    public IEnumerator LoadScoreboardData()
    {
        yield return new WaitForSeconds(0.1f);
    }

    public IEnumerator UpdateDeaths(int _deaths)
    {
        DocumentReference userRef = DBreference
            .Collection("users")
            .Document(firebaseManager.User.UserId);

        UpsertValue(userRef, "deaths", _deaths);
        yield return new WaitForSeconds(0.1f);
    }

    public IEnumerator UpdateKills(int _kills)
    {
        DocumentReference userRef = DBreference
            .Collection("users")
            .Document(firebaseManager.User.UserId);

        UpsertValue(userRef, "kills", _kills);
        yield return new WaitForSeconds(0.1f);
    }

    public IEnumerator UpdateXp(int _xp)
    {
        DocumentReference userRef = DBreference
            .Collection("users")
            .Document(firebaseManager.User.UserId);

        UpsertValue(userRef, "xp", _xp);
        yield return new WaitForSeconds(0.1f);
    }

    public IEnumerator UpdateUsernameDatabase(string _username)
    {
        yield return new WaitForSeconds(0.1f);
    }

    public IEnumerator LoadUserData()
    {
        yield return new WaitForSeconds(0.1f);
        // Load user's data from the database
        DocumentReference userRef = DBreference
            .Collection("users")
            .Document(firebaseManager.User.UserId);
        var DBTask = userRef.GetSnapshotAsync();

        DBTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle error.
                Debug.LogError("Error retrieving document: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DocumentSnapshot UserDataSnapshot = task.Result;
                try
                {
                    if (!UserDataSnapshot.Exists)
                    {
                        //No data exists yet
                        firebaseManager.xpField.text = "0";
                        firebaseManager.killsField.text = "0";
                        firebaseManager.deathsField.text = "0";
                        Debug.Log("Currently there is no Data that exists.");
                    }
                    else
                    {
                        //Data has been retrieved
                        Dictionary<string, object> data = UserDataSnapshot.ToDictionary();

                        if (data.ContainsKey("xp"))
                            firebaseManager.xpField.text = data["xp"].ToString();
                        if (data.ContainsKey("kills"))
                            firebaseManager.killsField.text = data["kills"].ToString();
                        if (data.ContainsKey("deaths"))
                            firebaseManager.deathsField.text = data["deaths"].ToString();
                    }
                }
                catch (Exception e)
                {
                    firebaseManager.xpField.text = "0";
                    firebaseManager.killsField.text = "0";
                    firebaseManager.deathsField.text = "0";
                    Debug.LogError("Error when checking if UserDataSnapshot exists: " + e.Message);
                }
            }
        });
    }

    private async Task UpsertValue(DocumentReference docRef, string fieldName, object value)
    {
        var data = new Dictionary<string, object> { { fieldName, value } };
        await UpsertValue(docRef, data);
    }

    private async Task UpsertValue(DocumentReference docRef, object value)
    {
        await docRef.SetAsync(value, SetOptions.MergeAll);
        if (value is IDictionary<string, object>)
        {
            await docRef.SetAsync(value, SetOptions.MergeAll);
        }
        else
        {
            Debug.LogError(
                $"Failed to upsert document because the provided value is not a dictionary: {value}"
            );
        }
    }
}
