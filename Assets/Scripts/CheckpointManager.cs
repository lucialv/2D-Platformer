using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    [Header("Lista de Checkpoints (en orden)")]
    public GameObject[] checkpoints;

    private int lastCheckpointIndex = -1;

    private PlayerManager player;
    [SerializeField]
    private GameObject spawnPoint;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
        player = playerGameObject.GetComponent<PlayerManager>();
    }

    public void UpdateCheckpoint(GameObject checkpoint)
    {
        int index = System.Array.IndexOf(checkpoints, checkpoint);

        if (index > lastCheckpointIndex)
        {
            lastCheckpointIndex = index;
            Debug.Log($"Checkpoint actualizado al {index}");
        }
    }

    public void RespawnPlayer()
    {
        if (lastCheckpointIndex >= 0)
        {
            player.transform.position = checkpoints[lastCheckpointIndex].transform.position;
        }
        else
        {
            player.transform.position = spawnPoint.transform.position;
        }

        player.ResetPlayer();
    }
}
