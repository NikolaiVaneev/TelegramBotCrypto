using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotCrypto.Services
{
    public static class Logger
    {
        public static StringBuilder Logs { get; set; } = new StringBuilder();
        public static event Action OnUpdateLog;
        public static void Add(string message)
        {
            Logs.AppendLine($"{DateTime.Now:G}  {message}");
            OnUpdateLog?.Invoke();
        }
    }
}
