namespace LightSourceProgram
{
    public class JLLightFactoryManager : LightFactoryManager//
    {
        public override Light CreateLight(LightType type)
        {
           Light light=null;
            switch (type)
            {
                case LightType.RS232:
                    light=new JLRS232Light();
                    break;
                case LightType.RS485:
                    light = new JLRS485Light();
                    break;
                case LightType.NET:
                    light = new JLNETLight();
                    break;                
                default:
                    break;
            }
            return light;
        }
    }
}
