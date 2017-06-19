using UnityEngine;
using System.Collections.Generic;

public class ClimbSystem : MonoBehaviour
{
    [SerializeField]
    GameObject buttomFloor;
    [SerializeField]
    GameObject floor;
    [SerializeField]
    GameObject topFloor;
    [SerializeField]
    int midFloorNum;
    [SerializeField]
    GameObject deadLineObject;

    int floorNum = 0;
    List<GameObject> floorObjects = new List<GameObject>();
    List<Character2> characters = new List<Character2>();
    GameObject deadLine;
    BoxCollider topArea;

    static ClimbSystem gameObjectRoot;

    void Awake()
    {
        gameObjectRoot = this;
    }

    void Start()
    {
        deadLine = Instantiate(deadLineObject);
        deadLine.transform.position = Vector3.zero;
        StaticParameter.FALL = deadLine.transform.position.y;
        SetGameObject(deadLine.transform);

        transform.position = Vector3.up * StaticParameter.FLOOR_HEIGHT * 2;
        topArea = gameObject.AddComponent<BoxCollider>();
        topArea.isTrigger = true;
        topArea.size = new Vector3(100, 5, 100);
    }

    void Update()
    {
        if (deadLine.transform.position.y < StaticParameter.FALL_NUTRAL)
        {
            deadLine.transform.Translate(Vector3.up * Time.deltaTime);
            StaticParameter.FALL = deadLine.transform.position.y;
        }

        if (floorNum < 3)
        {
            MakeFloor();
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.GetComponent<Character2Behavior>() != null)
        {
            MakeFloor();
        }
    }

    public int getFloorNum()
    {
        return floorNum;
    }

    void MakeFloor()
    {
        GameObject newFloor = null;
        if (floorNum <= midFloorNum)
        {
            FloorData d = null;
            if (floorObjects.Count > 0)
            {
                d = floorObjects[floorObjects.Count - 1].GetComponent<FloorData>();
            }
            if (d == null)
            {
                foreach (Character2 c in characters)
                {
                    if (c.GetState() == CharacterState2.Ground)
                    {
                        c.ChangeState(CharacterState2.Sky);
                    }
                }

                if (floorNum == 0)
                {
                    newFloor = Instantiate(buttomFloor);
                    newFloor.transform.position = Vector3.up * StaticParameter.FLOOR_HEIGHT;
                }
                else if (floorNum == midFloorNum + 1)
                {

                }
                else if (floorNum <= midFloorNum)
                {
                    newFloor = Instantiate(floor);
                    newFloor.transform.position = Vector3.up * StaticParameter.FLOOR_HEIGHT;
                }

                transform.Translate(Vector3.down * StaticParameter.FLOOR_HEIGHT);
                topArea.center = Vector3.up * (25 - transform.position.y);
                SetGameObject(newFloor.transform);
                floorObjects.Add(newFloor);
                if (floorObjects.Count == 5)
                {
                    Destroy(floorObjects[0]);
                    floorObjects.RemoveAt(0);
                }
                floorNum++;
            }
        }
        else if (floorNum == midFloorNum + 1)
        {
            newFloor = Instantiate(topFloor);
            newFloor.transform.position = Vector3.zero;
            SetGameObject(newFloor.transform);
            floorObjects.Add(newFloor);
            floorNum++;
            StaticParameter.FALL_NUTRAL = 25;
        }

        StaticParameter.FALL = deadLine.transform.position.y;
    }

    public void AddCharacter(Character2 c)
    {
        characters.Add(c);
        characters.RemoveAll(x => x == null);
    }

    public static void SetGameObject(Transform t)
    {
        if (gameObjectRoot != null)
        {
            t.SetParent(gameObjectRoot.transform);
            Character2Behavior behavior = t.GetComponent<Character2Behavior>();
            if (behavior != null)
            {
                gameObjectRoot.AddCharacter(behavior.GetAttribution());
            }
        }
    }
}
