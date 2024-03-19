using TG_Bot_MVC.DataClasses;

namespace TG_Bot_MVC.Handlers
{
    public abstract class BaseUpdateHandler(LocalAPI localAPI)
    {
        public readonly LocalAPI localAPI = localAPI;
        public readonly BaseUpdateHandler? nextHandler;

        public abstract bool Active(UserUpdate userUpdate);

        public bool? CallNextHandler(UserUpdate userUpdate)
        {
            return Active(userUpdate);
        }
    }
}
