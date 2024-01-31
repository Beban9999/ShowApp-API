using System;
namespace AppApi.Models.Chat
{
	public class RoomUser
	{
        public RoomUser(string id, string username)
        {
            this._id = id;
            this.username = username;
        }

        public string _id { get; set; }
		public string username { get; set; }
	}
}

