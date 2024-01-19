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

        public RequestResponse UploadMedia(IFormFileCollection? mediaData, int postId)
        {
            RequestResponse response = new RequestResponse();

            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (mediaData != null && mediaData.Count != 0)
                {
                    DataTable dtPostFiles = new DataTable();
                    dtPostFiles.Columns.Add(new DataColumn("FileName", typeof(string)));
                    dtPostFiles.Columns.Add(new DataColumn("FileType", typeof(string)));
                    dtPostFiles.Columns.Add(new DataColumn("PostId", typeof(long)));
                    foreach (var media in mediaData)
                    {
                        // Check if the media is an image or video based on its content type
                        bool isImage = media.ContentType.ToLower().StartsWith("image");
                        bool isVideo = media.ContentType.ToLower().StartsWith("video");

                        if (isImage || isVideo)
                        {
                            // Store media on the server
                            string fileName = media.FileName;
                            string directoryPath = Path.Combine("/Users/vrusic/Developer/Projects/ShowApp-UI/media/" + postId);
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
                            DataRow dr = dtPostFiles.NewRow();
                            dr["FileName"] = media.FileName;
                            dr["FileType"] = media.ContentType;
                            dr["PostId"] = postId;
                            dtPostFiles.Rows.Add(dr);
                        }
                        else
                        {
                            response.IsSuccessfull = false;
                            response.ErrorMessage = "Unsupported media type";
                            break;
                        }
                    }

                    SqlParameter fileParameter = new SqlParameter("@PostMediaData", SqlDbType.Structured);
                    fileParameter.Value = dtPostFiles;
                    fileParameter.TypeName = "[dbo].[PostFile]";
                    parameters.Add(fileParameter);

                    int resp = _dbHelper.ExecProcReturnScalar(parameters, "usp_InsertPostMedia");
                    if (resp != 0)
                    {
                        response.IsSuccessfull = true;
                    }
                    else
                    {
                        response.IsSuccessfull = false;
                        response.ErrorMessage = "Post media not inserted!";
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

