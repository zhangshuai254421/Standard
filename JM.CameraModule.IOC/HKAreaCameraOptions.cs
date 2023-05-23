using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JM.CameraModule.IOC
{
    public class HKAreaCameraOptions {
    public const string Camera01 = "Camera01";
    public const string Camera02 = "Camera02";
    public const string Camera03 = "Camera03";
    public const string Camera04 = "Camera04";
    public const string Camera05 = "Camera05";
    public const string Camera06 = "Camera06";
    public const string Camera07 = "Camera07";
    public const string Camera08 = "Camera08";

    public string IP { get; set; }

    public string SN { get; set; }
    public bool BlUseSoftTrigger { get; set; }

    public TriggerSourceEnum triggerSourceEnum { get; set; }

    public TriggerModeEnum triggerModeEnum { get; set; }

    public AcquisitionModeEnum acquisitionModeEnum { get; set; }
    }
}
