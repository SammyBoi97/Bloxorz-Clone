using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTile : MonoBehaviour
{
    public SwitchInfo switchInfo;

    public List<GameObject> childTiles = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignChildTiles() {

        foreach (string childTilePosStr in switchInfo.childTilePositions) {
            GameObject childTile  = GetTileAtStrPosition(childTilePosStr);
            childTile.SetActive(switchInfo.childTilesStartEnabled);
            childTiles.Add(childTile);
        }
    }

    public GameObject GetTileAtStrPosition(string strPosition) {

        int x = int.Parse(strPosition.Split(',')[0]);
        int z = int.Parse(strPosition.Split(',')[1]);

        Vector3 rayOrigin = new Vector3(x, 5f, z);
        RaycastHit raycastHit;

        if(Physics.Raycast(rayOrigin, Vector3.down, out raycastHit, 10f, LayerMask.GetMask("Tile"))) {
            return raycastHit.transform.gameObject;
        }

        return null;
    }

    public void TriggerSwitch() {

        foreach (GameObject tile in childTiles) {
            if (switchInfo.switchFunction == 0) {
                tile.SetActive(false);
            }
            else if (switchInfo.switchFunction == 1) {
                tile.SetActive(true);
            }
            else {
                tile.SetActive(!tile.activeSelf);
            }
        }

        
    }

    // public void ToggleChildTiles() {
    //     switchInfo.isOn = !switchInfo.isOn;

    //     foreach (GameObject tile in childTiles) {
    //         tile.SetActive(switchInfo.isOn);
    //     }
    // }
}
