using System;

namespace GenericTypeOperation
{

    public struct ExtensionFunction<T>
    {

        public static T Create(params object[] parameters)
        {
            return (T)typeof(T).GetConstructor(GetTypes(parameters)).Invoke(parameters);
        }

        public static T Create(Type type, params object[] parameters)
        {
            return (T)type.GetConstructor(GetTypes(parameters)).Invoke(parameters);
        }

        public static T Invoke<T>(Object obj, String methodName, params object[] parameters)
        {
            return (T)obj.GetType().GetMethod(methodName).Invoke(obj, parameters);
        }

        private static Type[] GetTypes(params object[] parameters)
        {
            Type[] types = new Type[parameters.Length];
            int i = 0;
            foreach (object obj in parameters)
            {
                types[i] = obj.GetType();
                i++;
            }
            return types;
        }
    }
}
