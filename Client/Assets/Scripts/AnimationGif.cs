using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;


[RequireComponent(typeof(Image))]
public class AnimationGif : MonoBehaviour
{
    public Sprite[] animationSprites;
    public float animationSpeed = 0.1f;
    public bool loop = true;

    public Action animationEndCallback;

    private Image _img;
    private float _curretnTime;
    private int _index;
    private float _length;
    private bool _continue;

    void Awake()
    {
        _continue = true;
        _img = GetComponent<Image>();
        _length = animationSprites.Length;
    }

    private void OnEnable()
    {
        _continue = true;
        _curretnTime = Time.time - animationSpeed - 1;
        _index = -1;
    }

    public void SetAnimation(bool play)
    {
       this.gameObject.SetActive(true);
        _img.sprite = animationSprites[0];
        _continue = play;
    }

    private void Update()
    {
        if (_continue && Time.time - _curretnTime > animationSpeed)
        {
            _index++;

            if (_index >= _length)
            {
                _index = 0;
                if (!loop)
                {
                    _continue = false;
                    if (animationEndCallback != null)
                        animationEndCallback();
                }
            }
            else
            {
                _curretnTime = Time.time;
                _img.sprite = animationSprites[_index];
                _img.SetNativeSize();
            }
        }
    }
}
