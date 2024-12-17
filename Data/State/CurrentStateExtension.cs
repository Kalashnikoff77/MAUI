using Data.Models.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace Data.State
{
    public static class CurrentStateExtension
    {
        /// <summary>
        /// Обработчик ответа внутри модели
        /// </summary>
        public static IDisposable? SignalRClient<T>(this IDisposable? clientHandler, CurrentState currentState)
            where T : SignalRModelBase<T>, new()
        {
            T TModel = new T();

            if (clientHandler == null && currentState.SignalR != null)
                clientHandler = currentState.SignalR.On(TModel.EnumSignalRHandlersClient.ToString(), TModel.Func(currentState));

            return clientHandler;
        }

        /// <summary>
        /// Обработчик ответа в параметрах
        /// </summary>
        public static IDisposable? SignalRClient<T>(this IDisposable? clientHandler, CurrentState currentState, Func<T, Task> func)
            where T : SignalRModelBase<T>, new()
        {
            T TModel = new T();

            if (clientHandler == null && currentState.SignalR != null)
                clientHandler = currentState.SignalR.On<T>(TModel.EnumSignalRHandlersClient.ToString(), func.Invoke);

            return clientHandler;
        }

        /// <summary>
        /// Отправка запроса на сервер SignalR
        /// </summary>
        public static Task SignalRServerAsync(this CurrentState currentState, SignalGlobalRequest request)
        {
            if (currentState.SignalR != null)
                return currentState.SignalR.SendAsync("GlobalHandler", request);
            else
                return Task.CompletedTask;
        }
    }
}
