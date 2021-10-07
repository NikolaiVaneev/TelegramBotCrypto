using System.Windows;
using TelegramBotCrypto.Infrastructure.Commands.Base;

namespace TelegramBotCrypto.Infrastructure.Commands
{
    internal class CloseApplicationCommand : Command
    {
        public override bool CanExecute(object parameter) => true;
        public override void Execute(object parameter) => Application.Current.Shutdown();
    }
}
