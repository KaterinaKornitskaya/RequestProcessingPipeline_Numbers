namespace RequestProcessingPipeline
{
    public class FromThousandToTenThousandMiddleware
    {
        // ссылка на следующий компонент конвеера
        private readonly RequestDelegate _next;

        // передаем ссылку на след. компонент в конструктор
        public FromThousandToTenThousandMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string? token = context.Request.Query["number"];
            string[] Ones = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
            try
            {
                int number = int.Parse(token);
                number = Math.Abs(number);

                switch (number)
                {
                    // если число меньше тысячи, передаем следующему компоненту
                    case (<1000): 
                        await _next.Invoke(context);
                        break;

                    // если число 1000-10000 - обрабатываем в этом компоненте
                    case (<10000):
                        // если кратно 1000 - выдаем результат клиенту
                        if(number % 1000 == 0)
                        {
                            await context.Response.WriteAsync($"I am 1000-9999, Your number is {Ones[(number/1000)-1]} thousand");
                        }
                        else
                        {
                            // иначе передаем следующему компоненту
                            await _next.Invoke(context);
                            string? res = string.Empty;
                            // получаем результат от предыдущих компонентов
                            res  = context.Session.GetString("number");
                            await context.Response.WriteAsync($"I am 1000-9999, Your number is {Ones[(number/1000)-1]} thousand {res}");
                        }
                        break;
                    case (10000):
                        await context.Response.WriteAsync("I am 1000-9999, Your number is ten thousand");
                        break;
                    case (>=10000):
                        await context.Response.WriteAsync("Your number is more then 10,000");
                        break;
                }
            }
            catch (Exception ex)
            {
                await context.Response.WriteAsync("I am 1000-9999, Incorrect parameter");
            }
        }
    }
}
