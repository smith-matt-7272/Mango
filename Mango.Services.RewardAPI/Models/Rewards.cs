namespace Mango.Services.RewardAPI.Models
{
    public class Rewards
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public DateTime RewardsDate { get; set; }
        public int RewardsActivity { get; set; }
        public int OrderID { get; set; }
    }
}
