using Qiwi.BillPayments.Client;
using Qiwi.BillPayments.Model;
using Qiwi.BillPayments.Model.In;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Text.Json;
using File = System.IO.File;
using Timer = System.Timers.Timer;
using System;
using System.Timers;
using System.Collections.Generic;
using Qiwi.BillPayments.Exception;
using Qiwi.BillPayments.Model.Out;

namespace TelegramPaymentQiwiBot
{
    internal class TelegramBotPayment
    {
        public TelegramBotClient BotClient { get; private set; }
        private BillPaymentsClient BillClient;
        private string PublicKey;
        
        private IList<BaseOffer> Offers;
        private IList<Order>? Orders;
        public long ReportEntityId { get; set; }
        public string UsersFileName { get; set; } = "savedOrders.json";
        private Timer KickTiemer = new Timer();
        public TelegramBotPayment(string secretKey, string publicKey, string botToken, long reportId, IList<BaseOffer> offers)
        {
            Orders = File.Exists(UsersFileName)
                ? JsonSerializer.Deserialize<IList<Order>>(File.ReadAllText(UsersFileName))
                : new List<Order>();
        
            KickTiemer.Interval = 15000;
            KickTiemer.Elapsed += KickTimerOnElapsed;
            KickTiemer.Start();
            
            Offers = offers;
            BillClient = BillPaymentsClientFactory.Create(secretKey: secretKey);
            PublicKey = publicKey;
            BotClient = new TelegramBotClient(botToken);
            ReportEntityId = reportId;

        }

        private void KickTimerOnElapsed(object? sender, ElapsedEventArgs e)
        {
            List<int> expired = new List<int>();
            for(int i = 0; i < Orders?.Count; i++)
            {
                if (Orders[i].Until < DateTime.Now)
                {
                    (from offer in Offers where offer.Id == Orders[i].OfferId select offer).First().TimeIsUp(BotClient, Orders[i].UserId);
                    expired.Add(i);
                }
            }
            
            expired.Reverse();
            bool update = false;
            foreach (var idx in expired)
            {
                Orders?.RemoveAt(idx);
                update = true;
            }
            if (update)
            {
                Console.WriteLine("List updated (deleted)");
                File.WriteAllText(UsersFileName, JsonSerializer.Serialize(Orders, new JsonSerializerOptions { WriteIndented = true }));
                Console.WriteLine("Base updated");
            }

        }

        public void StartHear()
        {
            BotClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync
            );
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {

            if (update.ChatJoinRequest != null)
            {
                if ((from order in Orders
                        where
                            (from offer in Offers
                                where offer is PrivateChannelInviterOffer
                                select offer.Id).Contains(order.OfferId)
                        select order.UserId).Contains(update.ChatJoinRequest.From.Id))
                {
                    try
                    {
                        await botClient.ApproveChatJoinRequest(update.ChatJoinRequest?.Chat, update.ChatJoinRequest.From.Id);
                    } catch {}
                }
            }

            if ((from offer in Offers select offer.OfferName).Contains(update.CallbackQuery?.Data))
            {
                BaseOffer currentOffer = (from offer in Offers
                    where offer.OfferName == update.CallbackQuery?.Data
                    select offer).First();
                InlineKeyboardButton payMonthPrivateButton = new InlineKeyboardButton("💳 КУПИТЬ");
                payMonthPrivateButton.CallbackData = $"{currentOffer?.OfferName}Pay";
                InlineKeyboardButton backToFullPriceButton = new InlineKeyboardButton("🔙 НАЗАД");
                backToFullPriceButton.CallbackData = "backToFullPrice";
                InlineKeyboardMarkup monthPriceMarkup =
                    new InlineKeyboardMarkup(new[] { payMonthPrivateButton, backToFullPriceButton });

                string? until = currentOffer?.Duration == TimeSpan.MaxValue 
                    ? "бессрочно"
                    : (DateTime.Now + currentOffer?.Duration).ToString();

                try
                {
                    await BotClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                        update.CallbackQuery.Message.MessageId,
                        $"Тариф: {currentOffer?.OfferName}\n" +
                        $"Стоимость: {currentOffer?.Price} {currentOffer?.Currency} \n" +
                        $"Срок действия до: {until} \n" +
                        currentOffer?.Comment,
                        replyMarkup: monthPriceMarkup);
                } catch {}
            }

