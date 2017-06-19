using UnityEngine;
using System.Collections.Generic;

public class CharacterSystem : MonoBehaviour {
    [SerializeField]
    GameObject playerObject;
    [SerializeField]
    List<GameObject> mobObjects = new List<GameObject>();

    ClimbSystem climb;

	void Start () {
        climb = FindObjectOfType<ClimbSystem>();
	}
	
	void Update () {
	    
	}
}
