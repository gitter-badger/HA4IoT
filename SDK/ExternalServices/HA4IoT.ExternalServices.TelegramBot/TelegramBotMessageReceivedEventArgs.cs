﻿using System;

namespace HA4IoT.ExternalServices.TelegramBot
{
    public class TelegramBotMessageReceivedEventArgs : EventArgs
    {
        public TelegramBotMessageReceivedEventArgs(TelegramBotService telegramBotService, TelegramInboundMessage message)
        {
            if (telegramBotService == null) throw new ArgumentNullException(nameof(telegramBotService));
            if (message == null) throw new ArgumentNullException(nameof(message));

            TelegramBotService = telegramBotService;
            Message = message;
        }

        public TelegramBotService TelegramBotService { get; }

        public TelegramInboundMessage Message { get; }
    }
}