            if ((from offer in Offers select $"{offer.OfferName}Pay").Contains(update.CallbackQuery?.Data))
            {
                BaseOffer currentOffer = (from offer in Offers
                    where $"{offer.OfferName}Pay" == update.CallbackQuery?.Data
                    select offer).First();

                if ((from order in Orders where order.UserId == update.CallbackQuery?.From.Id select order.OfferId)
                    .Contains(currentOffer.Id))
                {
                    await BotClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                        update.CallbackQuery.Message.MessageId,
                        "❌ Эта услуга у вас уже куплена, повторите попытку когда срок ее действия истечет");
                }
                else
                {
                    InlineKeyboardButton confirmPayButton = new InlineKeyboardButton("✅ ОПЛАТИТЬ");
                    confirmPayButton.CallbackData = $"{currentOffer.OfferName}Confirm";
                    InlineKeyboardButton backToOfferButton = new InlineKeyboardButton("❌ ОТМЕНА");
                    backToOfferButton.CallbackData = currentOffer?.OfferName;
                    InlineKeyboardMarkup confirmPayMonthMarkup =
                        new InlineKeyboardMarkup(new[] { confirmPayButton, backToOfferButton });

                    try
                    {
                        await BotClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                            update.CallbackQuery.Message.MessageId,
                            "✅ Счёт на оплату сформирован. Запросы к услугам будут выполнены как только вы оплатите его.",
                            replyMarkup: confirmPayMonthMarkup);
                    } catch {}

                }
            }

            if ((from offer in Offers select $"{offer.OfferName}Confirm").Contains(update.CallbackQuery?.Data))
            {
                BaseOffer currentOffer = (from offer in Offers
                    where $"{offer.OfferName}Confirm" == update.CallbackQuery?.Data
                    select offer).First();
                string billId = Guid.NewGuid().ToString();
                Uri paymentLink = BillClient.CreatePaymentForm(
                    paymentInfo: new PaymentInfo()
                    {
                        PublicKey = PublicKey,
                        Amount = new MoneyAmount()
                        {
                            ValueDecimal = currentOffer.Price,
                            CurrencyEnum = currentOffer.Currency
                        },
                        BillId = billId,
                    }
                );

                if (currentOffer is PrivateChannelInviterOffer)
                {
                    PrivateChannelInviterOffer inviteOffer = currentOffer as PrivateChannelInviterOffer;
                    try
                    {
                        if (inviteOffer.ChannelInviteLink == null)
                        {
                            inviteOffer.ChannelInviteLink = await BotClient.CreateChatInviteLinkAsync(
                                new ChatId((long)inviteOffer.Channel!),
                                "Bot`s link",
                                null,
                                null,
                                true,
                                cancellationToken);
                        }

                        await BotClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                            update.CallbackQuery.Message.MessageId,
                            $"✅ Вот ваш счет на оплату: {paymentLink} \n" +
                            $"✅ Как только оплата будет совершена вы можете вступить в чат по ссылке: {inviteOffer.ChannelInviteLink.InviteLink}");
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                }

                CheckPayment(new Order(update.CallbackQuery.From.Id, currentOffer.Id, 
                    currentOffer.Duration == TimeSpan.MaxValue ? DateTime.MaxValue : DateTime.Now + currentOffer.Duration), billId);
            }

            if (update.CallbackQuery?.Data == "backToFullPrice")
            {
                List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
                foreach (BaseOffer offer in Offers)
                {
                    buttons.Add(new InlineKeyboardButton(offer.OfferName));
                    buttons[^1].CallbackData = offer.OfferName;
                }

                InlineKeyboardMarkup priceMarkUp = new InlineKeyboardMarkup(buttons);

                try
                {
                    await BotClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                        update.CallbackQuery.Message.MessageId,
                        "Чтобы ознакомиться с тарифом, выберите необходимый, нажав на соответствующую кнопку 🔥",
                        replyMarkup: priceMarkUp);

                } catch {}
            }

            if (update.Message?.Text == "/start" || update.Message?.Text == "💵 Тарифы")
            {
                List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
                foreach (BaseOffer offer in Offers)
                {
                    buttons.Add(new InlineKeyboardButton(offer.OfferName));
                    buttons[^1].CallbackData = offer.OfferName;
                }

                InlineKeyboardMarkup priceMarkUp = new InlineKeyboardMarkup(buttons);

                ReplyKeyboardMarkup keyboardMarkup =
                    new(new[] { new KeyboardButton[] { "💵 Тарифы", "⏳ Моя подписка" } })
                        { ResizeKeyboard = true };

                try
                {
                    await BotClient.SendTextMessageAsync(update.Message.Chat,
                        "Чтобы ознакомиться с тарифом, выберите необходимый, нажав на соответствующую кнопку 🔥",
                        replyMarkup: priceMarkUp);
                    await BotClient.SendTextMessageAsync(update.Message.Chat,
                        "Чтобы ознакомиться со статусом аккаунта - выберите кнопку на клавиатуре 🔥",
                        replyMarkup: keyboardMarkup);
                }
                catch {}
            }

            if (update.Message?.Text == "⏳ Моя подписка")
            {
                if ((from order in Orders select order.UserId).Contains(update.Message.From.Id))
                {
                    string msg = "Ваши активные тарифы: \n";
                    foreach (var order in (from order in Orders
                                 where order.UserId == update.Message.From.Id
                                 select order))
                    {
                        BaseOffer offer = (from off in Offers where off.Id == order.OfferId select off).First();
                        msg += $"{offer.OfferName} - активнo до {order.Until}\n";
                    }

                    try
                    {
                        await BotClient.SendTextMessageAsync(update.Message.Chat, msg);
                    } catch {}
                }

                else
                {
                    InlineKeyboardButton button = new InlineKeyboardButton("КУПИТЬ ✅");
                    button.CallbackData = "backToFullPrice";

                    try
                    {
                        await BotClient.SendTextMessageAsync(update.Message.Chat,
                            "⌛️ У Вас нет действующей подписки. \n" +
                            "Ознакомьтесь с тарифами, нажав на соответствующую кнопку.",
                            replyMarkup: new InlineKeyboardMarkup(button));
                    } catch {}
                }
            }
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }

        private void CheckPayment(Order order, string billId)
        {
            Task.Run(async () =>
            {
                for (int i = 0; i < 600; i++, Thread.Sleep(1000))
                {
                    BillResponse value = new BillResponse();
                    try
                    {
                        value = await BillClient.GetBillInfoAsync(billId);
                    }
                    catch (BillPaymentsServiceException e) 
                    {
                        Console.WriteLine(e.Response);
                    }
                    if (value.Status != null && value.Status.ValueEnum == BillStatusEnum.Paid)
                    {
                        BaseOffer offer = (from off in Offers where off.Id == order.OfferId select off).First();
                        offer.PayConfirmed(BotClient, order.UserId);
                        Chat chat = await BotClient.GetChatAsync(order.UserId);

                        try
                        {
                            await BotClient.SendTextMessageAsync(ReportEntityId, $"Была приобретена услуга \"{offer.OfferName}\" пользователем @{chat.Username}");
                            await BotClient.SendTextMessageAsync(order.UserId, $"Была приобретена услуга \"{offer.OfferName}\"");
                        } catch {}

                        lock (new object())
                        {
                            Orders.Add(order);
                            File.WriteAllText(UsersFileName, JsonSerializer.Serialize(Orders, new JsonSerializerOptions { WriteIndented = true }));
                            Console.WriteLine("offer saved");
                            break;
                        }
                    }
                }
            });
        }
    }
}
