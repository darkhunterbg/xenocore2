using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore;
namespace Demo.Win32.Managed
{
    class Program
    {
        static void Main(string[] args)
        {
            XenoCoreEngine.Initialize(new XenoCoreEngineConfig()
            {
                FixedMemoryAddress = new IntPtr(0x1000000)
            });
            XenoCoreEngine.MemoryManager.AllocateBlock(1024 * 1024);
            XenoCoreEngine.MemoryManager.AllocateBlock(1024 * 1024);
        }
    }
}
