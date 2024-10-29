
namespace Server.Utils;

public struct EphemeralValue<T> 
where T : notnull {
    public DateTime Expiry { get; set; }
    public T Value { get; set; }
}

public class EphemeralKeyStore<K, V> : Dictionary<K, EphemeralValue<V>> 
where K : notnull 
where V : notnull {

    private Task _cleanupTask;

    public EphemeralKeyStore() : base() {
        _cleanupTask = Task.Run(async () => {
            while (true) {
                await Task.Delay(1000);
                var now = DateTimeOffset.Now;
                foreach (var key in this.Keys) {
                    if (this[key].Expiry < now) {
                        Remove(key);
                    }
                }
            }
        });
    }

    public void Add(K key, V value, TimeSpan ttl) {
        this[key] = new EphemeralValue<V> {
            Expiry = DateTime.Now.Add(ttl),
            Value = value
        };
    }

    public bool TryGetValue(K key, out V value) {
        if (base.TryGetValue(key, out var ephemeralValue)) {
            if (ephemeralValue.Expiry > DateTimeOffset.Now) {
                value = ephemeralValue.Value;
                return true;
            }
            Remove(key);
        }
        value = default;
        return false;
    }

    ~EphemeralKeyStore() {
        _cleanupTask.Dispose();
    }
}