using UnityEngine;
using VContainer;
using VContainer.Unity;
	
public class PlayerLifetimeScope : LifetimeScope
{
    //[SerializeField] private PlayerInputHandler inputHandler;
    //[SerializeField] private PlayerMovement movement;
    //[SerializeField] private PlayerLook look;

    protected override void Configure(IContainerBuilder builder)
    {
        // Register components
        //builder.RegisterComponent(inputHandler);
        //builder.RegisterComponent(movement);
        //builder.RegisterComponent(look);

        // Bind logic interfaces if needed (optional)
        // builder.RegisterEntryPoint<PlayerController>();
    }
}
