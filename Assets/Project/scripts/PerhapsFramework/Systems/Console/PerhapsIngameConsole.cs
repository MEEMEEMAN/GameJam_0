using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Perhaps
{
    [System.Serializable]
    public struct ConsoleConfig
    {
        public bool logDebugLogs;
        public bool logErrorLogs;
        public Color defaultColor;
        public int textPoolCount;
    }

    public class PerhapsIngameConsole : MonoBehaviour
    {
        [SerializeField] ConsoleConfig config;
        [SerializeField] TextMeshProUGUI textPrefab;
        [SerializeField] RectTransform textHolder;
        [SerializeField] TMP_InputField input;
        public static PerhapsIngameConsole instance { get; private set; }
        Dictionary<string, IConsoleCommand> consoleCommands = new Dictionary<string, IConsoleCommand>();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                return;
            }

            Init();
            RegisterStandardCommands();
        }

        void RegisterStandardCommands()
        {
            RegisterCommand(new HelpCommand(consoleCommands));
            RegisterCommand(new ClearCommand());
        }


        public void Update()
        {
            if (dirty)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(textHolder);
                dirty = false;
            }
        }

        public void RegisterCommand(IConsoleCommand cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            string command = cmd.GetCommand();
            if (consoleCommands.ContainsKey(command))
            {
                Debug.Log($"Console already has {command} registered! replacing");
                consoleCommands[command] = cmd;
                return;
            }

            consoleCommands.Add(cmd.GetCommand(), cmd);
        }

        public void UnregisterCommand(IConsoleCommand cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            consoleCommands.Remove(cmd.GetCommand());
        }

        Queue<TextMeshProUGUI> textPool = new Queue<TextMeshProUGUI>();
        void Init()
        {
            for (int i = 0; i < config.textPoolCount; i++)
            {
                TextMeshProUGUI txt = Instantiate(textPrefab, textHolder);
                txt.enabled = false;

                textPool.Enqueue(txt);
            }

            Application.logMessageReceived += OnLogReceive;
            input.onSubmit.AddListener(InputField);
        }

        private void OnLogReceive(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Warning:
                case LogType.Log:
                    if (config.logDebugLogs)
                    {
                        ConsolePrint(condition);
                    }
                    break;
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    if (config.logErrorLogs)
                    {
                        ConsolePrint(condition, Color.red);
                    }
                    break;
                default:
                    break;
            }
        }

        public void ConsolePrint(string msg)
        {
            ConsolePrint(msg, config.defaultColor);
        }

        bool dirty = false;
        int msgReel = 0;
        public void ConsolePrint(string msg, Color color)
        {
            TextMeshProUGUI txt = textPool.Dequeue();
            txt.text = $"{++msgReel}> {msg}";
            txt.color = color;
            txt.enabled = true;
            txt.transform.SetAsLastSibling();

            dirty = true;

            textPool.Enqueue(txt);
        }

        public void ClearConsole()
        {
            msgReel = 0;
            for (int i = 0; i < config.textPoolCount; i++)
            {
                TextMeshProUGUI txt = textPool.Dequeue();
                txt.enabled = false;
                textPool.Enqueue(txt);
            }
        }

        void InputField(string cmd)
        {
            ConsolePrint(cmd);
            CommandInput(cmd);
        }

        public void CommandInput(string command, params string[] arguments)
        {
            if (consoleCommands.TryGetValue(command, out IConsoleCommand cmd))
            {
                if (cmd.Execute(command, arguments))
                {

                }
                else
                {
                    ConsolePrint($"Failed to execute command \"{command}\"");
                }
            }
            else
            {
                ConsolePrint($"No such command \"{command}\" registered.");
            }
        }

        public void CommandInput(string rawCommand)
        {
            if (ParseCommand(rawCommand, out string cmd, out string[] args))
            {
                CommandInput(cmd, args);
            }
            else
            {
                ConsolePrint($"Failed to parse {rawCommand}");
            }
        }

        bool ParseCommand(string rawCommand, out string command, out string[] arguments)
        {
            if (string.IsNullOrEmpty(rawCommand))
            {
                command = null;
                arguments = null;
                return false;
            }

            string[] parts = rawCommand.Split(' ');
            command = parts[0];

            if (parts.Length == 1)
            {
                arguments = null;
                return true;
            }

            arguments = new string[parts.Length - 1];
            Array.Copy(parts, 1, arguments, 0, arguments.Length);
            return true;
        }
    }

    public class HelpCommand : IConsoleCommand
    {
        readonly Dictionary<string, IConsoleCommand> _commands;

        public HelpCommand(Dictionary<string, IConsoleCommand> cmds)
        {
            _commands = cmds;
        }

        public bool Execute(string command, string[] arguments)
        {
            if (arguments == null)
            {
                foreach (var item in _commands)
                {
                    PerhapsIngameConsole.instance.ConsolePrint($"{item.Key}: {item.Value.GetDescription()}");
                }

            }
            else
            {
                foreach (var item in arguments)
                {
                    if (_commands.TryGetValue(item, out IConsoleCommand cmd))
                    {
                        PerhapsIngameConsole.instance.ConsolePrint(($"{item}: {cmd.GetDescription()}"));
                    }
                    else
                    {
                        PerhapsIngameConsole.instance.ConsolePrint($"No such command \"{item}\" registered.");
                    }
                }
            }

            return true;
        }

        public string GetCommand()
        {
            return "help";
        }

        public string GetDescription()
        {
            return "$ help = returns a list of all commands.\n $ help {command} = returns a command's description.";
        }
    }

    public class ClearCommand : IConsoleCommand
    {
        public bool Execute(string command, string[] arguments)
        {
            PerhapsIngameConsole.instance.ClearConsole();
            return true;
        }

        public string GetCommand()
        {
            return "clear";
        }

        public string GetDescription()
        {
            return "$ clear = clears the console.";
        }
    }
}