using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FBLogin : MonoBehaviour {

	private string userID;
	private AccessToken AccessToken;
	private bool scraped;

	void Awake() {
		
		if (!FB.IsInitialized) {
			// Initialize the Facebook SDK
			FB.Init(InitCallback, OnHideUnity);
			Debug.Log ("Initializing Facebook");
		}else {
			// Already initialized, signal an app activation App Event
			FB.ActivateApp();
			Debug.Log ("Activating App");
		}
		scraped = false;
	
	}

	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...
		} else {
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}

	private void OnHideUnity (bool isGameShown)
	{
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}



	public void LogIn () {

		var perms = new List<string>(){"public_profile", "email"};
		Debug.Log ("Putting in Permissions");
		FB.LogInWithReadPermissions(perms, AuthCallback);
		Debug.Log ("Finished Logging In");


//		FB.API("/me", HttpMethod.GET, HandleResult);

//		foreach (string perm in perms) {
//			FB.API (perm, HttpMethod.GET, HandleResult);		
//		}

//		FB.API("/me", HttpMethod.GET, HandleResult);


	}



	private void HandleResult(IResult result)
	{
		if (result == null)
		{
			Debug.Log ("Null Response\n");
			return;
		}

		// Some platforms return the empty string instead of null.
		if (!string.IsNullOrEmpty(result.Error))
		{
			Debug.Log ("Error Response: " + result.Error);
		}
		else if (result.Cancelled)
		{
			Debug.Log ("Cancelled - Check log for details");
		}
		else if (!string.IsNullOrEmpty(result.RawResult))
		{
			Debug.Log ("Success - Timeline file is stored");
			System.IO.File.WriteAllText(Application.dataPath + "/TestData/" + userID + "_results.json", result.RawResult);

		}
		else
		{
			Debug.Log ("Empty Response");
		}

	}

	private void AuthCallback (ILoginResult result) {
		if (FB.IsLoggedIn) {
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			AccessToken = aToken;
			// Print current access token's User ID
			Debug.Log(aToken.UserId);
			userID = aToken.UserId;
			// Print current access token's granted permissions
			foreach (string perm in aToken.Permissions) {
				Debug.Log(perm);
			}

		} else {
			Debug.Log("User cancelled login");

		}
	}



	
	// Update is called once per frame
	void Update () {
		
		if (FB.IsLoggedIn && !scraped) {
			scraped = true;
			FB.API ("/me", HttpMethod.GET, HandleResult);
		} else {
			//do nothing
		}

	}
}
