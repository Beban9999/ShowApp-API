using AppApi.Helper;
using AppApi.Models;
using AppApi.Models.Post;
using AppApi.Repository.Contract;
using Microsoft.Data.SqlClient;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AppApi.Repository
{
    public class PostRepository : IPostRepository
    {
        public DbHelper _dbHelper;
        public PostRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public List<Post> Get_Post(int? id)
        {
            List<Post> posts = new List<Post>();
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@PostId", id ?? (object)DBNull.Value)
            };

            DataTable dt = _dbHelper.ExecProc(parameters, "usp_GetPosts");
            if(dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Post post = new Post();
                    post.Name = DbTypeHelper.GetString(dr, "Name");
                    //i tako dalje, posebna petlja za slike

                    posts.Add(post);
                }
            }

            return posts;
        }

        public RequestResponse InsertPost(PostsRequest postsRequest)
        {
            RequestResponse response = new RequestResponse();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Title", postsRequest.Title),
                    new SqlParameter("@Description", postsRequest.Description),
                    new SqlParameter("@Price", postsRequest.Price)
                };

                int resp = _dbHelper.ExecProcReturnScalar(parameters, "usp_InsertPost");
                if(resp == 1)
                {
                    response.IsSuccessfull = true;
                }
                else
                {
                    response.IsSuccessfull = false;
                    response.ErrorMessage = "Post not inserted!";
                }

            }
            catch(Exception ex)
            {
                response.IsSuccessfull = false;
                response.ErrorMessage = ex.Message;
            }


            return response;
        }
    
    }
}
