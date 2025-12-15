using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenMenu : MonoBehaviour
{
    public Transform player; // ссылка на объект игрока

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // сохраняем позицию игрока
            PlayerPrefs.SetFloat("PlayerPosX", player.position.x);
            PlayerPrefs.SetFloat("PlayerPosY", player.position.y);
            PlayerPrefs.SetFloat("PlayerPosZ", player.position.z);

            PlayerPrefs.SetFloat("PlayerRotY", player.eulerAngles.y);

            PlayerPrefs.Save();

            // разблокируем курсор
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // загружаем сцену
            SceneManager.LoadScene("MainMenu");
        }
    }
}
