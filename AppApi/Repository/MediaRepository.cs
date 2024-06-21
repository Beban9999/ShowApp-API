using System;
using System.Data;
using AppApi.Helper;
using AppApi.Models;
using AppApi.Repository.Contract;
using Microsoft.Data.SqlClient;

namespace AppApi.Repository
{
	public class MediaRepository : IMediaRepository
	{
        public DbHelper _dbHelper;
        public MediaRepository(DbHelper dbHelper)
		{
			_dbHelper = dbHelper;
		}

        public RequestResponse UploadMedia(IFormFileCollection? mediaData, int userId, bool isProfile, int? postId)
        {
            RequestResponse response = new RequestResponse();

            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@IsProfile", isProfile),
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@PostId", postId)
                };

                if (mediaData != null && mediaData.Count != 0)
                {
                    DataTable dtFiles = new DataTable();
                    dtFiles.Columns.Add(new DataColumn("FileName", typeof(string)));
                    dtFiles.Columns.Add(new DataColumn("FileType", typeof(string)));
                    dtFiles.Columns.Add(new DataColumn("UserId", typeof(long)));
                    foreach (var media in mediaData)
                    {
                        // Check if the media is an image or video based on its content type
                        bool isImage = media.ContentType.ToLower().StartsWith("image");
                        bool isVideo = media.ContentType.ToLower().StartsWith("video");

                        if (isImage || isVideo)
                        {
                            // Store media on the server
                            string fileName = media.FileName;
                            string directoryPath = Path.Combine("/Users/vrusic/Developer/Projects/ShowApp-UI/media/" + userId);
                            string filePath = Path.Combine(directoryPath, fileName);

                            if (!Directory.Exists(directoryPath))
                            {
                                Directory.CreateDirectory(directoryPath);
                            }

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                media.CopyTo(stream);
                            }

                            // Add database parameters
                            DataRow dr = dtFiles.NewRow();
                            dr["FileName"] = media.FileName;
                            dr["FileType"] = media.ContentType;
                            dr["UserId"] = userId;
                            dtFiles.Rows.Add(dr);
                        }
                        else
                        {
                            response.IsSuccessfull = false;
                            response.ErrorMessage = "Unsupported media type";
                            break;
                        }
                    }

                    SqlParameter fileParameter = new SqlParameter("@Media", SqlDbType.Structured);
                    fileParameter.Value = dtFiles;
                    fileParameter.TypeName = "[dbo].[MediaFile]";
                    parameters.Add(fileParameter);

                    int resp = _dbHelper.ExecProcReturnScalar(parameters, "usp_InsertArtistMedia");
                    if (resp != 0)
                    {
                        response.IsSuccessfull = true;
                    }
                    else
                    {
                        response.IsSuccessfull = false;
                        response.ErrorMessage = "Media not inserted!";
                    }
                }
                else
                {
                    response.IsSuccessfull = true;
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

