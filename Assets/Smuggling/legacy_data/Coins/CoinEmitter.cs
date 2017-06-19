using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoinEmitter : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> coins;
    private Vector3 position;
    private int point;
    private bool act;
    private float wait;

    void Start()
    {

    }

    public void Setting(Vector3 pos, int pt)
    {
        position = pos + Vector3.up * 0.1f;
        point = pt;
        act = true;
    }

    void Update()
    {
        if (act)
        {
            wait += Time.deltaTime;
            if (wait > 0.1f)
            {
                wait -= 0.1f;
                point -= Emission();
                if (point <= 0)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    int Emission()
    {
        int type = 0;
        int lost = 1;
        if (point > 100)
        {
            type = 4;
            lost = 100;
        }
        else if (point > 50)
        {
            type = 3;
            lost = 50;
        }
        else if (point > 10)
        {
            type = 2;
            lost = 10;
        }
        else if (point > 5)
        {
            type = 1;
            lost = 5;
        }
        GameObject newcoin = Instantiate<GameObject>(coins[type]);
        newcoin.transform.position = position;
        newcoin.transform.Rotate(Vector3.up * Random.Range(0f, 360f));
        Coin coinscr = newcoin.GetComponent<Coin>();
        Vector3 vec = new Vector3(Random.Range(-5f, 5f), Random.Range(1f, 5f), Random.Range(-5f, 5f)).normalized * 6f;
        coinscr.Setup(vec);
        return lost;
    }
}
