using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    private List<AudioClip> _audioClips = new();
    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {

        _audioClips = Resources.LoadAll<AudioClip>($"Music/Background/").ToList();
        for (int i = 0; i < _audioClips.Count; i++)
        {
            if (_audioClips[i] == null)
            {
                _audioClips.Remove(_audioClips[i]);
                i--;
            }
        }
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_audioSource.isPlaying)
        {
            System.Random rand = new((int)(System.DateTime.Now.Ticks));
            _audioSource.clip = this._audioClips[rand.Next(this._audioClips.Count)];
            _audioSource.Play();
        }
    }
}
