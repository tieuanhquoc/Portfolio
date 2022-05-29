using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BuildingBlocks.Data.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string Password { get; set; }

        public string Avatar { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string AboutMe { get; set; }
        public string Address { get; set; }
        public UserStatus Status { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<Room> RoomCreated { get; set; }
        public virtual ICollection<Room> RooJoined { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<UserSkill> UserSkills { get; set; }
    }

    public enum UserStatus
    {
        [Display(Name = "Enabled")] Enabled = 1,
        [Display(Name = "Disabled")] Disabled = 2,
        [Display(Name = "Waiting for accept")] Waiting = 3
    }

    public enum UserRole
    {
        Admin = 1,
        Member = 2
    }
}