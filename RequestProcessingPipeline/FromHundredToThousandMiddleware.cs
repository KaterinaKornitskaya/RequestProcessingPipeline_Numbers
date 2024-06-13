using Microsoft.AspNetCore.Http;

namespace RequestProcessingPipeline
{
    public class FromHundredToThousandMiddleware(RequestDelegate _next)
    {
        // Primary консруктор выше - аналог закомментированных строк ниже
        //private readonly RequestDelegate _next;

        //public FromHundredToThousandMiddleware(RequestDelegate next)
        //{
        //    _next = next;
        //}

        public async Task Invoke(HttpContext context)
        {
            string? taken = context.Request.Query["number"];

            try
            {
                int number = Convert.ToInt32(taken);
                number = Math.Abs(number);

                string[] Ones = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

                switch (number)
                {
                    case (<100):
                        // контекст запроса передаем следующему компоненту
                        await _next.Invoke(context);
                        break;

                    case (<=999):
                        if (number % 100 == 0)
                        {
                            await context.Response.WriteAsync(" I AM 100-1000, Your number is " + Ones[(number/100)-1] + " hundred");
                        }

                        else
                        {
                            await _next.Invoke(context);
                            string? result = string.Empty;
                            result = context.Session.GetString("number");
                            await context.Response.WriteAsync(" I AM 100-1000, Your number is " + Ones[(number/100)-1] + " hundred " + result);
                        }
                        break;

                    case (<=9999):
                        int num = (number - ((number/1000)*1000))/100;
                        if(num == 0)
                        {
                            await _next.Invoke(context);
                        }
                        else if (number % 100 == 0)
                        {
                            context.Session.SetString("number", $"{Ones[(num-1)]} hundreds");
                        }
                        else
                        {
                            // сначала отдадим следующему компоненту
                            await _next.Invoke(context);
                            string? result = string.Empty;
                            result = context.Session.GetString("number");
                            context.Session.SetString("number", $"{Ones[(num-1)]} hundreds" + " " + result);

                        }
                        break;
                }

                //if (number < 100)
                //{
                //    // контекст запроса передаем следующему компоненту
                //    await _next.Invoke(context);
                //}
               
                //else if(number >=100 && number <= 999)
                //{
                //    if (number % 100 == 0)
                //    {
                //        await context.Response.WriteAsync(" I AM 100-1000, Your number is " + Ones[(number/100)-1] + " hundred");
                //    }
                  
                //    else
                //    {
                //        await _next.Invoke(context);
                //        string? result = string.Empty;
                //        result = context.Session.GetString("number");
                //        await context.Response.WriteAsync(" I AM 100-1000, Your number is " + Ones[(number/100)-1] + " hundred " + result);
                //    }
                //}
            }
            catch (Exception)
            {
                await context.Response.WriteAsync(" I AM 100-1000, Incorrect parameter");
            }
        }
    }
}
