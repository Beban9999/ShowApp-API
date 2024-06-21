using System;
using System.Data;
using AppApi.Helper;
using AppApi.Models;
using AppApi.Models.Chat;
using AppApi.Repository.Contract;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppApi.Repository
{
    public class MessageRepository : IMessageRepository
    {
        public DbHelper _dbHelper;
        public MessageRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public Response CreateRoom(int receiverId, int senderId)
        {
            Response response = new Response();

            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@SenderId", senderId),
                    new SqlParameter("@ReceiverId", receiverId)
                };

                int result = _dbHelper.ExecProcReturnScalar(parameters, "usp_CreateRoom");
                if (result > 0)
                {
                    response.Status = RequestStatus.Success;
                    response.Data = result.ToString();
                }
            }
            catch (Exception ex)
            {
                response.Status = RequestStatus.Error;
                response.Message = ex.Message;
            }

            return response;
        }

        public List<Message> GetRoomMessages(int roomId, int userId)
        {
            List<Message> messages = new List<Message>();
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@RoomId", roomId),
                new SqlParameter("@UserId", userId)
            };

            DataTable dt = _dbHelper.ExecProcs(parameters, "usp_GetRoomMessages");

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Message message = new Message
                    (
                        roomId,
                        DbTypeHelper.GetString(dr, "SenderId"),
                        DbTypeHelper.GetString(dr, "ReceiverId"),
                        DbTypeHelper.GetString(dr, "Content"),
                        DbTypeHelper.GetString(dr, "Date"),
                        DbTypeHelper.GetString(dr, "Timestamp")
                    );

                    messages.Add(message);
                }
            }

            return messages;
        }

        public List<Room> GetUserRooms(int userId)
        {
            List<Room> rooms = new List<Room>();
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId)
            };

            DataTable dt = _dbHelper.ExecProcs(parameters, "usp_GetUserRooms");

            if(dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Room room = new Room
                    (
                        DbTypeHelper.GetString(dr, "RoomName"),
                        DbTypeHelper.GetString(dr, "Avatar"),
                        DbTypeHelper.GetInt(dr, "RoomId"),
                        DbTypeHelper.GetString(dr, "SenderId"),
                        DbTypeHelper.GetString(dr, "Sender"),
                        DbTypeHelper.GetString(dr, "ReceiverId"),
                        DbTypeHelper.GetString(dr, "Receiver")
                    );

                    rooms.Add(room);
                }
            }

            return rooms;
        }

        public Response InsertMessage(int roomId, string senderId, string content, string date, string timestamp)
        {
            Response response = new Response();

            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@RoomId", roomId),
                    new SqlParameter("@SenderId", senderId),
                    new SqlParameter("@Content", content),
                    new SqlParameter("@Date", date),
                    new SqlParameter("@Timestamp", timestamp)
                };

                DataTable dt = _dbHelper.ExecProcs(parameters, "usp_InsertMessage");
                if (dt.Rows.Count > 0)
                {
                    List<Message> messages = new List<Message>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        Message message = new Message
                        (
                            roomId,
                            DbTypeHelper.GetString(dr, "SenderId"),
                            DbTypeHelper.GetString(dr, "ReceiverId"),
                            DbTypeHelper.GetString(dr, "Content"),
                            DbTypeHelper.GetString(dr, "Date"),
                            DbTypeHelper.GetString(dr, "Timestamp")
                        );

                        messages.Add(message);
                    }

                    response.Status = RequestStatus.Success;
                    response.Data = JsonConvert.SerializeObject(messages);
                }
            }
            catch (Exception ex)
            {
                response.Status = RequestStatus.Error;
                response.Message = ex.Message;
            }

            return response;
        }

        public int GetUserUnreadMessages(int userId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId)
            };

            int result = _dbHelper.ExecProcReturnScalar(parameters, "usp_GetUserUnreadMessages");

            return result;
        }
    }
}

