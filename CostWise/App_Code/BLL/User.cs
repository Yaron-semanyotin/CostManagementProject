using System;

namespace CostWise.App_Code.BLL
{
    public class User
    {
        public int UserId { get; set; }
        public int BusinessId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}