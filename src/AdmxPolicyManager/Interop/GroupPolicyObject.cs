using AdmxPolicyManager.Interop;
using System;
using System.Runtime.InteropServices;

namespace AdmxPolicyManager.Interop
{
    internal sealed class GroupPolicyObject : IDisposable
    {
        internal GroupPolicyObject()
        {
            _gp = new GroupPolicyClass();
            _gpo = (IGroupPolicyObject2)_gp;
            _disposed = false;
        }

        private GroupPolicyClass _gp;
        private IGroupPolicyObject2 _gpo;
        private bool _disposed;

        ~GroupPolicyObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_gpo != null)
                {
                    Marshal.ReleaseComObject(_gpo);
                    _gpo = null;
                }

                if (_gp != null)
                {
                    Marshal.ReleaseComObject(_gp);
                    _gp = null;
                }

                _disposed = true;
            }
        }

        internal IGroupPolicyObject2 GPO
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("This object was already disposed.");
                if (_gpo == null)
                    throw new GroupPolicyManagementException("Group policy object is not ready.");
                return _gpo;
            }
        }
    }
}
