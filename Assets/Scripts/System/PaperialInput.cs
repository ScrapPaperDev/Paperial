using Paperial.Services;

namespace Paperial
{
    public static class PaperialInput
    {

        public const string axis_LHori = "Hori";
        public const string axis_LVerti = "Verti";
        public const string axis_RHori = "RHori";
        public const string axis_RVerti = "RVerti";

        public const string button_Select = "Submit";
        public const string button_Cancel = "Cancel";
        public const string button_Loop = button_Select;
        public const string button_Pause = "Pause";

        public static float GetHori
        {
            get
            {
                float i = UnityEngine.Input.GetAxisRaw(axis_LHori);
                return Game.invertX ? -i : i;
            }
        }

        public static float GetVerti
        {
            get
            {
                float i = UnityEngine.Input.GetAxisRaw(axis_LVerti);
                return Game.invertY ? -i : i;
            }
        }
    }
}