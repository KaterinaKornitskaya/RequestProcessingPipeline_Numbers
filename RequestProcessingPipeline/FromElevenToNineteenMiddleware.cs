namespace RequestProcessingPipeline
{
    public class FromElevenToNineteenMiddleware
    {
        private readonly RequestDelegate _next;

        public FromElevenToNineteenMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string? token = context.Request.Query["number"];
            try
            {
                string[] Numbers = { "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                int number = Convert.ToInt32(token);
                number = Math.Abs(number);

                switch (number)
                {
                    case (<100):
                        if (number < 11 || number > 19)
                        {
                            await _next.Invoke(context);  //Контекст запроса передаем следующему компоненту
                        }
                        else
                        {
                            // Выдаем окончательный ответ клиенту
                            await context.Response.WriteAsync(" I AM 11-19, Your number is " + Numbers[number - 11]);
                        }
                    break;

                    case (<9999):
                        if ((number-((number/100)*100)<=11)
                        ||   (number-((number/100)*100)>19))
                        {
                            await _next.Invoke(context);
                        }
                        else
                        {
                            context.Session.SetString("number", $"{Numbers[number-((number/100)*100)-11]}" );
                        }
                        break;
                }
            }
            catch (Exception)
            {
                // Выдаем окончательный ответ клиенту
                await context.Response.WriteAsync(" I AM 11-19, Incorrect parameter");
            }
        }
    }
}
