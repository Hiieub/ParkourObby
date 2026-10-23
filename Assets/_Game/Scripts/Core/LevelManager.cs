using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Database Configuration")]
    [SerializeField] private LevelDatabaseSO levelDatabase;

    [Header("Level Spawn Root")]
    [SerializeField] private Transform levelRoot;

    [SerializeField] private Player player;

    private GameObject currentLoadedLevel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (levelDatabase != null)
        {
            DataManager.SetLevelDatabase(levelDatabase);
        }
    }

    private void Start()
    {
        string selectedLevel = DataManager.SelectedLevel;
        LoadLevel(selectedLevel);
    }

    public void LoadLevel(string levelKey)
    {
        if (currentLoadedLevel != null)
        {
            Destroy(currentLoadedLevel);
        }

        if (levelDatabase == null)
        {
            levelDatabase = DataManager.Database;
        }

        LevelConfigSO config = levelDatabase != null ? levelDatabase.GetLevelById(levelKey) : null;
        if (config != null && config.LevelPrefab != null)
        {
            Transform parent = levelRoot != null ? levelRoot : transform;
            currentLoadedLevel = Instantiate(config.LevelPrefab, Vector3.zero, Quaternion.identity, parent);

            StartPos startPos = currentLoadedLevel.GetComponentInChildren<StartPos>();
            if (startPos != null && player != null)
            {
                player.SetStartPos(startPos.transform.position, startPos.transform.rotation);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.FindAndRegisterCheckpoints();
            }
        }
        else
        {
            Debug.LogError($"[LevelManager] Không tìm thấy cấu hình cho level '{levelKey}' trong LevelDatabase!");
        }
    }
}
