using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.Base
{
    public interface WriteCacheInterface
    {
        void setOffset(int offset, byte value);

        void setOffset(int offset, short value);

        void setOffset(int offset, int value, bool writeOnly = false);

        void setOffset(int offset, string value);

        void executeMacro(string macroName, int parameter);

        void Write();
    }
}
