using UnityEngine;
using UnityEngine.Serialization;

namespace Actions
{
    public class Sounds : MonoBehaviour
    {
        public static Sounds Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this);
            else Instance = this;
            DontDestroyOnLoad(gameObject);

            _bgSource = gameObject.AddComponent<AudioSource>();
            _bgSource.loop = true;
            _bgSource.spatialBlend = 0f; // 2D
            _bgSource.volume = 0.6f;
        }
        
        
        public void PlayStatsScreenBackground()
        {
            if (_bgSource.isPlaying) return;

            _bgSource.clip = statsScreenBackground;
            _bgSource.Play();
        }
        public void PlayGameBackground()
        {
            if (_bgSource.isPlaying) return;

            _bgSource.clip = crowdBackground;
            _bgSource.Play();
        }

        public void StopBackground()
        {
            _bgSource.Stop();
        }
        
        public void PlayButtonClick()
        {
            Play2DSound(buttonClick);
        }
        
        [Header("Actions")]
        public AudioClip missile;
        public AudioClip bouncyMissile;
        public AudioClip beam;
        public AudioClip jump;
        public AudioClip crash;
        public AudioClip crashImpact;
        public AudioClip teleport;
        public AudioClip gale;
        public AudioClip juggernaut;
        
        [Header("Ambience")]
        public AudioClip crowdBackground;
        public AudioClip statsScreenBackground;
        [FormerlySerializedAs("crowdCheerDamage")] public AudioClip crowdCheerHit;
        
        [Header("UI")]
        public AudioClip buttonClick;
        
        private AudioSource _bgSource;

        
        public static void Play2DSound(AudioClip clip, float volume = 1f)
        {
            var go = new GameObject("OneShotAudio");
            var source = go.AddComponent<AudioSource>();

            source.clip = clip;
            source.volume = volume;
            source.spatialBlend = 0f;
            source.Play();

            Object.Destroy(go, clip.length);
        }

    }
}