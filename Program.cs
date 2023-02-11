using System;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Drawing;

namespace SushiMix
{
    internal class Program
    {

        static Image img = Bitmap.FromFile("images/zagotovka.png"); //путь к картинке
         static Graphics g = Graphics.FromImage(img);
        static TelegramBotClient Bot = new TelegramBotClient("5710464764:AAGhJxXXnpp7PUL_xHaYt2M7IbTVGJAq9aw");
        static DataBase db = new DataBase();
       
        static void Main(string[] args)
        {
            Bot.StartReceiving(Update, Error);
            while (true)
                Console.ReadKey();
        }

        private static async Task Update(ITelegramBotClient bot, Update update, CancellationToken arg3)
        {
            return;
            if (update.Message != null)
            {
                if (update.Message.Location != null)
                {
                    List<Product> korzina = db.Tovarsinkorzina(update.Message.From.Id);
                    if (korzina == null || korzina.Count == 0)
                    {
                        await bot.SendTextMessageAsync(update.Message.From.Id, "Ваша корзина пустая.");
                      

                    }
                    else
                    {
                        string zakaz = "СушиМикс\n";
                        int allprice = 0;
                        for (int i = 0; i < korzina.Count; i++)
                        {
                            zakaz += korzina[i].name + " " + korzina[i].count + " шт " + korzina[i].price + " " + (korzina[i].count * korzina[i].price) + "\n";
                            allprice += (korzina[i].count * korzina[i].price);
                        }
                        zakaz += "Общая сумма: " + allprice;
                        await bot.SendTextMessageAsync(update.Message.From.Id, zakaz);
                        g.DrawString("12345", new Font("Verdana", (float)30), new SolidBrush(Color.Black), 190, 8);
                        using (var s1 = System.IO.File.OpenRead("otvet_new.jpg"))
                            await Bot.SendPhotoAsync(update.Message.From.Id, s1);

                        await bot.SendTextMessageAsync(-611761263, "Новый заказ:\n"+update.Message.From.Username+" "+update.Message.From.FirstName+"\n"+zakaz);
                    }
                }

                if (update.Message.Text != null)

                    switch (update.Message.Text)
                    {

                        case "/start":


                            mainMethod(bot, update);


                            break;
                        case "chatid":

                            await bot.SendTextMessageAsync(update.Message.Chat.Id, update.Message.Chat.Id.ToString());
                            break;
                        case "Меню":

                            category(bot, update);


                            break;
                        case "Корзина":
                            List<Product> korzina = db.Tovarsinkorzina(update.Message.From.Id);
                            if (korzina == null || korzina.Count == 0)
                            {
                                await bot.SendTextMessageAsync(update.Message.From.Id, "Ваша корзина пустая.");
                                break;

                            }
                            await bot.SendTextMessageAsync(update.Message.Chat.Id, "Вы попали в раздел корзина");
                            Console.WriteLine(korzina);
                            for (int i = 0; i < korzina.Count; i++)
                            {
                                var klava = (new[] {


                new[]{
                     InlineKeyboardButton.WithCallbackData(" - ","0") ,
                    InlineKeyboardButton.WithCallbackData(korzina[i].count.ToString(),korzina[i].id.ToString()) ,// Будем передаввать команду AddTovarInKorzina
                    InlineKeyboardButton.WithCallbackData(" + ","1")
                }


            });

                                InlineKeyboardMarkup replyKeyboard = new InlineKeyboardMarkup(klava);
                                await bot.SendTextMessageAsync(update.Message.From.Id, korzina[i].name + " Price: " + korzina[i].price + " count: " + korzina[i].count + " " + korzina[i].img, replyMarkup: replyKeyboard);
                            }
                            var zakaz = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Оформить заказ", "Оформить заказ"));
                            await bot.SendTextMessageAsync(update.Message.From.Id, "Желаете подтвердить заказ?", replyMarkup: zakaz);

                            break;
                        case "О боте":
                            await bot.SendTextMessageAsync(update.Message.Chat.Id, "Это будет бот про пиццу/суши, что то сама тут напиши на эту тему");

                            break;
                        case "Владелец Бота":
                            await bot.SendTextMessageAsync(update.Message.Chat.Id, "https://t.me/anyaaaaaa0a");

                            break;
                        case "Назад":
                            mainMethod(bot, update);
                            break;

                        case "Ролы":
                            Rolls(bot, update);
                            break;

                        case "Суши":
                            Susi(bot, update);
                            break;

                        case "Маки":
                            Makki(bot, update);
                            break;

                        case "Салати та супи":
                            Salatu_and_sups(bot, update);
                            break;

                        case "Сашими":
                            Sashimi(bot, update);
                            break;

                        case "Сеты":
                            Setti(bot, update);
                            break;

                        case "Пицца 30см":
                            Pizza_30(bot, update);
                            break;

                        case "Пицца 45см":
                            Pizza_45(bot, update);
                            break;

                        case "Соки":
                            Soki(bot, update);
                            break;

                        case "Вода":
                            Woter(bot, update);
                            break;

                        case "Пиво":
                            Beer(bot, update);
                            break;



                        default:
                            await bot.SendTextMessageAsync(update.Message.Chat.Id, "Я не знаю что вам ответить");

                            break;


                    }
            }
            if (update.CallbackQuery != null)
            {
                switch (update.CallbackQuery.Data)
                {
                    case "0":
                        {

                            string text = update.CallbackQuery.Message.Text;
                            // Photo photo = x.Photo;
                            int count_tovar = Convert.ToInt32(update.CallbackQuery.Message.ReplyMarkup.InlineKeyboard.ToList()[0].ToList()[1].Text);
                            //int tovar_data = Convert.ToInt32(update.CallbackQuery.Message.ReplyMarkup.InlineKeyboard.ToList()[0].ToList()[1].CallbackData);
                            if (count_tovar > 0)
                            {
                                count_tovar--;
                                int tovarid = Convert.ToInt32(update.CallbackQuery.Message.ReplyMarkup.InlineKeyboard.ToList()[0].ToList()[1].CallbackData);
                                db.Delete(update.CallbackQuery.From.Id, tovarid);
                          
                            var klava = (new[] {


                new[]{
                     InlineKeyboardButton.WithCallbackData(" - ","0") ,
                    InlineKeyboardButton.WithCallbackData(count_tovar.ToString(),tovarid.ToString()) ,// Будем передаввать команду AddTovarInKorzina
                    InlineKeyboardButton.WithCallbackData(" + ","1")
                }


            });

                            InlineKeyboardMarkup replyKeyboard = new InlineKeyboardMarkup(klava);

                            await bot.EditMessageReplyMarkupAsync(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, replyMarkup: replyKeyboard); ;
                        }
                            else return;
                }
                        break;
                    case "1":
                        {
                            string text = update.CallbackQuery.Message.Text;
                            // Photo photo = x.Photo;
                            int count_tovar = Convert.ToInt32(update.CallbackQuery.Message.ReplyMarkup.InlineKeyboard.ToList()[0].ToList()[1].Text);
                            //int tovar_data = Convert.ToInt32(update.CallbackQuery.Message.ReplyMarkup.InlineKeyboard.ToList()[0].ToList()[1].CallbackData);
                            count_tovar++;
                            int tovarid = Convert.ToInt32(update.CallbackQuery.Message.ReplyMarkup.InlineKeyboard.ToList()[0].ToList()[1].CallbackData);
                            db.Insert(update.CallbackQuery.From.Id, update.CallbackQuery.From.Username, update.CallbackQuery.From.FirstName, update.CallbackQuery.From.LastName, 
                                tovarid);


                            var klava = (new[] {


                new[]{
                    InlineKeyboardButton.WithCallbackData(" - ","0") ,
                    InlineKeyboardButton.WithCallbackData(count_tovar.ToString(),tovarid.ToString()) ,// Будем передаввать команду AddTovarInKorzina
                    InlineKeyboardButton.WithCallbackData(" + ","1")
                }


            });

                            InlineKeyboardMarkup replyKeyboard = new InlineKeyboardMarkup(klava);

                            await bot.EditMessageReplyMarkupAsync(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, replyMarkup: replyKeyboard); ;
                        }
                        break;
                    case "Оформить заказ":

                        await bot.SendTextMessageAsync(update.CallbackQuery.From.Id, "Укажите адрес доставки");
                       
                        break;
                  
                }
            }


        }
        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }



