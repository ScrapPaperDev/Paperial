using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paperial
{
    /// <summary>
    /// Describes an object that can be interacted via a 'collision' by an interactor
    /// </summary>
    public interface IInteractable
    {
        void Interact(IInteractor interactor);
    }

    /// <summary>
    /// Describes an object that can interact with 'collision' zones
    /// </summary>
    public interface IInteractor : IIdentifiable
    {
        Transform transform { get; }
        void OnTriggerEnter(Collider other);
    }

    /// <summary>
    /// Describes an object that can set an area of the play area as active or 'filled'
    /// </summary>
    public interface IFiller
    {
        IEnumerable<Vector3> GetFillArea(float nodeSize);
    }

    /// <summary>
    /// Interface for if you need movement to be a Monobehaviour.
    /// </summary>
    public interface IMovementProvider
    {
        void Bind(Transform t, params object[] dependencies);
        void Fly();
        void Sommersault(float x, float y);

    }

    /// <summary>
    /// Describes an object that can be tracked by the UI radar.
    /// </summary>
    public interface ITrackable
    {
        Transform transform { get; }
        Sprite Icon { get; }
        float IconSize { get; }
    }

    /// <summary>
    /// Describes an object that is affected by being in areas with confetti.
    /// </summary>
    public interface IConfettiAffectable
    {
        void EnterConfetti();
        void ExitConfetti();
    }

    public interface IPresenter<T>
    {
        T Presenter { get; }
        GameDataModel GameData { get; }
        void Present();
    }

    public interface IManaged
    {
        void ManageUpdate();
    }

    public interface IIdentifiable
    {
        string Identifier { get; }
        int ID { get; }
        int GetHashCode();
    }



}