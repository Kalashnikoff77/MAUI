using Data.Enums;
using Data.State;
using Microsoft.JSInterop;

namespace Data.Models.SignalR
{
    public abstract class SignalRModelBase<T>
    {
        /// <summary>
        /// Название метода, вызывающегося в JS
        /// </summary>
        public abstract EnumSignalRHandlers EnumSignalRHandlersClient { get; }

        /// <summary>
        /// По умолчанию сразу вызывается метод в JS. Иначе класс можно переопределить
        /// </summary>
        public virtual Func<T, Task> Func(CurrentState currentState) => async (response) =>
            await currentState.JS.InvokeVoidAsync(EnumSignalRHandlersClient.ToString(), response);
    }
}
