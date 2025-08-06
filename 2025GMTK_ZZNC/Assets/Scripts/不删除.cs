using UnityEngine;

public class 不删除 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
