using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Vector3 currentRespawnPosition;
    [SerializeField] private CinemachineOrbitalFollow cinemachineOrbitalFollow;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    public void SetCheckpoint(Vector3 newPosition)
    {
        currentRespawnPosition = newPosition;
    }

    public void ReSpawnPlayer(Player player)
    {
        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null)
        {
            cc.enabled = false;

            player.transform.position = currentRespawnPosition;
            player.transform.rotation = Quaternion.identity;
            cinemachineOrbitalFollow.HorizontalAxis.Value = 0f;
            cinemachineOrbitalFollow.VerticalAxis.Value = 17.5f;
            
            cc.enabled = true;
        }
        else
        {
            player.transform.position = currentRespawnPosition;
        }
    }
}
