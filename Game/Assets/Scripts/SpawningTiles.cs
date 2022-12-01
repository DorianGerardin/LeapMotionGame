using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningTiles : MonoBehaviour
{

    public GameObject tileObj;
    public GameObject lastTile; 

    private Vector3 nextTileSpawn;
    

    // Start is called before the first frame update
    void Start()
    {
        nextTileSpawn = new Vector3(0f, 0f, lastTile.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if(lastTile.transform.position.z <= 450f) {
            SpawnTile();
        }
    }

    private void SpawnTile() {
        lastTile = Instantiate(tileObj, nextTileSpawn, tileObj.transform.rotation);
    }
}