        static async void mainMethod(ITelegramBotClient bot, Update update)
        {
            var replyKeyboard = new ReplyKeyboardMarkup((new[]

                          {
                                new[]
                                    {

                                        new KeyboardButton("Меню"),
                                         new KeyboardButton("Корзина"),

                                    },
                                new[]
                                    {
                                        new KeyboardButton("О боте"),
                                        new KeyboardButton("Владелец Бота"),
                                    },

                                 }));

            replyKeyboard.ResizeKeyboard = true;

            await Bot.SendTextMessageAsync(update.Message.Chat.Id, "Добро пожаловать", replyMarkup: replyKeyboard);

        }
        static async void category(ITelegramBotClient bot, Update update)
        {

            var replyKeyboard1 = new ReplyKeyboardMarkup((new[]

               {
                                new[]
                                    {

                                        new KeyboardButton("Назад"),
                                         new KeyboardButton("Ролы"),
                                              new KeyboardButton("Маки"),

                                    },
                                new[]
                                    {
                                        new KeyboardButton("Сашими"),
                                        new KeyboardButton("Сеты"),
                                           new KeyboardButton("Суши"),
                                    },
                                 new[]
                                    {
                                        new KeyboardButton("Пицца 30см"),
                                        new KeyboardButton("Пицца 45см"),
                                           new KeyboardButton("Соки"),
                                    },
                                 new[]
                                    {
                                        new KeyboardButton("Вода"),
                                        new KeyboardButton("Пиво"),
                                  },

                                 }));


            replyKeyboard1.ResizeKeyboard = true;

            await Bot.SendTextMessageAsync(update.Message.Chat.Id, "Добро пожаловать", replyMarkup: replyKeyboard1);
        }
        static async void Rolls(ITelegramBotClient bot, Update update)
        {
            List<Product> rolls = db.rolu;
            Console.WriteLine(rolls.Count);
            for (int i = 0; i < rolls.Count; i++)
            {
                var klava = (new[] {


                new[]{
                    InlineKeyboardButton.WithCallbackData(" - ","0") ,
                    InlineKeyboardButton.WithCallbackData("0",rolls[i].id.ToString()) ,// Будем передаввать команду AddTovarInKorzina
                    InlineKeyboardButton.WithCallbackData(" + ","1")
                }


            });

                InlineKeyboardMarkup replyKeyboard = new InlineKeyboardMarkup(klava);

                await bot.SendPhotoAsync(update.Message.From.Id, photo: rolls[i].img.ToString(), rolls[i].name + "\n" + rolls[i].description + "\n" + rolls[i].price + " грн", replyMarkup: replyKeyboard);
                //   await Bot.SendTextMessageAsync(update.Message.From.Id, rolls[i].name, replyMarkup: replyKeyboard);
            }
        }



