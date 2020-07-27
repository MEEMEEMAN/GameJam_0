using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    public interface IConsoleCommand
    {
        string GetDescription();
        string GetCommand();
        bool Execute(string command, string[] arguments);
    }

}