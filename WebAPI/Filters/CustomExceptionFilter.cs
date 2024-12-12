using Common.Dto.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Text.Json;
using WebAPI.Exceptions;

namespace WebAPI.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            int statusCode = 500;

            switch(context.Exception)
            {
                case BadRequestException:
                    statusCode = (int)HttpStatusCode.BadRequest; break;

                case UnauthorizedException:
                    statusCode = (int)HttpStatusCode.Unauthorized; break;

                case NotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound; break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError; break;
            }

            var json = JsonSerializer.Serialize(new ResponseDtoBase { ErrorMessage = context.Exception.Message });

            context.Result = new ContentResult 
            { 
                Content = json,
                ContentType = "application/json",
                StatusCode = statusCode
            };

            context.ExceptionHandled = true;
        }
    }
}
