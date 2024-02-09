using UnityEngine;
using UnityEngine.EventSystems;
using static Paperial.PaperialInput;

namespace Paperial
{
    public class EventSystemInputHelper : MonoBehaviour
    {

        private void Awake()
        {
            var eve = GetComponent<StandaloneInputModule>();

            eve.horizontalAxis = axis_LHori;
            eve.verticalAxis = axis_LVerti;
            eve.submitButton = button_Select;
            eve.cancelButton = button_Cancel;
        }

    }
}
