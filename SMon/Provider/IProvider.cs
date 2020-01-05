using System;
using System.Collections.Generic;
using System.Text;

namespace SMon.Provider
{
    public interface IProvider
    {
        void Install(string[] args);
        void Uninstall();

        void Start();
        void Stop();
        void Restart();
    }
}
