﻿namespace Bulky.Models.ViewModels.Admin
{
	public class UserListViewModel
	{
		public int Id { get; set; }
        public string Name { get; set; }
		public string Email { get; set; } 
		public string? Phone { get; set; }
		public string? Company { get; set; }
		public string Role { get; set; }
		public bool IsLocked { get; set; }
    }
}
