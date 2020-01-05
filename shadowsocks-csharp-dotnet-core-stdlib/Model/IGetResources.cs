using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks.Std.Model
{
    public interface IGetResources
    {
        public string GetI18NCSV();

        public byte[] GetLib(string name);
    }
}
