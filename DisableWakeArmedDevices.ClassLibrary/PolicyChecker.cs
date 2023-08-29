using System;
using System.Security.Principal;

namespace DisableWakeArmedDevices.ClassLibrary
{
    public static class PolicyChecker
    {
        public static bool IsAdmin()
        {
            try
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent())
                    .IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}