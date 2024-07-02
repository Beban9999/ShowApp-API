using System;
using System.Data;
using AppApi.Helper;
using AppApi.Models;
using AppApi.Models.Post;
using AppApi.Models.Artist;
using AppApi.Repository.Contract;
using Microsoft.Data.SqlClient;
using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AppApi.Repository
{
	public class ArtistRepository : IArtistRepository
	{
        public DbHelper _dbHelper;
        private IConfiguration _configuration;

        public ArtistRepository(DbHelper dbHelper, IConfiguration configuration)
        {
            _dbHelper = dbHelper;
            _configuration = configuration;
        }

        public List<Artist> GetArtists(int? userId)
        {
            List<Artist> artists = new List<Artist>();
            List<SqlParameter> parameters = new List<SqlParameter>();
            if(userId != null)
            {
                parameters.Add(new SqlParameter("@UserId", userId));
            }

            DataSet ds = _dbHelper.ExecDsProc(parameters, "usp_GetArtists");
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0) 
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Artist artist = new Artist();
                    artist.UserId = DbTypeHelper.GetInt(dr, "UserID");
                    artist.Name = DbTypeHelper.GetString(dr, "Name");
                    artist.Description = DbTypeHelper.GetString(dr, "Description");
                    artist.Price = DbTypeHelper.GetDecimal(dr, "Price");
                    artist.Location = DbTypeHelper.GetString(dr, "Location");
                    artist.Avatar = DbTypeHelper.GetString(dr, "Avatar");
                    artist.Type = DbTypeHelper.GetString(dr, "Type");

                    artists.Add(artist);
                }

                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[1].Rows)
                    {
                        int id = DbTypeHelper.GetInt(dr, "UserID");
                        string genre = DbTypeHelper.GetString(dr, "Name");

                        Artist artist = artists.FirstOrDefault(a => a.UserId == id);
                        if (artist != null)
                        {
                            artist.Genres.Add(genre);
                        }
                    }
                }

                if(userId != null)
                {
                    if(ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
                    {
                        List<ArtistPost> posts = new List<ArtistPost>();
                        foreach (DataRow dr in ds.Tables[2].Rows)
                        {
                            ArtistPost post = new ArtistPost();
                            post.Id = DbTypeHelper.GetInt(dr, "Id");
                            post.Description = DbTypeHelper.GetString(dr, "Description");
                            post.CreatedDate = DbTypeHelper.GetString(dr, "CreatedDate");
                            post.UserId = DbTypeHelper.GetInt(dr, "UserId");
                            post.Media = new List<ArtistMedia>();
                            posts.Add(post);
                        }

                        if(ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables[3].Rows)
                            {
                                ArtistMedia media = new ArtistMedia();
                                media.FileName = DbTypeHelper.GetString(dr, "FileName");
                                media.FileType = DbTypeHelper.GetString(dr, "FileType");
                                media.PostId = DbTypeHelper.GetInt(dr, "PostId");
                                media.UserId = DbTypeHelper.GetLong(dr, "UserId");

                                ArtistPost post = posts.FirstOrDefault(p => p.Id == media.PostId);
                                if (post != null)
                                {
                                    post.Media.Add(media);
                                }
                            }
                        }

                        foreach (ArtistPost post in posts)
                        {
                            Artist artist = artists.FirstOrDefault(a => a.UserId == post.UserId);
                            if (artist != null)
                            {
                                artist.Posts.Add(post);
                            }
                        }

                    }
                }

            }


            return artists;
        }

        public List<ArtistType> GetArtistTypes()
        {
            List<ArtistType> types = new List<ArtistType>();
            DataTable dt = _dbHelper.ExecProcs(null, "usp_GetArtistTypes");
            foreach (DataRow dr in dt.Rows)
            {
                ArtistType type = new ArtistType();
                type.TypeId = DbTypeHelper.GetInt(dr, "Id");
                type.TypeDescription = DbTypeHelper.GetString(dr, "Description");

                types.Add(type);
            }

            return types;
        }

        public List<ArtistGenre> GetArtistGenres()
        {
            List<ArtistGenre> types = new List<ArtistGenre>();
            DataTable dt = _dbHelper.ExecProcs(null, "usp_GetArtistGenres");
            foreach (DataRow dr in dt.Rows)
            {
                ArtistGenre type = new ArtistGenre();
                type.GenreId = DbTypeHelper.GetInt(dr, "Id");
                type.GenreName = DbTypeHelper.GetString(dr, "Name");

                types.Add(type);
            }

            return types;
        }

        public RequestResponse BecomeArtist(ArtistRequest request)
        {
            RequestResponse response = new RequestResponse();

            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                 {
                    new SqlParameter("@Name", request.Name),
                    new SqlParameter("@Description", request.Description),
                    new SqlParameter("@Price", request.Price),
                    new SqlParameter("@UserId", request.UserId),
                    new SqlParameter("@Location", request.Location),
                    new SqlParameter("@TypeId", request.TypeId)
                };


                DataTable dtGenres = new DataTable();
                dtGenres.Columns.Add(new DataColumn("Id", typeof(int)));
                foreach (var genre in request.Genre)
                {
                    DataRow dr = dtGenres.NewRow();
                    dr["Id"] = genre;
                    dtGenres.Rows.Add(dr);
                }

                SqlParameter genreParamter = new SqlParameter("@Genre", SqlDbType.Structured);
                genreParamter.Value = dtGenres;
                genreParamter.TypeName = "[dbo].[Ids]";
                parameters.Add(genreParamter);

                int resp = _dbHelper.ExecProcReturnScalar(parameters, "usp_BecomeArtist");
                if (resp != 0)
                {
                    response.IsSuccessfull = true;
                }
                else
                {
                    response.IsSuccessfull = false;
                    response.ErrorMessage = "Artist not created!";
                }

            }
            catch (Exception ex)
            {
                response.IsSuccessfull = false;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        public RequestResponse UpdateArtist(UpdateRequest request)
        {
            RequestResponse response = new RequestResponse();

            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@FirstName", request.FirstName),
                    new SqlParameter("@LastName", request.LastName),
                    new SqlParameter("@UserName", request.UserName),
                    new SqlParameter("@Email", request.Email),
                    new SqlParameter("@OldPassword", request.OldPassword),
                    new SqlParameter("@NewPassword", request.NewPassword),
                    new SqlParameter("@ArtistName", request.ArtistName),
                    new SqlParameter("@Location", request.Location),
                    new SqlParameter("@Description", request.Description),
                    new SqlParameter("@Type", request.Type),
                    new SqlParameter("@UserId", request.UserId),
                    new SqlParameter("@IsArtist", request.IsArtist)
                };


                DataTable dtGenres = new DataTable();
                dtGenres.Columns.Add(new DataColumn("Id", typeof(int)));
                foreach (var genre in request.Genre)
                {
                    DataRow dr = dtGenres.NewRow();
                    dr["Id"] = genre;
                    dtGenres.Rows.Add(dr);
                }

                SqlParameter genreParamter = new SqlParameter("@Genre", SqlDbType.Structured);
                genreParamter.Value = dtGenres;
                genreParamter.TypeName = "[dbo].[Ids]";
                parameters.Add(genreParamter);

                int resp = _dbHelper.ExecProcReturnScalar(parameters, "usp_UpdateArtist");
                if (resp != 0)
                {
                    response.IsSuccessfull = true;
                }
                else
                {
                    response.IsSuccessfull = false;
                    response.ErrorMessage = "Artist not updated!";
                }

            }
            catch (Exception ex)
            {
                response.IsSuccessfull = false;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        public RequestResponse InsertPost(ArtistPostRequest request)
        {
            RequestResponse response = new RequestResponse();

            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                 {
                    new SqlParameter("@UserId", request.UserId),
                    new SqlParameter("@Description", request.Description)
                    
                };

                int resp = _dbHelper.ExecProcReturnScalar(parameters, "usp_InsertPost");
                if (resp != 0)
                {
                    response.IsSuccessfull = true;
                    response.Result = resp;
                }
                else
                {
                    response.IsSuccessfull = false;
                    response.ErrorMessage = "Post not created!";
                }

            }
            catch (Exception ex)
            {
                response.IsSuccessfull = false;
                response.ErrorMessage = ex.Message;
            }

            return response;

        }

        public RequestResponse RemovePost(int postId)
        {
            RequestResponse response = new RequestResponse();

            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                 {
                    new SqlParameter("@PostId", postId)

                };

                int resp = _dbHelper.ExecProcReturnScalar(parameters, "usp_RemovePost");
                if (resp != 0)
                {
                    response.IsSuccessfull = true;
                    response.Result = resp;
                }
                else
                {
                    response.IsSuccessfull = false;
                    response.ErrorMessage = "Post not removed!";
                }

            }
            catch (Exception ex)
            {
                response.IsSuccessfull = false;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        public RequestResponse RemoveArtist(int userId)
        {
            RequestResponse response = new RequestResponse();

            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                 {
                    new SqlParameter("@UserId", userId)

                };

                int resp = _dbHelper.ExecProcReturnScalar(parameters, "usp_RemoveArtist");
                if (resp != 0)
                {
                    response.IsSuccessfull = true;
                    response.Result = resp;
                }
                else
                {
                    response.IsSuccessfull = false;
                    response.ErrorMessage = "Artist not removed!";
                }

            }
            catch (Exception ex)
            {
                response.IsSuccessfull = false;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}

