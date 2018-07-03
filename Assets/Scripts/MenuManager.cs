using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public InputField DiscQuantityInput;

    private void Start()
    {
        GameData.DiscQuantity = 4;
    }

    public void SetDiscsAmount()
    {
        GameData.DiscQuantity = System.Convert.ToInt32(DiscQuantityInput.text);
    }

    public void StartLevel()
    {
        SceneManager.LoadScene("Level");
    }
}
