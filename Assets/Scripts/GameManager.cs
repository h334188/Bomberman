using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public GameObject[] players;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            NewRound();
        }
    }

    public void CheckWinState() {
        int aliveCount = 0;

        foreach (GameObject player in players) {
            if (player.activeSelf) {
                aliveCount++;
            }
        }

        if (aliveCount <= 1) {
            Invoke(nameof(NewRound), 1.5f);
        }
    }

    private void NewRound() {
        // 获取、构建、重载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
