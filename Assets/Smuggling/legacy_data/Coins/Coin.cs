using UnityEngine;
using System.Collections;
using HTGames.BattleMain;
using HTGames.System;
using System;

public class Coin : MonoBehaviour, Blowable
{
    [SerializeField]
    private int point;
    private Rigidbody rd;
    private Vector3 vector;

    [SerializeField]
    GameObject effector;
    [SerializeField]
    int effectorPower = 1;

    void Start()
    {
        rd = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (transform.position.y < -10)
        {
            Destroy(this.gameObject);
        }
    }

    public void Setup(Vector3 vec)
    {
        Start();
        rd.AddForce(vec, ForceMode.VelocityChange);
    }


    void OnTriggerEnter(Collider col)
    {
        /*
		CharaHitter ch = col.GetComponent<CharaHitter> ();
		if(ch != null){
            Character c = ch.GetChara();
            c.GetPoint(point);
			Destroy (this.gameObject);
		}
        */
    }

    public void Blow(Vector3 vec)
    {
        rd.AddForce(vec, ForceMode.VelocityChange);
    }

    public void Catched(Character c)
    {
        c.GetCharacterStatus().AddValue(StatusName.Points, point);
        for (int i = 0; i < effectorPower; i++)
        {
            Instantiate(effector, transform.position + Vector3.up * 0.1f, Quaternion.identity);
        }
        Destroy(this.gameObject);
    }
}