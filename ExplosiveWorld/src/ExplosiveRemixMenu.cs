using Menu.Remix.MixedUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RWCustom;

namespace ExplosiveWorld
{
    public class ExplosiveMenu : OptionInterface
    {
        public static Configurable<bool> NoBombs { get; set; }
        public static Configurable<bool> SingulatiryBombs { get; set; }

        //public static Configurable<float> DamageSmall { get; set; }
        //public static Configurable<float> DamageEXTREME { get; set; }

        //public static Configurable<float> Rad { get; set; }
        //public static Configurable<float> RadEXTREME { get; set; }

        private OpCheckBox NoBombsBox;
        private OpCheckBox SingulatiryBombsBox;

        private OpLabel NoBombsLabel;
        private OpLabel SingulatiryBombsLabel;

        //Small select 1-100
        // new OpUpdown(ScreenShakeValue, new Vector2(10,340), 50){_fMax = 10.0f, _fMin = 0.1f},
        // new OpLabel(70f, 340f, "ScreenShake streght"),
        //Big select
        // new OpFloatSlider(MaulDamageExtra, new Vector2(5, 245), 200, 2, true){max = 10000f, min = 1f, hideLabel = false},
        // new OpLabel(45f, 420f, "Maul Extra DAMAGE!"),

        public ExplosiveMenu()
        {
            NoBombs = config.Bind("NoBombs", false, new ConfigAcceptableList<bool>(false, true));
            SingulatiryBombs = config.Bind("SingulatiryBombs", false, new ConfigAcceptableList<bool>(false, true));
        }
        public override void Update()
        {
            base.Update();
            Color colorOff = new Color(0.1451f, 0.1412f, 0.1529f);
            Color colorOn = new Color(0.6627f, 0.6431f, 0.698f);

            string caseValue = "";
            if (NoBombsBox.value == "true") caseValue = "NoBombs";
            else if (SingulatiryBombsBox.value == "true") caseValue = "SingulatiryBombsBox";

            switch (caseValue)
            {
                case "NoBombs": //1
                    NoBombsBox.greyedOut = false;
                    SingulatiryBombsBox.greyedOut = true;

                    NoBombsLabel.color = colorOn;
                    SingulatiryBombsLabel.color = colorOff;
                    break;

                case "SingulatiryBombsBox": //2
                    NoBombsBox.greyedOut = true;
                    SingulatiryBombsBox.greyedOut = false;

                    NoBombsLabel.color = colorOff;
                    SingulatiryBombsLabel.color = colorOn;
                    break;

                default: //3
                    NoBombsBox.greyedOut = false;
                    SingulatiryBombsBox.greyedOut = false;

                    NoBombsLabel.color = colorOn;
                    SingulatiryBombsLabel.color = colorOn;
                    break;
            }
        }

        public override void Initialize()
        {
            var opTab1 = new OpTab(this, Translate("Options"));
            Tabs = new[] { opTab1};

            OpContainer tab1Container = new OpContainer(new Vector2(0, 0));
            opTab1.AddItems(tab1Container);

            UIelement[] UIArrayElements1 = new UIelement[]
            {
                NoBombsBox = new OpCheckBox(NoBombs, 10, 540) { description = Translate("Disable all the bombs") },
                NoBombsLabel = new OpLabel(45f, 540f, Translate("No Bombs")),

                SingulatiryBombsBox = new OpCheckBox(SingulatiryBombs, 10, 500) { description = Translate("Enable singularity bombs instead of normal bombs! :3c") },
                SingulatiryBombsLabel = new OpLabel(45f, 500f, Translate("Singulatiry Bombs")),
            };
            opTab1.AddItems(UIArrayElements1);
        }
    }
}
