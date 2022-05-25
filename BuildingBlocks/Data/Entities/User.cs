using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BuildingBlocks.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string AboutMe { get; set; }
        public string Adress { get; set; }
        public UserStatus Status { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Message> MessageSends { get; set; }
        public virtual ICollection<Message> MessageReceivers { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<UserSkill> UserSkills { get; set; }
    }

    public enum UserStatus
    {
        [Display(Name = "Đang hoạt động")] Enabled = 1,
        [Display(Name = "Ngưng hoạt động")] Disabled = 2
    }

    public enum UserRole
    {
        Admin = 1,
        Member = 2
    }
}