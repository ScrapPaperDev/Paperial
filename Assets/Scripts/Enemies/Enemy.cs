using Paperial.Services;
using UnityEngine;

namespace Paperial
{
    public abstract class Enemy : MonoBehaviour, IConfettiAffectable, ITrackable
    {
        protected PlayArea area;
        [SerializeField] private GameDataModel gameDataModel;

        public Sprite icon;
        public Sprite Icon => icon;

        public float IconSize => .5f;

        protected virtual void Init() => area = Game.GetService<PlayArea>();

        public virtual void ManageUpdate() { }

        public void EnterConfetti() { }

        public void ExitConfetti() { }

        public virtual void Take()
        {
            Init();
            gameObject.SetActive(true);
            area.Presenter.AddNewTrackable(this);
        }

        public virtual void Return()
        {
            gameObject.SetActive(false);
            area.Presenter.RemoveTrackable(this);
        }
    }
}