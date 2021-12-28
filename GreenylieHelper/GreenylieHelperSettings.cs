using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;

namespace Celeste.Mod.GreenylieHelper
{
        public class GreenylieHelperSettings : EverestModuleSettings
    {
        [SettingName("modoptions_examplemodule_title")]
        public bool Test { get; set; } = false;

        public bool Other { get; set; } = false;
    }
}
