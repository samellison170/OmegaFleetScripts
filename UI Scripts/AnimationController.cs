using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationController : MonoBehaviour
{

    public Image m_Image;

    public Sprite[] m_SpriteArray;
    public float m_Speed = .02f;

    private int m_IndexSprite;
    Coroutine m_CorotineAnim;
    bool IsDone;
    private void Start()
    {
        //StartCoroutine(StartAnimationWithDelay(1f));
    }
    public void Func_PlayUIAnim()
    {
        IsDone = false;
        StartCoroutine(Func_PlayAnimUI());
    }

    public void Func_StopUIAnim()
    {
        IsDone = true;
        StopCoroutine(Func_PlayAnimUI());
    }
    public IEnumerator StartAnimationWithDelay(float delay)
    {
        //Debug.Log("i am being called!-----------------------------------------------------------------------------");
        yield return new WaitForSeconds(delay);
        StartCoroutine(Func_PlayAnimUI());
        yield return null;
    }
    IEnumerator Func_PlayAnimUI()
    {
        //Debug.Log("animation should be playing");
        yield return new WaitForSeconds(m_Speed);
        if (m_IndexSprite >= m_SpriteArray.Length)
        {
            m_IndexSprite = 0; // Optionally reset to the start, or stop the animation
            IsDone = true;
            yield break; // Stop the coroutine here
        }
        m_Image.sprite = m_SpriteArray[m_IndexSprite];
        m_IndexSprite += 1;
        if (IsDone == false)
            m_CorotineAnim = StartCoroutine(Func_PlayAnimUI());
    }
}