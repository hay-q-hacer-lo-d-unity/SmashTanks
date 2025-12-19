using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;

namespace Manager
{
    public class SoundsScript : MonoBehaviour
    {
        public static SoundsScript Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _bgSource = gameObject.AddComponent<AudioSource>();
            _bgSource.loop = true;
            _bgSource.spatialBlend = 0f;
            _bgSource.volume = 0.6f;
        }
        
        private void Start()
        {
            if (skillsetMute != null)
            {
                skillsetMute.onClick.AddListener(ToggleMute);
            }

            if (playerCountMute != null)
            {
                playerCountMute.onClick.AddListener(ToggleMute);
            }

            UpdateButtonText();
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
        
        public AudioClip atomicEssence;
        public AudioClip atomicHit;
        
        [Header("Ambience")]
        public AudioClip crowdBackground;
        public AudioClip statsScreenBackground;
        [FormerlySerializedAs("crowdCheerDamage")] public AudioClip crowdCheerHit;
        
        [Header("UI")]
        public AudioClip buttonClick;
        
        private AudioSource _bgSource;

        [Header("MuteButtons")]
        [SerializeField] private Button skillsetMute;
        [SerializeField] private Button playerCountMute;

        public void ToggleMute()
        {
            AudioListener.volume = AudioListener.volume > 0 ? 0f : 1f;
            UpdateButtonText();
        }

        private void UpdateButtonText()
        {
            bool isMuted = AudioListener.volume <= 0;
            string textToShow = isMuted ? "Unmute" : "Mute";

            UpdateButtonTextInternal(skillsetMute, textToShow);
            UpdateButtonTextInternal(playerCountMute, textToShow);
        }

        private void UpdateButtonTextInternal(Button button, string text)
        {
            if (button == null) return;

            var tmpText = button.GetComponentInChildren<TMP_Text>();
            if (tmpText != null)
            {
                tmpText.text = text;
                return;
            }

            var legacyText = button.GetComponentInChildren<Text>();
            if (legacyText != null)
            {
                legacyText.text = text;
            }
        }
        
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