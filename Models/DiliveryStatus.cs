using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotCrypto.Models
{
    class DiliveryStatus
    {
        public DiliveryStatus(User user, TaskStatus status)
        {
            User = user;
            Status = status;
        }

        public User User { get; private set; }
        public TaskStatus Status { get; private set; }
    }
}
