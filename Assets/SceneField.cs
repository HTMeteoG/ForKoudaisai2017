using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class SceneField : MonoBehaviour
{
    [SerializeField]
    Vector3 boundSize = Vector3.one * 100;
    [SerializeField]
    string meshLayer = "Walkable";

    /*
     その他詳しいことは、Unityメニューバーの「Help -> Scripting Reference」を参照すること！
     特に日本語版は未完成のため、ネット上で検索しても見つからないので注意！
         */
    // NavMeshBuildSettingsはプログラムから追加・編集および削除が可能っぽい
    NavMeshBuildSettings navSetting;

    // NavMesh作成に使うデータの集合、これを用意するプログラムが重要そうだ
    List<NavMeshBuildSource> navSources = new List<NavMeshBuildSource>();

    // このbound空間内部にあるNavMeshしか生成されない
    Bounds navBound;

    // NavMeshBuilderが作成したデータ、これをNavMeshに追加して初めてゲームに反映される
    NavMeshData navData;

    // 追加時に得られるNavMeshDataInstanceは、それを削除する際に必要なため、ここに格納する
    // なお、NavMeshDataInstanceにはNavMesh.AddNavMeshData()で追加したNavMeshDataがそのまま入る。
    NavMeshDataInstance usingNavMesh;

    // Use this for initialization
    void Start()
    {
        //多分初期設定はこれで取得できる
        navSetting = NavMesh.GetSettingsByIndex(0);
        navBound = new Bounds(transform.position, boundSize);

        BuildNavmeshData();
    }

    // 以下、NavMeshBuildSource作成例
    public static NavMeshBuildSource MakeNavMeshBuildSourceFromBox(Transform t)
    {
        NavMeshBuildSource newNavSource = new NavMeshBuildSource();

        // 1.transform情報を得る。4*4matrix形式にする必要がある。
        newNavSource.transform = t.localToWorldMatrix;
        // 2.形状を選択する。
        newNavSource.shape = NavMeshBuildSourceShape.Box;
        // 2-2.形状をMeshまたはTerrainとした場合のみ、使用するメッシュ(テライン)情報を格納する
        // newNavSource.sourceObject = t.GetComponent<MeshFilter>().mesh;
        // 3.サイズを得る。
        //      ただし、配置済みのオブジェクトを使用する場合は既にサイズ情報を得ているため、
        //      情報が重複しないように"Vector3.one"にしないといけない。
        newNavSource.size = Vector3.one;

        return newNavSource;
    }

    public static NavMeshBuildSource MakeNavMeshBuildSourceFromBox(Transform t, string layer)
    {
        NavMeshBuildSource source = MakeNavMeshBuildSourceFromBox(t);
        // 4.(必要ならば)Areaを指定する
        source.area = NavMesh.GetAreaFromName(layer);
        return source;
    }

    void BuildNavmeshData()
    {
        if (NavMesh.GetAreaFromName(meshLayer) == -1)
        {
            meshLayer = "Walkable";
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform t = transform.GetChild(i);
            if (t.gameObject.layer.CompareTo(LayerMask.NameToLayer("NavMesh")) == 0)
            {
                /*
                MeshBuildSclipt s = t.GetComponent<MeshBuildSclipt>();
                if (s != null)
                {
                    navSources.AddRange(s.GetNavmeshes());
                }
                else
                {
                    MeshLinkGenerator link = t.GetComponent<MeshLinkGenerator>();
                    if (link != null)
                    {
                        link.ReGenerate();
                    }
                    else
                    {
                        navSources.Add(MakeNavMeshBuildSourceFromBox(t, meshLayer));
                    }
                }
                */
            }
        }

        navData = NavMeshBuilder.BuildNavMeshData(navSetting, navSources, navBound, Vector3.zero, Quaternion.identity);
        usingNavMesh = NavMesh.AddNavMeshData(navData);
        usingNavMesh.owner = gameObject;
    }

    void Rebuild()
    {
        // 1つのNavMeshを再構成する際はnavSourcesを編集する必要がある。
        Debug.Log("del");
        usingNavMesh.Remove();

        navSources.Clear();

        BuildNavmeshData();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Rebuild();
        }
    }
}