using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
  public Levelstueck[] levelstuecks;
  public Transform _camera;
  public int drawDistance;

  public float pieceLength;
  public float speed;
  Queue<GameObject> activePieces = new Queue<GameObject>();
  List<int> probabilityList = new List<int>();


  int currentcamstep = 0;
  int lastcamstep = 0;

  private void Start()
  {
    BuildProbabilityList();

    //Spawn starting level pieces
    for (int i = 0; i < drawDistance; i ++)
    {
      Spawnnaechstesstueck();
    }


    currentcamstep = (int)(_camera.transform.position.z / pieceLength);
    lastcamstep = currentcamstep;
  }

  private void Update()
  {
    _camera.transform.position = Vector3.MoveTowards(_camera.transform.position, _camera.transform.position + Vector3.forward, Time.deltaTime * speed);

    currentcamstep = (int)(_camera.transform.position.z / pieceLength);
    if (currentcamstep != lastcamstep)
    {
      lastcamstep = currentcamstep;
      Spawnnaechstesstueck();
      Despawnpeace();

    }
  }

  void Spawnnaechstesstueck()
  {
    int stueckIndex = probabilityList[Random.Range(0, probabilityList.Count)];
    GameObject newLevelstueck = Instantiate(levelstuecks[stueckIndex].prefab, new Vector3(0f, 0f,(currentcamstep + activePieces.Count) * pieceLength), Quaternion.identity);
    activePieces.Enqueue(newLevelstueck);
  }
  void Despawnpeace(){
  GameObject altesstueck = activePieces.Dequeue();
  Destroy(altesstueck);
}

  void BuildProbabilityList()
  {
    int index = 0;
    foreach(Levelstueck stueck in levelstuecks)
    {
      for (int i = 0; i < stueck.probability; i++)
      {
        probabilityList.Add(index);
      }
      index++;
    }
  }
}
[System.Serializable]
public class Levelstueck
{
  public string name;
  public GameObject prefab;
  public int probability = 1;
}

