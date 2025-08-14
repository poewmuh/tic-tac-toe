using System;
using UniRx;
using Unity.Netcode;

namespace TicTacToe.Gameplay.Helper
{
    public static class RxNetworkExtensions
    {
        public static IObservable<T> ObserveValue<T>(this NetworkVariable<T> nv)
        {
            return Observable.Create<T>(observer =>
            {
                void Handler(T oldV, T newV) => observer.OnNext(newV);
                nv.OnValueChanged += Handler;

                return Disposable.Create(() => nv.OnValueChanged -= Handler);
            });
        }
        
        public static IObservable<NetworkListEvent<T>> ObserveListChanged<T>(this NetworkList<T> list) where T : unmanaged, IEquatable<T>
        {
            return Observable.Create<NetworkListEvent<T>>(observer =>
            {
                void Handler(NetworkListEvent<T> e) => observer.OnNext(e);
                list.OnListChanged += Handler;

                return Disposable.Create(() => list.OnListChanged -= Handler);
            });
        }
        
        public static IObservable<ulong> ObserveClientConnected(this NetworkManager nm)
        {
            return Observable.Create<ulong>(observer =>
            {
                void H(ulong id) => observer.OnNext(id);
                nm.OnClientConnectedCallback += H;
                return Disposable.Create(() => nm.OnClientConnectedCallback -= H);
            });
        }

        public static IObservable<ulong> ObserveClientDisconnected(this NetworkManager nm)
        {
            return Observable.Create<ulong>(observer =>
            {
                void H(ulong id) => observer.OnNext(id);
                nm.OnClientDisconnectCallback += H;
                return Disposable.Create(() => nm.OnClientDisconnectCallback -= H);
            });
        }
    }
}