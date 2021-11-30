namespace BennyKok.ReactiveProperty
{
    public interface IPersistenceKeyProvider
    {
        public string GetPersistenceKey(string currentKey) => currentKey;
    }
}