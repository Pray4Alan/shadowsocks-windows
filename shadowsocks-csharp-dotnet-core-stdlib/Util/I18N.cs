using System.Collections.Generic;

namespace Shadowsocks.Std.Util
{
    public interface IGetI18N
    {
        public Dictionary<string, string> GetI18N(ref Dictionary<string, string> strings);
    }

    public class I18N
    {
        public const string I18N_FILE = "i18n.csv";

        private static Dictionary<string, string> _strings = new Dictionary<string, string>();

        private readonly IGetI18N i18N;

        public I18N(IGetI18N i18N)
        {
            this.i18N = i18N;
            this.i18N.GetI18N(ref _strings);
        }

        public static string GetString(string key, params object[] args) => string.Format(_strings.TryGetValue(key.Trim(), out var value) ? value : key, args);
    }
}