        static async void Beer(ITelegramBotClient bot, Update update)
        {
            List<Product> beer = db.pivo;
            Console.WriteLine(beer.Count);
            for (int i = 0; i < beer.Count; i++)
            {
                var klava = (new[] {


                new[]{
                    InlineKeyboardButton.WithCallbackData(" - ","0") ,
                    InlineKeyboardButton.WithCallbackData("0",beer[i].id.ToString()) ,// Будем передаввать команду AddTovarInKorzina
                    InlineKeyboardButton.WithCallbackData(" + ","1")
                }


            });

                InlineKeyboardMarkup replyKeyboard = new InlineKeyboardMarkup(klava);

                await bot.SendPhotoAsync(update.Message.From.Id, photo: beer[i].img.ToString(), beer[i].name + "\n" + beer[i].description + "\n" + beer[i].price + " грн", replyMarkup: replyKeyboard);
                //   await Bot.SendTextMessageAsync(update.Message.From.Id, rolls[i].name, replyMarkup: replyKeyboard);
            }
        }




        static async void Woter(ITelegramBotClient bot, Update update)
        {
            List<Product> woter = db.voda;
            Console.WriteLine(woter.Count);
            for (int i = 0; i < woter.Count; i++)
            {
                var klava = (new[] {


                new[]{
                    InlineKeyboardButton.WithCallbackData(" - ","0") ,
                    InlineKeyboardButton.WithCallbackData("0",woter[i].id.ToString()) ,// Будем передаввать команду AddTovarInKorzina
                    InlineKeyboardButton.WithCallbackData(" + ","1")
                }


            });

                InlineKeyboardMarkup replyKeyboard = new InlineKeyboardMarkup(klava);

                await bot.SendPhotoAsync(update.Message.From.Id, photo: woter[i].img.ToString(), woter[i].name + "\n" + woter[i].description + "\n" + woter[i].price + " грн", replyMarkup: replyKeyboard);
                //   await Bot.SendTextMessageAsync(update.Message.From.Id, rolls[i].name, replyMarkup: replyKeyboard);
            }
        }




