using Game.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Game.Systems
{
    public class HungerSystem : MonoBehaviour
    {
        [SerializeField] private float maxHunger = 100f;
        [SerializeField] private float hungerDecreaseRate = 0.5f; // per second
        [SerializeField] private float minHungerForMovement = 10f; // character slows when below this

        public float CurrentHunger { get; private set; }
        public float HungerPercent => CurrentHunger / maxHunger;
        public bool IsExhausted => CurrentHunger <= minHungerForMovement;
        
        [FormerlySerializedAs("OnHungerChanged")] public UnityEvent<float> onHungerChanged = new(); // sends HungerPercent
        [FormerlySerializedAs("OnExhausted")] public UnityEvent onExhausted = new();
        [FormerlySerializedAs("OnFed")] public UnityEvent onFed = new();

        private PlayerMovement movement;

        private void Awake()
        {
            movement = GetComponent<PlayerMovement>();
        }

        private void Start()
        {
            CurrentHunger = maxHunger * 0.6f; // Start at 60%
            onHungerChanged?.Invoke(HungerPercent);
        }

        private void Update()
        {
            DecreaseHunger(hungerDecreaseRate * Time.deltaTime);

            // Optional: Slow character if exhausted
            if (IsExhausted && movement != null)
            {
                // You could slow movement here if desired
            }
        }

        public void Eat(float amount)
        {
            CurrentHunger = Mathf.Min(CurrentHunger + amount, maxHunger);
            onHungerChanged?.Invoke(HungerPercent);
            onFed?.Invoke();
        }

        private void DecreaseHunger(float amount)
        {
            CurrentHunger = Mathf.Max(CurrentHunger - amount, 0);
            onHungerChanged?.Invoke(HungerPercent);

            if (CurrentHunger == 0)
                onExhausted?.Invoke();
        }

        public void SetHunger(float amount)
        {
            CurrentHunger = Mathf.Clamp(amount, 0, maxHunger);
            onHungerChanged?.Invoke(HungerPercent);
        }
    }
}