using Mono.CecilX;
using System.Collections.Generic;
using Unity.CompilationPipeline.Common.Diagnostics;

namespace SadJamEditor.Weaver
{
    public class ILPostProcessorLogger : Logger
    {
        internal List<DiagnosticMessage> Logs = new List<DiagnosticMessage>();

        void Add(string message, DiagnosticType logType)
        {
            Logs.Add(new DiagnosticMessage
            {
                DiagnosticType = logType,
                File = null,
                Line = 0,
                Column = 0,
                MessageData = message
            });
        }

        public void LogDiagnostics(string message, DiagnosticType logType = DiagnosticType.Warning)
        {
            string[] lines = message.Split('\n');

            if (lines.Length == 1)
            {
                Add($"{message}", logType);
            }
            else
            {
                Add("----------------------------------------------", logType);
                foreach (string line in lines) Add(line, logType);
                Add("----------------------------------------------", logType);
            }
        }

        public void Warning(string message) => Warning(message, null);
        public void Warning(string message, MemberReference mr)
        {
            if (mr != null) message = $"{message} (at {mr})";
            LogDiagnostics(message, DiagnosticType.Warning);
        }

        public void Error(string message) => Error(message, null);
        public void Error(string message, MemberReference mr)
        {
            if (mr != null) message = $"{message} (at {mr})";
            LogDiagnostics(message, DiagnosticType.Error);
        }
    }
}
