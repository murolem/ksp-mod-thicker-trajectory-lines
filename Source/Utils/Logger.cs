using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace ThickerTrajectoryLines
{
    public enum LogLevel
    {
        VERBOSE,
        DEBUG,
        // INFO,
        // WARN,
        ERROR,
    }
    
    public class Logger
    {
        public static Logger log = new Logger();
        
        private string staticPrefix = "ThickerTrajectoryLines";
        private string prefix;
        private string messagePrefix;
        private static LogLevel logLevel =  LogLevel.DEBUG;

        public static void SetLogLevel(LogLevel logLevel)
        {
            Logger.logLevel = logLevel;
        }
        
        public Logger() {}

        public Logger(string prefix)
        {
            this.prefix = prefix;
        }
        
        public Logger(string prefix, string messagePrefix)
        {
            this.prefix = prefix;
            this.messagePrefix =  messagePrefix;
        }

        public Logger Clone(string prefix = null, string messagePrefix = null)
        {
            return new Logger(prefix ?? this.prefix , messagePrefix ?? this.messagePrefix);
        }

        public Logger SetPrefix(string prefix)
        {
            this.prefix = prefix;
            return this;
        }

        public Logger SetMessagePrefix(string messagePrefix)
        {
            this.messagePrefix = messagePrefix;
            return this;
        }

        public void Verbose(string message)
        {
            Log(LogLevel.VERBOSE, message);
        }
        
        public void Debug(string message)
        {
            Log(LogLevel.DEBUG, message);
        }

        public void Error(string message)
        {
            Log(LogLevel.ERROR, message);
        }

        private void Log(LogLevel level, string message)
        {
            if((int)level < (int)Logger.logLevel)
                return;
            
            var msg = message;
            if (messagePrefix != null)
            {
                msg = messagePrefix + " " + msg;
            }

            if (prefix != null)
            {
                msg = "[" + prefix + "] " + msg;
            }

            // debug is a default log level so no need to bloat the log file by specifying it
            if (level != LogLevel.DEBUG)
            {
                msg =  "[" + level + "] " + msg;
            }
            
            msg =  "[" + staticPrefix + "] " + msg;
            
            switch (level)
            {
                case LogLevel.ERROR:
                    UnityEngine.Debug.LogError(msg);
                    break;
                default:
                    UnityEngine.Debug.Log(msg);
                    break;
            }
        }
    }
}