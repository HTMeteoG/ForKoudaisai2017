using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace koudaigame2017
{
    public class StageManager : MonoBehaviour
    {
        /*
         * どうしよう
         * 基本は2Dフィールド
         * 奥に道が続いている場合、上を押すとカメラが90度回転する
         * 
         */
        StageNavMeshManager navManager;

        void Start()
        {
            navManager = new StageNavMeshManager();
            navManager.DefineBound(transform.position, Vector3.one * 100);

            Transform startObject = transform.GetChild(0);
            navManager.AddSource(navManager.MakeSourceFromBox(startObject, "Walkable"));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public class StageNavMeshManager
    {
        // NavMeshBuildSettingsはプログラムから追加・編集および削除が可能っぽい
        NavMeshBuildSettings navSetting;
        // NavMesh作成に使うデータの集合
        List<NavMeshBuildSource> navSources = new List<NavMeshBuildSource>();
        // NavMeshはこのbound領域内に含まれる部分しか生成されない
        Bounds navBound;
        // NavMeshBuilderが作成したデータ、これをNavMeshに追加して初めてゲームに反映される
        NavMeshData navData;

        // 追加時に得られるNavMeshDataInstanceは、それを削除する際に必要なため、ここに格納する
        // なお、NavMeshDataInstanceにはNavMesh.AddNavMeshData()で追加したNavMeshDataがそのまま入る。
        NavMeshDataInstance usingNavMesh;

        public StageNavMeshManager()
        {
            navSetting = NavMesh.GetSettingsByIndex(0);
        }

        public void DefineBound(Vector3 center, Vector3 size)
        {
            navBound = new Bounds(center, size);
        }

        public int AddSource(NavMeshBuildSource source)
        {
            NavMesh.RemoveNavMeshData(usingNavMesh);
            navSources.Add(source);
            int index = navSources.Count - 1;
            navData = NavMeshBuilder.BuildNavMeshData(navSetting, navSources, navBound, Vector3.zero, Quaternion.identity);
            usingNavMesh = NavMesh.AddNavMeshData(navData);
            return index;
        }

        public void RemoveSource(int index)
        {
            NavMesh.RemoveNavMeshData(usingNavMesh);
            navSources.RemoveAt(index);
            navData = NavMeshBuilder.BuildNavMeshData(navSetting, navSources, navBound, Vector3.zero, Quaternion.identity);
            usingNavMesh = NavMesh.AddNavMeshData(navData);
        }

        public NavMeshBuildSource MakeSourceFromBox(Transform t, string layer)
        {
            NavMeshBuildSource newNavSource = new NavMeshBuildSource();

            // 1.transform情報を得る。4*4matrix形式にする必要がある。
            newNavSource.transform = t.localToWorldMatrix;
            // 2.形状を選択する。
            newNavSource.shape = NavMeshBuildSourceShape.Box;
            // 3.サイズを得る。
            //      ただし、配置済みのオブジェクトを使用する場合は既にサイズ情報を得ているため、
            //      情報が重複しないように"Vector3.one"にしないといけない。
            newNavSource.size = Vector3.one;

            return newNavSource;
        }

        public NavMeshBuildSource MakeSourceFromMesh(Transform t, string layer)
        {
            NavMeshBuildSource newNavSource = new NavMeshBuildSource();

            // 1.transform情報を得る。4*4matrix形式にする必要がある。
            newNavSource.transform = t.localToWorldMatrix;
            // 2.形状を選択する。
            newNavSource.shape = NavMeshBuildSourceShape.Mesh;
            // 2-2.形状をMeshまたはTerrainとした場合のみ、使用するメッシュ(テライン)情報を格納する
            newNavSource.sourceObject = t.GetComponent<MeshFilter>().mesh;
            // 3.サイズを得る。
            newNavSource.size = Vector3.one;

            return newNavSource;
        }

    }
}