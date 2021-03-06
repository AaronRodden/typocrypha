﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// manages the game state (cutscene/battle/etc)
public class StateManager : MonoBehaviour {
	public LoadGameFlow load_game_flow; // loads the gameflow file
	public TrackTyping track_typing; // tracks player's typing
	public CutsceneManager cutscene_manager; // manages cutscenes
	public BattleManager battle_manager; // manages battles
	public GameObject dialogue_box; // text box for dialogue

	GameScene[] scene_arr; // array of gamescenes (loaded by load_game_flow)
	int curr_scene; // current index of game scene

	IEnumerator Start () {
		while (!load_game_flow.is_loaded) // wait for gameflow to load
			yield return new WaitForSeconds (0.1f);
		scene_arr = load_game_flow.scene_arr;
		curr_scene = -1;
		nextScene ();
	}

	// transition to next scene; returns false if at end
	public bool nextScene() {
		if (curr_scene >= scene_arr.Length - 1) {
			Debug.Log ("game over");
			return false;
		}
		GameScene next_scene = scene_arr [++curr_scene];
		string scene_type = next_scene.GetType ().ToString ();
		switch(scene_type) {
		case "IntroScene":
			Debug.Log ("Intro! (please wait to change)");
			track_typing.enabled = true;
			cutscene_manager.enabled = false;
			battle_manager.enabled = false;
			dialogue_box.SetActive (false);
			StartCoroutine (nextSceneDelayed (3.0f)); // change after waiting for a bit
			break;
		case "CutScene":
			track_typing.enabled = false;
			cutscene_manager.enabled = true;
			battle_manager.enabled = false;
			dialogue_box.SetActive (true);
			cutscene_manager.startCutscene ((CutScene)next_scene);
			break;
		case "BattleScene":
			track_typing.enabled = true;
			cutscene_manager.enabled = false;
			battle_manager.enabled = true;
			dialogue_box.SetActive (false);
			battle_manager.startBattle ((BattleScene)next_scene);
			StartCoroutine (nextSceneDelayed (3.0f)); // change after waiting for a bit
			break;
		}
		return true;
	}

	// go to next scene after a delay
	public IEnumerator nextSceneDelayed(float seconds) {
		yield return new WaitForSeconds (seconds);
		nextScene ();
	}
}
