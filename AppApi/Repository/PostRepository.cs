using AppApi.Helper;
using AppApi.Models;
using AppApi.Models.Post;
using AppApi.Repository.Contract;
using Microsoft.Data.SqlClient;
using System.Data;

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

            DataTable dt = _dbHelper.ExecProcs(parameters, "usp_GetPosts");
            if(dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Post post = new Post();
                    post.Id = DbTypeHelper.GetInt(dr, "Id");
                    post.Title = DbTypeHelper.GetString(dr, "Title");
                    post.Description = DbTypeHelper.GetString(dr, "Description");
                    post.Price = DbTypeHelper.GetDecimal(dr, "Price");

                    parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@PostId", post.Id)
                    };

                    DataTable dtMedia = _dbHelper.ExecProcs(parameters, "usp_GetPostMedia");
                    if(dtMedia.Rows.Count > 0)
                    {
                        List<PostMedia> medias = new List<PostMedia>();
                        foreach (DataRow drMedia in dtMedia.Rows)
                        {
                            PostMedia media = new PostMedia();
                            media.FileName = DbTypeHelper.GetString(drMedia, "FileName");
                            media.FileType = DbTypeHelper.GetString(drMedia, "FileType");
                            media.PostId = DbTypeHelper.GetLong(drMedia, "PostId");

                            medias.Add(media);
                        }

                        post.Medias = medias;
                    }
                    posts.Add(post);
                }
            }

            return posts;
        }

        public RequestResponse InsertPost(Post postsRequest)
        {
            RequestResponse response = new RequestResponse();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Title", postsRequest.Title),
                    new SqlParameter("@Description", postsRequest.Description),
                    new SqlParameter("@Price", postsRequest.Price),
                    new SqlParameter("@Username", postsRequest.User)
                };
                
                int resp = _dbHelper.ExecProcReturnScalar(parameters, "usp_InsertPost");
                if(resp != 0)
                {
                    response.Result = resp;
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
