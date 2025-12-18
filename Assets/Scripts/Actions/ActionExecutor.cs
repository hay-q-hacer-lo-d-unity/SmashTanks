using UnityEngine;

namespace Actions
{
    public class ActionExecutor
    {
        private readonly AudioSource _audioSource;

        public ActionExecutor(AudioSource audioSource)
        {
            _audioSource = audioSource;
        }

        public void Execute(IAction action, Vector3 origin, Vector3 target)
        {
            action.Execute(origin, target);

            if (action is IActionWithSound sound && sound.ExecuteSound)
            {
                _audioSource.PlayOneShot(sound.ExecuteSound);
            }
        }
    }

}