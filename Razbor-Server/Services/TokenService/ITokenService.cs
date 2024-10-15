namespace Services.TokenService
{
    public interface ITokenService
    {
        public string Generate(UserInfoTable user);
    }
}