        static async void Soki(ITelegramBotClient bot, Update update)
        {
            List<Product> soki = db.soki;
            Console.WriteLine(soki.Count);
            for (int i = 0; i < soki.Count; i++)
            {
                var klava = (new[] {


                new[]{
                    InlineKeyboardButton.WithCallbackData(" - ","0") ,
                    InlineKeyboardButton.WithCallbackData("0",soki[i].id.ToString()) ,// Будем передаввать команду AddTovarInKorzina
                    InlineKeyboardButton.WithCallbackData(" + ","1")
                }


            });

                InlineKeyboardMarkup replyKeyboard = new InlineKeyboardMarkup(klava);

                await bot.SendPhotoAsync(update.Message.From.Id, photo: soki[i].img.ToString(), soki[i].name + "\n" + soki[i].description + "\n" + soki[i].price + " грн", replyMarkup: replyKeyboard);
                //   await Bot.SendTextMessageAsync(update.Message.From.Id, rolls[i].name, replyMarkup: replyKeyboard);
            }
        }




        static async void Pizza_45(ITelegramBotClient bot, Update update)
        {
            List<Product> pizza_45 = db.pizza45sm;
            Console.WriteLine(pizza_45.Count);
            for (int i = 0; i < pizza_45.Count; i++)
            {
                var klava = (new[] {


                new[]{
                    InlineKeyboardButton.WithCallbackData(" - ","0") ,
                    InlineKeyboardButton.WithCallbackData("0",pizza_45[i].id.ToString()) ,// Будем передаввать команду AddTovarInKorzina
                    InlineKeyboardButton.WithCallbackData(" + ","1")
                }


            });

                InlineKeyboardMarkup replyKeyboard = new InlineKeyboardMarkup(klava);

                await bot.SendPhotoAsync(update.Message.From.Id, photo: pizza_45[i].img.ToString(), pizza_45[i].name + "\n" + pizza_45[i].description + "\n" + pizza_45[i].price + " грн", replyMarkup: replyKeyboard);
                //   await Bot.SendTextMessageAsync(update.Message.From.Id, rolls[i].name, replyMarkup: replyKeyboard);
            }
        }





        static async void Pizza_30(ITelegramBotClient bot, Update update)
        {
            List<Product> pizza_30 = db.pizza30sm;
            Console.WriteLine(pizza_30.Count);
            for (int i = 0; i < pizza_30.Count; i++)
            {
                var klava = (new[] {


                new[]{
                    InlineKeyboardButton.WithCallbackData(" - ","0") ,
                    InlineKeyboardButton.WithCallbackData("0",pizza_30[i].id.ToString()) ,// Будем передаввать команду AddTovarInKorzina
                    InlineKeyboardButton.WithCallbackData(" + ","1")
                }


            });

                InlineKeyboardMarkup replyKeyboard = new InlineKeyboardMarkup(klava);

                await bot.SendPhotoAsync(update.Message.From.Id, photo: pizza_30[i].img.ToString(), pizza_30[i].name + "\n" + pizza_30[i].description + "\n" + pizza_30[i].price + " грн", replyMarkup: replyKeyboard);
                //   await Bot.SendTextMessageAsync(update.Message.From.Id, rolls[i].name, replyMarkup: replyKeyboard);
            }
        }



        
        static async void Setti(ITelegramBotClient bot, Update update)
        {
            List<Product> setu = db.setu;
            Console.WriteLine(setu.Count);
            for (int i = 0; i < setu.Count; i++)
            {
                var klava = (new[] {


                new[]{
                    InlineKeyboardButton.WithCallbackData(" - ","0") ,
                    InlineKeyboardButton.WithCallbackData("0",setu[i].id.ToString()) ,// Будем передаввать команду AddTovarInKorzina
                    InlineKeyboardButton.WithCallbackData(" + ","1")
                }


            });

                InlineKeyboardMarkup replyKeyboard = new InlineKeyboardMarkup(klava);

                await bot.SendPhotoAsync(update.Message.From.Id, photo: setu[i].img.ToString(), setu[i].name + "\n" + setu[i].description + "\n" + setu[i].price + " грн", replyMarkup: replyKeyboard);
                //   await Bot.SendTextMessageAsync(update.Message.From.Id, rolls[i].name, replyMarkup: replyKeyboard);
            }
        }



