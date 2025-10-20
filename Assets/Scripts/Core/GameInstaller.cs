using VContainer;
using VContainer.Unity;

namespace Core
{
    public class GameInstaller : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // reg core sys
            builder.Register<DDOL>(Lifetime.Singleton);
            builder.Register<SceneLoader>(Lifetime.Singleton);
            builder.Register<Bootstrap>(Lifetime.Singleton);
            //builder.Register<PlayerLifeTimeScope>(Lifetime.Singleton);
            // builder.Register<AudioManager>(Lifetime.Singleton);
            // builder.Register<SaveSystem>(Lifetime.Singleton);
            // builder.Register<EventBus>(Lifetime.Singleton);
            //
            // // reg game sys
            // builder.Register<DialogueManager>(Lifetime.Singleton);
            // builder.Register<InventorySystem>(Lifetime.Singleton);
            // builder.Register<QuestSystem>(Lifetime.Singleton);
        }
    }
}