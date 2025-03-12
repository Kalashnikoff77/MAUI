namespace Data.Dto.Requests
{
    public class GetFriendsForAccountsRequestDto : GetAccountsRequestDto
    {
        public override string Uri => "/Accounts/GetFriends";
    }
}
