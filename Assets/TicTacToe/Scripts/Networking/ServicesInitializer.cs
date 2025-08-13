using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Zenject;

namespace TicTacToe.Networking
{
    public class ServicesInitializer : IInitializable
    {
        public void Initialize()
        {
            InitializeAsync().Forget();
        }

        async UniTaskVoid InitializeAsync()
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
}