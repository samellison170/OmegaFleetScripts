using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class FadeInImage : MonoBehaviour
{
    public Image image;
    public float fadeDuration = 3.0f;

    private float targetAlpha = .5f;
    private float initialAlpha = 0.0f;

    // Time to wait before starting the FadeIn coroutine
    public float delayTime = 3.0f;

    private void Start()
    {
        if (image != null)
        {
            Color color = image.color;
            color.a = initialAlpha;
            image.color = color;
        }
    }

    // Make this coroutine public so other scripts can access it
    public IEnumerator FadeIn()
    {

        yield return new WaitForSeconds(delayTime);

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(initialAlpha, targetAlpha, elapsedTime / fadeDuration);

            if (image != null)
            {
                Color color = image.color;
                color.a = newAlpha;
                image.color = color;
            }

            yield return null;
        }

        if (image != null)
        {
            Color color = image.color;
            color.a = targetAlpha;
            image.color = color;
        }
    }
}
