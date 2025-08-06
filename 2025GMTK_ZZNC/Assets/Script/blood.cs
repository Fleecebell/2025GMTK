using UnityEngine;
using UnityEngine.UI; // 确保包含UI命名空间

public class blood : MonoBehaviour
{
    public Slider mySlider;
    public static float HP = 100f;

    // Start is called before the first frame update
    void Start()
    {
        if (mySlider != null)
        {
            mySlider.value = HP;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mySlider != null)
        {
            mySlider.value = HP;
        }
    }
}