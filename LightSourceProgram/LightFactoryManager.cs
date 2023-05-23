using System.Threading;

namespace LightSourceProgram
{
    public abstract class LightFactoryManager
    {
        public Light Initialize(LightType type)
        {
            Light light = null;
            light=CreateLight(type);
            return light;
        }
        public abstract Light CreateLight(LightType type);
    }
}
