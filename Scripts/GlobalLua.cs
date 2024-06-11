using NLua;
using StaticClass;

namespace KemoCard.Scripts
{
    public class GlobalLua
    {
        public static GlobalLua GetInstance()
        {
            if (_ins == null)
            {
                _ins = new GlobalLua
                {
                    lua = new Lua()
                };
                StaticUtils.InitLuaEnv(_ins.lua);
            };
            return _ins;
        }
        private static GlobalLua _ins;
        public Lua lua;

        public static void ExecuteInSandBox(string script)
        {
            GetInstance().lua.DoString(script);
        }
    }
}
