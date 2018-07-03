using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Основной класс, реализующий визуализацию
/// Использован нерекурсивный цикличный алгоритм (см. https://goo.gl/A3Az7G)
/// </summary>
public class LevelManager : MonoBehaviour
{
    public GameObject DiscPrefab;
    public Transform StartingTower;
    public GameObject SolvedPanel;

    const float waitingTime = 1.25f;
    float hoverHeight = 1.25f;
    float offset;
    Vector3 start, end;

    public Tower[] Towers;

	void Start ()
    {
        offset = DiscPrefab.transform.localScale.y;
        if (offset * GameData.DiscQuantity + 0.25f > hoverHeight)
            hoverHeight = offset * GameData.DiscQuantity + 0.25f;

        GenerateDiscs();

        if (GameData.DiscQuantity % 2 == 0)
            StartCoroutine(Move_Even());
        else
            StartCoroutine(Move_Odd());
    }

    void GenerateDiscs()
    {
        var q = GameData.DiscQuantity;
        for (int i = q; i > 0; i--)
        {
            var disc = Instantiate(DiscPrefab, new Vector3(StartingTower.position.x, (offset * (q-i+1) * 2), 0f), Quaternion.identity);
            float scale = disc.transform.localScale.x / q * i;
            disc.transform.localScale = new Vector3(scale, offset, scale);
            disc.GetComponent<DiscParam>().Weight = i;
            disc.transform.SetParent(StartingTower);
            disc.name = ("Disc " + i).ToString();
            Towers[0].DiscHolder.Push(disc);
        }
    }

    IEnumerator Move_Odd()
    {
        while(true)
        {
            yield return new WaitForSeconds(waitingTime);
            Move(0, 2);
            yield return new WaitForSeconds(waitingTime);
            Move(0, 1);
            yield return new WaitForSeconds(waitingTime);
            Move(1, 2);
        }
    }

    IEnumerator Move_Even()
    {
        while(true)
        {
            yield return new WaitForSeconds(waitingTime);
            Move(0, 1);
            yield return new WaitForSeconds(waitingTime);
            Move(0, 2);
            yield return new WaitForSeconds(waitingTime);
            Move(1, 2);
        }
    }

    void Move(int first, int second) // TODO: Empty condition
    {
        // If empty check
        if (Towers[first].DiscHolder.Count == 0)
        {
            if (Towers[second].DiscHolder.Count == 0) return;
            StartCoroutine(Animation(Towers[second].DiscHolder.Peek(), Towers[first]));
            Towers[first].DiscHolder.Push(Towers[second].DiscHolder.Pop());
        }
        else if (Towers[second].DiscHolder.Count == 0)
        {
            StartCoroutine(Animation(Towers[first].DiscHolder.Peek(), Towers[second]));
            Towers[second].DiscHolder.Push(Towers[first].DiscHolder.Pop());
        }

        // Weights comparison
        else if (Towers[first].DiscHolder.Peek().GetComponent<DiscParam>().Weight < Towers[second].DiscHolder.Peek().GetComponent<DiscParam>().Weight)
        {
            StartCoroutine(Animation(Towers[first].DiscHolder.Peek(), Towers[second]));
            Towers[second].DiscHolder.Push(Towers[first].DiscHolder.Pop());
        }
        else
        {
            StartCoroutine(Animation(Towers[second].DiscHolder.Peek(), Towers[first]));
            Towers[first].DiscHolder.Push(Towers[second].DiscHolder.Pop());
        }
    }

    IEnumerator Animation(GameObject obj, Tower target)
    {
        obj.transform.SetParent(gameObject.transform);

        start = obj.transform.position;
        end = new Vector3(start.x, hoverHeight, start.z);
        yield return StartCoroutine(Anim_Pop(obj, target));

        start = obj.transform.position;
        end = new Vector3(target.transform.position.x, start.y, start.z);
        yield return StartCoroutine(Anim_Hover(obj, target));

        start = obj.transform.position;
        end = new Vector3(start.x, offset * target.DiscHolder.Count * 2, start.z);
        yield return StartCoroutine(Anim_Push(obj, target));

        obj.transform.SetParent(target.transform);
        WinConditionCheck();
        yield break;
    }

    IEnumerator Anim_Pop(GameObject obj, Tower target)
    {
        var t = 0f;
        while (obj.transform.position.y != end.y)
        {
            t += Time.deltaTime * GameData.Speed;
            obj.transform.position = new Vector3(start.x, Mathf.Lerp(start.y, end.y, t), start.z);
            yield return null;
        }
        yield break;
    }

    IEnumerator Anim_Hover(GameObject obj, Tower target)
    {
        var t = 0f;
        while (obj.transform.position.x != end.x)
        {
            t += Time.deltaTime * GameData.Speed;
            obj.transform.position = new Vector3(Mathf.Lerp(start.x, end.x, t), start.y, start.z);
            yield return null;
        }
        yield break;
    }

    IEnumerator Anim_Push(GameObject obj, Tower target)
    {
        var t = 0f;
        while (obj.transform.position.y != end.y)
        {
            t += Time.deltaTime * GameData.Speed;
            obj.transform.position = new Vector3(start.x, Mathf.Lerp(start.y, end.y, t), start.z);
            yield return null;
        }
        yield break;
    }

    void WinConditionCheck()
    {
        if (Towers[0].DiscHolder.Count == 0 &&
            Towers[1].DiscHolder.Count == 0 &&
            Towers[2].DiscHolder.Count == GameData.DiscQuantity)
        {
            StopAllCoroutines();
            SolvedPanel.SetActive(true);
        }
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