        static async void Sashimi(ITelegramBotClient bot, Update update)
        {
            List<Product> sashimi = db.sashimi;
            Console.WriteLine(sashimi.Count);
            for (int i = 0; i < sashimi.Count; i++)
            {
                var klava = (new[] {


                new[]{
                    InlineKeyboardButton.WithCallbackData(" - ","0") ,
                    InlineKeyboardButton.WithCallbackData("0",sashimi[i].id.ToString()) ,// Будем передаввать команду AddTovarInKorzina
                    InlineKeyboardButton.WithCallbackData(" + ","1")
                }


            });

                InlineKeyboardMarkup replyKeyboard = new InlineKeyboardMarkup(klava);

                await bot.SendPhotoAsync(update.Message.From.Id, photo: sashimi[i].img.ToString(), sashimi[i].name + "\n" + sashimi[i].description + "\n" + sashimi[i].price + " грн", replyMarkup: replyKeyboard);
                //   await Bot.SendTextMessageAsync(update.Message.From.Id, rolls[i].name, replyMarkup: replyKeyboard);
            }
        }





        static async void Salatu_and_sups(ITelegramBotClient bot, Update update)
        {
            List<Product> salat_sups = db.salat;
            Console.WriteLine(salat_sups.Count);
            for (int i = 0; i < salat_sups.Count; i++)
            {
                var klava = (new[] {


                new[]{
                    InlineKeyboardButton.WithCallbackData(" - ","0") ,
                    InlineKeyboardButton.WithCallbackData("0",salat_sups[i].id.ToString()) ,// Будем передаввать команду AddTovarInKorzina
                    InlineKeyboardButton.WithCallbackData(" + ","1")
                }


            });

                InlineKeyboardMarkup replyKeyboard = new InlineKeyboardMarkup(klava);

                await bot.SendPhotoAsync(update.Message.From.Id, photo: salat_sups[i].img.ToString(), salat_sups[i].name + "\n" + salat_sups[i].description + "\n" + salat_sups[i].price + " грн", replyMarkup: replyKeyboard);
                //   await Bot.SendTextMessageAsync(update.Message.From.Id, rolls[i].name, replyMarkup: replyKeyboard);
            }
        }




        static async void Makki(ITelegramBotClient bot, Update update)
        {
            List<Product> makki = db.maki;
            Console.WriteLine(makki.Count);
            for (int i = 0; i < makki.Count; i++)
            {
                var klava = (new[] {


                new[]{
                    InlineKeyboardButton.WithCallbackData(" - ","0") ,
                    InlineKeyboardButton.WithCallbackData("0",makki[i].id.ToString()) ,// Будем передаввать команду AddTovarInKorzina
                    InlineKeyboardButton.WithCallbackData(" + ","1")
                }


            });

                InlineKeyboardMarkup replyKeyboard = new InlineKeyboardMarkup(klava);

                await bot.SendPhotoAsync(update.Message.From.Id, photo: makki[i].img.ToString(), makki[i].name + "\n" + makki[i].description + "\n" + makki[i].price + " грн", replyMarkup: replyKeyboard);
                //   await Bot.SendTextMessageAsync(update.Message.From.Id, rolls[i].name, replyMarkup: replyKeyboard);
            }
        }

        static async void Susi(ITelegramBotClient bot, Update update)
        {
            List<Product> susi = db.susi;
            Console.WriteLine(susi.Count);
            for (int i = 0; i < susi.Count; i++)
            {
                var klava = (new[] {


                new[]{
                    InlineKeyboardButton.WithCallbackData(" - ","0") ,
                    InlineKeyboardButton.WithCallbackData("0",susi[i].id.ToString()) ,// Будем передаввать команду AddTovarInKorzina
                    InlineKeyboardButton.WithCallbackData(" + ","1")
                }


            });

                InlineKeyboardMarkup replyKeyboard = new InlineKeyboardMarkup(klava);

                await bot.SendPhotoAsync(update.Message.From.Id, photo: susi[i].img.ToString(), susi[i].name + "\n" + susi[i].description + "\n" + susi[i].price + " грн", replyMarkup: replyKeyboard);
                //   await Bot.SendTextMessageAsync(update.Message.From.Id, rolls[i].name, replyMarkup: replyKeyboard);
            }
        }
    }
}
