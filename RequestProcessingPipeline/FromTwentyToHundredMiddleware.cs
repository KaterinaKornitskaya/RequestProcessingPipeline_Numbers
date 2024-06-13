namespace RequestProcessingPipeline
{
    // по соглашению имя классов для компонентов заканч на Middleware
    public class FromTwentyToHundredMiddleware
    {
        private readonly RequestDelegate _next;

        // констурктор любого класса компонента должен принимать делегат RequestDelegate
        // в этом делегате будет ссылка на следующий компонент
        public FromTwentyToHundredMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        // следующий метод Invoke вызывается автоматически, мы не прописываем
        // где его вызывать, его вызывает ASP.NET инфраструктура, когда мы
        // в Программ прописали app.UseFromTwentyToHundred();
        public async Task Invoke(HttpContext context)  // сюда пердается контекст запроса
        {
            // из контекста нужно достать параметр намбер ( в этом приложении у нас
            // нет никаких текстареа, мы руками пишем намбер в строку поиска
            string? token = context.Request.Query["number"]; // Получим число из контекста запроса
            try
            {
                // конвертируем значение token в инт, если не сковертируется - попадем в catch
                int number = Convert.ToInt32(token);

                // получаем абсолютную величину (без учета знака ( -25 будет 25))
                number = Math.Abs(number);

                string[] Tens = { "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                switch (number)
                {
                    case (<20):
                        // если число меньше 20 - 
                        // контекст запроса передаем следующему компоненту
                        await _next.Invoke(context);
                    break;

                    case (<100):
                        // если число 20-99
                        if (number % 10 == 0)  // если число 20-99 и кратно десяти
                        {
                            // Выдаем окончательный ответ клиенту
                            await context.Response.WriteAsync(" I AM 20-99, Your number is " + Tens[number / 10 - 2]);                           
                        }
                        else  //  если число 20-99 и НЕ кратно десяти
                        {
                            // сначала отдадим следующему компоненту
                            await _next.Invoke(context); 
                            string? result = string.Empty;

                            // получим число от след.компонента FromOneToTenMiddleware
                            // младший компонент запишет в сессию ключ, и мы по нему достанем строку
                            result = context.Session.GetString("number");

                            // Выдаем окончательный ответ клиенту
                            await context.Response.WriteAsync(" I AM 20-99, Your number is " + Tens[number / 10 - 2] + " " + result);
                        }
                    break;

                    case (<=9999):
                        // если число 100-9999
                        if (number - ((number/100)*100) < 20)  // если значение десяток меньше 20
                        {
                            // контекст запроса передаем следующему компоненту
                            await _next.Invoke(context);
                        }
                        else
                        {
                            // если значение десяток равно 0
                            if((number -  ((number/100)*100)) == 0)
                            {
                                // контекст запроса передаем следующему компоненту
                                await _next.Invoke(context);
                            }
                            else if ((number -  ((number/100)*100)) % 10 == 0) 
                            {   
                            
                                // если значение десяток кратно 10
                                // записываем значение в сесионную переменную
                                context.Session.SetString("number", Tens[((number -  ((number/100)*100)) / 10) - 2]);
                            }
                            else  //  если значение десяток  НЕ кратно десяти
                            {
                                // сначала отдадим следующему компоненту
                                await _next.Invoke(context); 
                                string? result = string.Empty;
                                result = context.Session.GetString("number");
                                // записываем значение в сесионную переменную
                                context.Session.SetString("number", Tens[((number -  ((number/100)*100)) / 10) - 2] + " " + result);
                            }
                        }
                    break;
                }     
            }
            catch (Exception)
            {
                // Выдаем окончательный ответ клиенту
                await context.Response.WriteAsync("I am 20-100, Incorrect parameter");
            }
        }
    }
}
