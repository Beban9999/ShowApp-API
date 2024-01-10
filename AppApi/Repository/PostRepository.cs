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

            DataTable dt = _dbHelper.ExecProc(parameters, "usp_GetPosts");
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

                    DataTable dtImages = _dbHelper.ExecProc(parameters, "usp_GetPostImages");
                    if(dtImages.Rows.Count > 0)
                    {
                        List<PostImage> images = new List<PostImage>();
                        foreach (DataRow drImage in dtImages.Rows)
                        {
                            PostImage image = new PostImage();
                            byte[] file = (byte[])drImage["ImageData"];
                            
                            image.ImageFile = Convert.ToBase64String(file);
                            image.ImageFileType = DbTypeHelper.GetString(drImage, "ImageType");
                            image.ImageName = DbTypeHelper.GetString(drImage, "ImageName");

                            images.Add(image);
                        }

                        post.Images = images;
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
                    new SqlParameter("@Price", postsRequest.Price)
                };

                if(postsRequest.Images.Count > 0)
                {
                    DataTable dtPostImages = new DataTable();
                    dtPostImages.Columns.Add(new DataColumn("ImageName"));
                    dtPostImages.Columns.Add(new DataColumn("ImageFileType"));
                    dtPostImages.Columns.Add(new DataColumn("ImageFile", typeof(byte[])));
                    foreach (var img in postsRequest.Images)
                    {
                        DataRow dr = dtPostImages.NewRow();
                        dr["ImageName"] = img.ImageName;
                        dr["ImageFileType"] = img.ImageFileType;
                        //byte[] file = Convert.FromBase64String(img.ImageFile);
                        //dr["ImageFile"] = file;
                        dtPostImages.Rows.Add(dr);
                    }

                    SqlParameter imageParameter = new SqlParameter("@PostImages", SqlDbType.Structured);
                    imageParameter.Value = dtPostImages;
                    imageParameter.TypeName = "[dbo].[PostImage]";
                    parameters.Add(imageParameter);
                }

                
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
