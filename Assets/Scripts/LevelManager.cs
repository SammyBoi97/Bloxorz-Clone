using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class SwitchInfo {
    public int level;
    public bool isSoftSwitch;
    public int switchFunction;
    public string position;
    public string[] childTilePositions;
    public bool childTilesStartEnabled;
}

[System.Serializable]
public class Switches {
    public SwitchInfo[] switches;
}


public class LevelManager : MonoBehaviour
{
    public Transform startingPosition;

    public int curLevel = 1;

    public Texture2D[] levelTextureMaps;

    public GameObject floorTilePrefab;
    public GameObject goalTilePrefab;
    public GameObject softSwitchPrefab;
    public GameObject hardSwitchPrefab;
    public GameObject toggleTilePrefab;
    public GameObject lightFloorPrefab;

    public GameObject curLevelTiles;

    public TextAsset switchesInfoJsonFile;

    public List<SwitchInfo> level2SwitchInfos = new List<SwitchInfo>();

    public Switches switchesInJson;


    // Start is called before the first frame update
    void Start()
    {
        switchesInJson = JsonUtility.FromJson<Switches>(switchesInfoJsonFile.text);
        NextLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextLevel() {
        curLevel++;

        while (curLevelTiles.transform.childCount > 0) {
            DestroyImmediate(curLevelTiles.transform.GetChild(0).gameObject);
        }

        BuildLevelFromTextureMap(levelTextureMaps[curLevel - 1]);
    }

    public void BuildLevelFromTextureMap(Texture2D levelTextureMap) {

        List<GameObject> allNewTiles = new List<GameObject>();

        for (int x = 0; x < levelTextureMap.width; x++) {
            for (int y = 0; y < levelTextureMap.height; y++) {


                Color curPixel = levelTextureMap.GetPixel(x, y);

                GameObject tileToInstantiate = null;
                GameObject newTile = null;

                if (curPixel == Color.white || curPixel == Color.red) {
                    tileToInstantiate = floorTilePrefab;
                }
                if (curPixel == Color.red) {
                    startingPosition.transform.position = new Vector3(x, 1f , y);
                }
                if (curPixel == Color.black) {
                    tileToInstantiate = goalTilePrefab;
                }
                if (curPixel == Color.green) {
                    tileToInstantiate = hardSwitchPrefab;
                }
                if (curPixel == Color.blue) {
                    tileToInstantiate = softSwitchPrefab;
                }
                if (curPixel == Color.cyan) {
                    tileToInstantiate = toggleTilePrefab;
                }
                if (curPixel == new Color(1,1,0,1)) {
                    Debug.Log("yellow");
                    tileToInstantiate = lightFloorPrefab;
                }

                if (tileToInstantiate) {
                    newTile = Instantiate(tileToInstantiate, new Vector3(x, 0f, y), Quaternion.identity, curLevelTiles.transform);
                    allNewTiles.Add(newTile);
                }
            }
        }

        foreach(GameObject tile in allNewTiles) {
            
            foreach (SwitchInfo switchInfo in switchesInJson.switches) {
                if (switchInfo.level != curLevel) {
                    continue;
                }

                if (checkTileIsSwitch(switchInfo, tile)) {
                    SwitchTile newSwitchTile = tile.GetComponent<SwitchTile>();
                    if (newSwitchTile == null) {
                        newSwitchTile = tile.AddComponent<SwitchTile>();
                        newSwitchTile.switchInfo = switchInfo;
                        newSwitchTile.AssignChildTiles();
                    }
                }

            }
        }

    }

    public bool checkTileIsSwitch(SwitchInfo switchInfo, GameObject tile) {
        return switchInfo.position == string.Format("{0},{1}", tile.transform.position.x, tile.transform.position.z);
    }

    public bool checkTileIsChildOfSwitch(SwitchInfo switchInfo, GameObject tile) {
        foreach (string childTilePos in switchInfo.childTilePositions) {
            if (childTilePos == string.Format("{0},{1}", tile.transform.position.x, tile.transform.position.z)) {
                return true;
            }
        }
        return false;
    }

}


