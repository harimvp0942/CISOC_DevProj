using CASIO_HariKrishna.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;
using Microsoft.Extensions.Logging;
using Google.Cloud.Storage.V1;
using System;
using System.IO;
using System.Collections.Generic;
using CASIO_HariKrishna.EntityFrameWork;
using System.Linq;

namespace CASIO_HariKrishna.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UserController> _logger;
        private readonly CisocContext _cisocContext;
        private readonly ITokenService _tokenService;
        public UserController(IHttpClientFactory httpClientFactory, ILogger<UserController> logger, CisocContext cisocContext, ITokenService tokenService)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _cisocContext = cisocContext;
            _tokenService = tokenService;
        }

        [HttpGet]
        [Route("userdetails")]
        public async Task<UserDetailModel> GetUserDetails()
        {
            try
            {
                _logger.LogInformation("UserController | GetUserDetails | Started..");
                UserDetailModel userdetails = new UserDetailModel();
                var httpClient = _httpClientFactory.CreateClient("UserApiClient");
                var httpResponse = await httpClient.GetAsync("/api/userdetails");
                if (httpResponse != null && httpResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(jsonResponse))
                    {
                        userdetails = JsonConvert.DeserializeObject<UserDetailModel>(jsonResponse);
                    }
                    _logger.LogInformation("UserController | GetUserDetails | The response receieved with success.");
                }
                else
                {
                    _logger.LogInformation("UserController | GetUserDetails | Error while fetching the response from end point  : " + httpClient.BaseAddress);
                }
                return userdetails;
            }
            catch (System.Exception ex)
            {
                _logger.LogError("UserController | GetUserDetails | Exception occured : " + ex.Message);
                throw;
            }
        }

        [HttpPost]
        public async Task<string> UploadMediaToCloud([FromBody] List<PageFile> pageFiles)
        {
            try
            {
                _logger.LogInformation("UserController | UploadMediaToCloud | Started..");
                if (pageFiles != null && pageFiles.Count > 0)
                {
                    foreach (var pageFile in pageFiles)
                    {
                        Token bearerToken = await _tokenService.GetElibilityToken();
                        if (!string.IsNullOrWhiteSpace(bearerToken.AccessToken))
                        {
                            string bucketName = "CISCO-Bucket";
                            var gcsStorage = StorageClient.Create();
                            using (var f = System.IO.File.OpenRead(pageFile.FileName))
                            {
                                await gcsStorage.UploadObjectAsync(bucketName, pageFile.ObjectName, null, f);
                            }
                            _cisocContext.tblUploads.Add(new Entities.tblUpload
                            {
                                FileName = pageFile.FileName,
                                FileContent = pageFile.ObjectName,
                                CreatedBy = "dbo",
                                CreatedDate = DateTime.UtcNow
                            });
                            _cisocContext.SaveChanges();
                        }
                        else
                        {
                            return "Problem occured while generating the bearer token.";
                        }
                    }
                    _logger.LogInformation("UserController | UploadMediaToCloud | Successfully uploaded to cloud and updated in db.");
                }
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError("UserController | UploadMediaToCloud | Exception occured : " + ex.Message);
                throw;
            }
        }

        [HttpPatch]
        public PageContent ProcessPageContent([FromBody] PageContent pagecontent)
        {
            try
            {
                if (pagecontent != null)
                {
                    var upload = _cisocContext.tblUploads.FirstOrDefault(x => x.FileName == pagecontent.FileName);
                    if (upload != null)
                    {
                        pagecontent.Id = upload.Id;
                        upload.FileContent = pagecontent.ObjectContent;
                        upload.UpdatedBy = "dbo";
                        upload.UpdatedDate = DateTime.UtcNow;
                        _cisocContext.Entry(upload).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                    _cisocContext.SaveChanges();
                }
                return pagecontent;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserController | ProcessPageContent | Exception occured : " + ex.Message);
                throw;
            }

        }

    }
}
