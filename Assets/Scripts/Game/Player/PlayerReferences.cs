// using UnityEngine;
//
// namespace Core.Game.Player
// {
//     public class PlayerReferences : MonoBehaviour
//     {
//         [field: Header("Core References")]
//         [field: SerializeField] public CharacterController Controller { get; private set; }
//         [field: SerializeField] public Transform CameraPivot { get; private set; }
//         [field: SerializeField] public Camera PlayerCamera { get; private set; }
//
//         [field: Header("Optional References")]
//         [field: SerializeField] public Animator Animator { get; private set; }
//         [field: SerializeField] public AudioSource AudioSource { get; private set; }
//
//         private void OnValidate()
//         {
//             // Try auto-assign to save manual setup
//             if (!Controller) Controller = GetComponent<CharacterController>();
//             if (!PlayerCamera && CameraPivot)
//                 PlayerCamera = CameraPivot.GetComponentInChildren<Camera>();
// 			if (!Movement) Movement = GetComponent<PlayerMovement>();
//         	if (!Look) Look = GetComponent<PlayerLook>();
//     		
//         }
//     }
// }