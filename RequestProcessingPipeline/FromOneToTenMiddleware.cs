namespace RequestProcessingPipeline
{
    public class FromOneToTenMiddleware
    {
        private readonly RequestDelegate _next;

        public FromOneToTenMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string? token = context.Request.Query["number"]; // Получим число из контекста запроса
            try
            {
                int number = Convert.ToInt32(token);
                number = Math.Abs(number);
                string[] Ones = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

                switch (number)
                {
                    case (10):
                        await context.Response.WriteAsync("I AM 1-10, Your number is ten");
                        break;

                    case (<10):
                        // Выдаем окончательный ответ клиенту
                        await context.Response.WriteAsync(" I AM 1-10, Your number is " + Ones[number - 1]); // от 1 до 9
                        break;

                    case (<100):
                        // Записываем в сессионную переменную number результат для компонента FromTwentyToHundredMiddleware
                        // под ключом намбер результат Ones[number % 10 - 1], создает ключ в произвольном виде
                        context.Session.SetString("number", Ones[number % 10 - 1]);
                        break;

                    case (<=9999):
                        int num = (number - ((number/10)*10));
                        if (num == 0)
                        {
                            await _next.Invoke(context);
                        }
                        else
                        {
                            context.Session.SetString("number", Ones[num - 1]);
                        }
                      
                        break;
                }              
            }
            catch(Exception)
            {
                await context.Response.WriteAsync(" I AM 1-10, Incorrect parameter");
            }
        }
    }
}
