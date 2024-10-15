namespace Services.HashingService
{
    public interface IHashingService
    {
        public bool Verify(string hashedPassword, string password);

        public string Hash(string password);
    }
}
