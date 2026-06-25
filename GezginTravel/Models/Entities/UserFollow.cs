using GezginTravel.Models.Identity;

namespace GezginTravel.Models.Entities
{
    public class UserFollow : BaseEntity
    {
        
        public int FollowerId { get; set; }
        public AppUser Follower { get; set; }

        public int FollowingId { get; set; }
        public AppUser Following { get; set; }
    }
}
