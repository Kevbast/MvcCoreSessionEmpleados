using Newtonsoft.Json;

namespace MvcCoreSessionEmpleados.Extensions
{
    //LA CLASE TIENE QUE SER STATIC Y SUS MÉTODOS TAMBIÉN
    public static class SessionExtension
    {
        public static void SetObject(this ISession session,string key,object value)
        {
            string json = JsonConvert.SerializeObject(value);
            session.SetString(key, json);
        }

        //A LA INVERSA USANDO EL GENÉRICO
        public static T GetObject<T>(this ISession session,string key)
        {
            string data = session.GetString(key);
            if (data == null)
            {
                return default(T);
            }
            else
            {//devolvemos el objeto deserializado
                return JsonConvert.DeserializeObject<T>(data);
            }
        }

    }
}
